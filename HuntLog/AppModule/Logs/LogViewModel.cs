using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.AppModule.CustomFields;
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
    public class LogViewModel : ViewModelBase
    {
        private readonly INavigator _navigator;
        private readonly IBaseService<Logg> _logService;
        private readonly IFileManager _fileManager;
        private readonly IHuntFactory _huntFactory;
        private readonly ICustomFieldFactory _fieldFactory;
        private Jakt _huntDto;
        private Logg _dto;

        public string HuntId { get; set; }
        public string Header => $"{Date.ToString("hh:mm", new CultureInfo("nb-no"))} {Specie?.Name}";
        public string Detail => $"{_dto.Sett} Sett\n{_dto.Skudd} Skudd\n{_dto.Treff} Treff";
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public string Weather { get; set; }
        public string WeaponType { get; set; }
        public int Tags { get; set; }
        public int Weight { get; set; }
        public int ButchWeight { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public int Observed { get; set; }
        public int Shots { get; set; }
        public int Hits { get; set; }
        public List<PickerItem> HuntersPickers { get; set; }
        public List<PickerItem> DogsPickers { get; set; }
        public List<PickerItem> SpeciesPickers { get; set; }
        public List<PickerItem> ObservedPickers { get; set; }
        public List<PickerItem> ShotsPickers { get; set; }
        public List<PickerItem> HitsPickers { get; set; }
        public SpecieViewModel Specie { get; set; }
        public ObservableCollection<PickerItem> HunterAndDog { get; set; }
        public List<CustomFieldViewModel> CustomFields { get; set; }

        public Command ItemTappedCommand { get; set; }
        public Command CancelCommand { get; set; }
        public Command SaveCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command TimeCommand { get; set; }
        public Command CustomFieldsCommand { get; set; }
        public Command NoSpecieCommand { get; set; }

        public CellAction ImageAction { get; set; }
        public CellAction MapAction { get; set; }
        public Position Position { get; set; }
        public IMediaService MediaService { get; set; }
        public Position HuntPosition => new Position(Utility.MapStringToDouble(_huntDto.Latitude),
                                                     Utility.MapStringToDouble(_huntDto.Longitude));
        public LogViewModel(INavigator navigator,
                            IBaseService<Logg> logService,
                            IFileManager fileManager,
                            IHuntFactory huntFactory,
                            ICustomFieldFactory fieldFactory,
                            IMediaService mediaService)
        {
            _navigator = navigator;
            _logService = logService;
            _fileManager = fileManager;
            _huntFactory = huntFactory;
            _fieldFactory = fieldFactory;
            MediaService = mediaService;

            ItemTappedCommand = new Command(async () => await ShowItem());
            SaveCommand = new Command(async () => await Save());
            DeleteCommand = new Command(async () => await Delete());
            CancelCommand = new Command(async () => { await _navigator.PopAsync(); });

            TimeCommand = new Command(async () => { await EditDateFrom(); });
            NoSpecieCommand = new Command(async () => { await _navigator.PushAsync<SpeciesViewModel>(); });
            HunterAndDog = new ObservableCollection<PickerItem>();
            CustomFields = new List<CustomFieldViewModel>();

            CreateImageActions();
            CreatePositionActions();
        }

        private async Task EditDateFrom()
        {
            await _navigator.PushAsync<InputTimeViewModel>(
                beforeNavigate: async (arg) =>
                {
                    await arg.InitializeAsync(Date,
                        _huntDto.DatoFra,
                        _huntDto.DatoTil,
                        completeAction: (value) =>
                        {
                            Date = value;
                        });
                });
        }

        private void CreateImageActions()
        {
            ImageAction = new CellAction();
            ImageAction.Save += (object mediafile) =>
            {
                MediaFile = (MediaFile)mediafile;
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
                Position = (Position)obj;
            };

            MapAction.Delete += () =>
            {
                Position = new Position();
            };
        }

        public void BeforeNavigate(Logg dto, Jakt huntDto)
        {
            _huntDto = huntDto;
            _dto = dto ?? CreateItem();

            SetState(_dto);
            Title = IsNew ? "Ny loggføring" : dto.Dato.ToShortTimeString();
        }

        private void SetState(Logg dto)
        {
            ID = dto.ID;
            HuntId = dto.JaktId;
            Date = dto.Dato;
            Position = new Position(Utility.MapStringToDouble(dto.Latitude), Utility.MapStringToDouble(dto.Longitude));
            ImagePath = $"jaktlogg_{ID}.jpg";
            ImageSource = Utility.GetImageSource(ImagePath);
            Observed = dto.Sett;
            Shots = dto.Skudd;
            Hits = dto.Treff;
            Notes = dto.Notes;
            Weather = dto.Weather;
            WeaponType = dto.WeaponType;
            Tags = dto.Tags;
            Weight = dto.Weight;
            ButchWeight = dto.ButchWeight;
            Gender = dto.Gender;
            Age = dto.Age;
        }

        public async Task AfterNavigate()
        {
            if (IsNew)
            {
                await SetPositionAsync();
            }
            await PopulatePickers();

            CustomFields = await _fieldFactory.CreateCustomFields(_dto);

            //if (!string.IsNullOrEmpty(_dto.ArtId))
            //{
            //    Specie = await _huntFactory.CreateSpecieViewModel(_dto.ArtId);
            //}
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

            int max = 100;
            for (var i = 1; i <= max; i++)
            {
                HitsPickers.Add(new PickerItem
                {
                    ID = i.ToString(),
                    Title = i.ToString(),
                    Selected = i == _dto.Treff
                });

                ObservedPickers.Add(new PickerItem
                {
                    ID = i.ToString(),
                    Title = i.ToString(),
                    Selected = i == _dto.Sett
                });

                ShotsPickers.Add(new PickerItem
                {
                    ID = i.ToString(),
                    Title = i.ToString(),
                    Selected = i == _dto.Skudd
                });
            }
            ////Add custom value pickers too
            //HitsPickers.Add(new PickerItem
            //{
            //    ID = _dto.Treff.ToString(),
            //    Title = _dto.Treff.ToString(),
            //    Selected = _dto.Treff > max,
            //    Custom = true
            //});

            //ObservedPickers.Add(new PickerItem
            //{
            //    ID = _dto.Sett.ToString(),
            //    Title = _dto.Sett.ToString(),
            //    Selected = _dto.Sett > max,
            //    Custom = true
            //});

            //ShotsPickers.Add(new PickerItem
            //{
            //    ID = _dto.Skudd.ToString(),
            //    Title = _dto.Skudd.ToString(),
            //    Selected = _dto.Skudd > max,
            //    Custom = true
            //});

            //Add hunter and dog
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
                Position = new Position(location.Latitude, location.Longitude);
            }
        }

        private Logg CreateItem()
        {
            if (_huntDto == null) { throw new ArgumentNullException(nameof(_huntDto), "You need to pass in the huntDto to the SetState() method."); }

            return new Logg
            {
                JaktId = _huntDto.ID,
                Created = DateTime.Now,
                Dato = DateTime.Now
            };
        }

        private async Task ShowItem()
        {
            await _navigator.PushAsync<LogViewModel>(
                beforeNavigate: (arg) => arg.BeforeNavigate(_dto, _huntDto));
            //afterNavigate: async (arg) => await arg.OnAppearing());
        }

        private async Task Delete()
        {
            var ok = await _huntFactory.DeleteLog(ID);
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
                SaveImage($"jaktlogg_{dto.ID}.jpg", _fileManager);
            }

            await _logService.Save(dto);
            await _navigator.PopAsync();
        }

        protected Logg CreateLogDto()
        {
            var log = new Logg();

            log.ID = ID ?? Guid.NewGuid().ToString();
            log.JaktId = HuntId;
            log.Sett = Observed; // int.Parse(ObservedPickers.SingleOrDefault(h => h.Selected)?.ID ?? "0");
            log.Skudd = Shots; // int.Parse(ShotsPickers.SingleOrDefault(h => h.Selected)?.ID ?? "0");
            log.Treff = Hits; // int.Parse(HitsPickers.SingleOrDefault(h => h.Selected)?.ID ?? "0");
            log.Dato = Date;
            log.Latitude = Position.Latitude.ToString();
            log.Longitude = Position.Longitude.ToString();
            log.ImagePath = ImagePath;
            log.Notes = Notes;
            log.JegerId = HuntersPickers.SingleOrDefault(h => h.Selected)?.ID;
            log.DogId = DogsPickers.SingleOrDefault(h => h.Selected)?.ID;
            log.ArtId = SpeciesPickers.SingleOrDefault(h => h.Selected)?.ID;
            log.Age = Age;
            log.ButchWeight = ButchWeight;
            log.Gender = Gender;
            log.Weather = Weather;
            log.WeaponType = WeaponType;
            log.Weight = Weight;
            log.Tags = Tags;

            return log;
        }
    }
}
