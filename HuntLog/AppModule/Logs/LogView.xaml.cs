
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.AppModule.Hunts;
using HuntLog.AppModule.Species;
using HuntLog.Cells;
using HuntLog.Factories;
using HuntLog.Helpers;
using HuntLog.InputViews;
using HuntLog.Models;
using HuntLog.Services;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

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
        private readonly IBaseService<Logg> _logService;
        private readonly IFileManager _fileManager;
        private readonly IHuntFactory _huntFactory;
        private Logg _dto;

        public string HuntId { get; private set; }
        public string Header => $"{Date.ToString("hh:mm", new CultureInfo("nb-no"))} {Specie?.Name}";
        public string Detail => $"{_dto.Sett} Sett\n{_dto.Skudd} Skudd\n{_dto.Treff} Treff";
        public DateTime Date { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Notes { get; set; }
        public List<PickerItem> HuntersPickers { get; set; }
        public List<PickerItem> DogsPickers { get; set; }
        public List<PickerItem> SpeciesPickers { get; set; }
        public List<PickerItem> ObservedPickers { get; set; }
        public List<PickerItem> ShotsPickers { get; set; }
        public List<PickerItem> HitsPickers { get; set; }
        public SpecieViewModel Specie { get; set; }
        public ObservableCollection<PickerItem> HunterAndDog { get; set; }

        public Command ItemTappedCommand { get; set; }
        public Command CancelCommand { get; set; }
        public Command SaveCommand { get; set; }
        public Command DeleteCommand { get; set; }

        public CellAction ImageAction { get; set; }
        public CellAction MapAction { get; set; }

        public LogViewModel(INavigator navigator, 
                            IBaseService<Logg> logService,
                            IFileManager fileManager,
                            IHuntFactory huntFactory)
        {
            _navigator = navigator;
            _logService = logService;   
            _fileManager = fileManager;
            _huntFactory = huntFactory;

            ItemTappedCommand = new Command(async () => await ShowItem());
            SaveCommand = new Command(async () => await Save());
            DeleteCommand = new Command(async () => await Delete());
            CancelCommand = new Command(async () => { await _navigator.PopAsync(); });
            HunterAndDog = new ObservableCollection<PickerItem>();

            CreateImageActions();
            CreatePositionActions();
        }

        private void CreateImageActions()
        {
            ImageAction = new CellAction();
            ImageAction.Save += (object obj) =>
            {
                MediaFile = (MediaFile)obj;
                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = MediaFile.GetStreamWithImageRotatedForExternalStorage();
                    return stream;
                });
                OnPropertyChanged(nameof(ImageSource));
            };

            ImageAction.Delete += () =>
            {
                MediaFile?.Dispose();
                ImageSource = null;
                ImagePath = string.Empty;
            };
        }

        private void CreatePositionActions()
        {
            MapAction = new CellAction();
            MapAction.Save += (object obj) =>
            {
                var pos = (Position)obj;
                Latitude = pos.Latitude;
                Longitude = pos.Longitude;
            };

            MapAction.Delete += () =>
            {
                Latitude = Longitude = 0;
            };
        }

        public void BeforeNavigate(Logg dto, string huntId = null)
        {
            _dto = dto ?? CreateItem(huntId);
            SetState(_dto);
            Title = IsNew ? "Ny loggføring" : dto.Dato.ToShortTimeString();
        }

        private void SetState(Logg dto)
        {
            ID = dto.ID;
            HuntId = dto.JaktId;
            Date = dto.Dato;
            Latitude = Utility.MapStringToDouble(dto.Latitude); //TODO Create an extention on String class
            Longitude = Utility.MapStringToDouble(dto.Longitude);
            ImagePath = dto.ImagePath;
            ImageSource = Utility.GetImageSource(dto.ImagePath);
            Notes = dto.Notes;
        }

        public override async Task AfterNavigate()
        {
            if (IsNew)
            {
#pragma warning disable CS4014
                SetPositionAsync();
#pragma warning restore CS4014
            }
            await PopulatePickers();
            if(!string.IsNullOrEmpty(_dto.ArtId))
            {
                Specie = await _huntFactory.CreateSpecieViewModel(_dto.ArtId);
            }
        }

        private async Task PopulatePickers()
        {
            var jegerIds = new List<string> { _dto.JegerId };
            var dogIds = new List<string> { _dto.DogId };
            HuntersPickers = await _huntFactory.CreateHunterPickerItems(jegerIds, _dto.JaktId);
            DogsPickers = await _huntFactory.CreateDogPickerItems(dogIds, _dto.JaktId);
            SpeciesPickers = await _huntFactory.CreateSpeciePickerItems(_dto.ArtId);

            HitsPickers = new List<PickerItem>();
            ShotsPickers = new List<PickerItem>();
            ObservedPickers = new List<PickerItem>();

            for (var i = 0; i <= 5; i++)
            {
                HitsPickers.Add(new PickerItem 
                { 
                    ID = i.ToString(), 
                    Title = i.ToString(),
                    IsNumericPicker = true,
                    Selected = i == _dto.Treff
                });

                ObservedPickers.Add(new PickerItem
                {
                    ID = i.ToString(),
                    Title = i.ToString(),
                    IsNumericPicker = true,
                    Selected = i == _dto.Sett
                });

                ShotsPickers.Add(new PickerItem
                {
                    ID = i.ToString(),
                    Title = i.ToString(),
                    IsNumericPicker = true,
                    Selected = i == _dto.Skudd
                });
            }

            if (!string.IsNullOrEmpty(_dto.JegerId))
            {
                HunterAndDog.Add(HuntersPickers.SingleOrDefault(h => h.Selected));
            }
            if (!string.IsNullOrEmpty(_dto.DogId))
            {
                HunterAndDog.Add(DogsPickers.SingleOrDefault(h => h.Selected));
            }
        }

        private async Task SetPositionAsync()
        {
            var location = await PositionHelper.GetLocationAsync();
            if (location != null)
            {
                Latitude = location.Latitude;
                Longitude = location.Longitude;
            }
        }

        private Logg CreateItem(string huntId)
        {
            if(huntId == null) { throw new ArgumentNullException(nameof(huntId), "You need to pass in the huntId to the SetState() method."); }
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
            var ok = await _huntFactory.DeleteLog(ID, ImagePath);
            if (ok)
            {
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
                Sett = int.Parse(ObservedPickers.SingleOrDefault(h => h.Selected)?.ID),
                Skudd = int.Parse(ShotsPickers.SingleOrDefault(h => h.Selected)?.ID),
                Treff = int.Parse(HitsPickers.SingleOrDefault(h => h.Selected)?.ID),
                Dato = Date,
                Latitude = Latitude.ToString(),
                Longitude = Longitude.ToString(),
                ImagePath = ImagePath,
                Notes = Notes,
                JegerId = HuntersPickers.SingleOrDefault(h => h.Selected)?.ID,
                DogId = DogsPickers.SingleOrDefault(h => h.Selected)?.ID,
                ArtId = SpeciesPickers.SingleOrDefault(h => h.Selected)?.ID
            };
        }
    }
}
