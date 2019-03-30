
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HuntLog.AppModule.Hunts;
using HuntLog.Helpers;
using HuntLog.Models;
using HuntLog.Services;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace HuntLog.AppModule.Logs
{
    public partial class LogView : ContentPage
    {
        private readonly LogViewModel _viewModel;

        public LogView(LogViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
            _viewModel = viewModel;
        }
    }

    public class LogViewModel : ViewModelBase
    {
        private readonly INavigator _navigator;
        private readonly IBaseService<Jakt> _huntService;
        private readonly IBaseService<Logg> _logService;
        private readonly IDialogService _dialogService;
        private readonly IFileManager _fileManager;
        private Logg _dto;

        public string ID { get; set; }
        public string HuntId { get; private set; }
        public string Detail { get; set; }
        public int Observed { get; set; }
        public int Shots { get; set; }
        public int Hits { get; set; }
        public DateTime Date { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ImagePath { get; set; }

        private MediaFile _mediaFile;

        public ImageSource ImageSource { get; set; }
        public string Notes { get; set; }

        public bool IsNew => string.IsNullOrEmpty(ID);
        public Command ItemTappedCommand { get; set; }
        public Command CancelCommand { get; set; }
        public Command SaveCommand { get; set; }
        public Command DeleteCommand { get; set; }

        public Action<MediaFile> SaveImageAction { get; set; }
        public Action DeleteImageAction { get; set; }

        public LogViewModel(INavigator navigator, 
                            IBaseService<Jakt> huntService, 
                            IBaseService<Logg> logService, 
                            IDialogService dialogService,
                            IFileManager fileManager)
        {
            _navigator = navigator;
            _huntService = huntService;
            _logService = logService;
            _dialogService = dialogService;
            _fileManager = fileManager;

            ItemTappedCommand = new Command(async () => await ShowItem());
            SaveCommand = new Command(async () => await Save());
            DeleteCommand = new Command(async () => await Delete());
            CancelCommand = new Command(async () => { await _navigator.PopAsync(); });

            CreateImageActions();
        }

        private void CreateImageActions()
        {
            SaveImageAction += (MediaFile obj) =>
            {
                _mediaFile = obj;
                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = _mediaFile.GetStreamWithImageRotatedForExternalStorage();
                    return stream;
                });
                OnPropertyChanged(nameof(ImageSource));
            };

            DeleteImageAction += () =>
            {
                _mediaFile?.Dispose();
                ImageSource = null;
                ImagePath = string.Empty;
            };
        }

        public void BeforeNavigate(Logg dto, string huntId = null)
        {
            _dto = dto ?? CreateItem(huntId);
            SetState(_dto);
        }

        private void SetState(Logg dto)
        {
            Title = IsNew ? "Ny loggføring" : dto.Dato.ToShortDateString();
            ID = dto.ID;
            HuntId = dto.JaktId;
            Detail = dto.ID;
            Observed = dto.Sett;
            Shots = dto.Skudd;
            Hits = dto.Treff;
            Date = dto.Dato;
            Latitude = Utility.MapStringToDouble(dto.Latitude); //TODO Create an extention on String class
            Longitude = Utility.MapStringToDouble(dto.Longitude);
            ImagePath = dto.ImagePath;
            ImageSource = Utility.GetImageSource(dto.ImagePath);
            Notes = dto.Notes;
        }

        private Logg CreateItem(string huntId)
        {
            if(huntId == null) { throw new ArgumentNullException("huntId", "You need to pass in the huntId to the SetState() method."); }
            return new Logg
            {
                JaktId = huntId,
                Created = DateTime.Now,
                Dato = DateTime.Now
            };
        }

        private async Task ShowItem()
        {
            await _navigator.PushAsync<LogViewModel>(
                beforeNavigate: (arg) => arg.BeforeNavigate(_dto),
                afterNavigate: async (arg) => await arg.AfterNavigate());
        }

        private async Task Delete()
        {
            var ok = await _dialogService.ShowConfirmDialog("Bekreft sletting", "Loggføringen blir permanent slettet. Er du sikker?");
            if (ok)
            {
                await _logService.Delete(ID);
                await _navigator.PopAsync();
            }
        }

        private async Task Save()
        {
            Logg dto = CreateLogDto();
            if (MediaFile != null)
            {
                dto.ImagePath = SaveImage($"log_{ID}.jpg", _fileManager);
            }

            await _logService.Save(dto);
            await _navigator.PopAsync();
        }

        protected Logg CreateLogDto()
        {

            return new Logg
            {
                ID = ID ?? Guid.NewGuid().ToString(),
                JaktId = HuntId,
                Sett = Observed,
                Skudd = Shots,
                Treff = Hits,
                Dato = Date,
                Latitude = Latitude.ToString(),
                Longitude = Longitude.ToString(),
                ImagePath = ImagePath,
                Notes = Notes
            };
        }
    }
}
