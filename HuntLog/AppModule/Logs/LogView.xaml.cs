
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HuntLog.AppModule.Hunts;
using HuntLog.Helpers;
using HuntLog.Models;
using HuntLog.Services;
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
        private Logg _dto;

        public string ID { get; set; }
        public string Detail { get; set; }
        public int Observed { get; set; }
        public int Shots { get; set; }
        public int Hits { get; set; }
        public DateTime Date { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ImagePath { get; set; }
        public ImageSource ImageSource { get; set; }
        public string Notes { get; set; }

        public bool IsNew => string.IsNullOrEmpty(ID);
        public Command ItemTappedCommand { get; set; }
        public Command CancelCommand { get; set; }
        public Command SaveCommand { get; set; }
        public Command DeleteCommand { get; set; }

        public LogViewModel(INavigator navigator, IBaseService<Jakt> huntService, IBaseService<Logg> logService, IDialogService dialogService)
        {
            _navigator = navigator;
            _huntService = huntService;
            _logService = logService;
            _dialogService = dialogService;

            ItemTappedCommand = new Command(async () => await ShowItem());
            SaveCommand = new Command(async () => await Save());
            DeleteCommand = new Command(async () => await Delete());
            CancelCommand = new Command(async () => {
                await _navigator.PopAsync();
            });
        }

        public void SetState(Logg dto, string huntId = null)
        {
            _dto = dto ?? CreateItem(huntId);

            Title = IsNew ? "Ny loggføring" : _dto.Dato.ToShortDateString();
            ID = _dto.ID;
            Detail = _dto.ID;
            Observed = _dto.Sett;
            Shots = _dto.Skudd;
            Hits = _dto.Treff;
            Date = _dto.Dato;
            Latitude = Utility.MapStringToDouble(_dto.Latitude); //TODO Create an extention on String class
            Longitude = Utility.MapStringToDouble(_dto.Longitude);
            ImagePath = _dto.ImagePath;
            ImageSource = Utility.GetImageSource(_dto.ImagePath);
            Notes = _dto.Notes;
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
                beforeNavigate: (arg) => arg.SetState(_dto),
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
            //if (_mediaFile != null)
            //{
            //    dto.ImagePath = SaveImage($"hunt_{DateTime.Now}.jpg");
            //}

            await _logService.Save(dto);
            //_callback.Invoke(dto);
            await _navigator.PopAsync();
        }

        protected Logg CreateLogDto()
        {

            return new Logg
            {
                ID = ID ?? Guid.NewGuid().ToString(),
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
