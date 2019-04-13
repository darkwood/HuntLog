using Xamarin.Forms;
using System;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.InputViews;
using HuntLog.Models;
using HuntLog.Services;
using Plugin.Media.Abstractions;
using Xamarin.Forms.Maps;
using HuntLog.Helpers;
using HuntLog.Factories;
using HuntLog.Cells;
using System.Collections.Generic;

namespace HuntLog.AppModule.Hunts
{
    public partial class EditHuntView : ContentPage
    {
        private readonly EditHuntViewModel _viewModel;

        public EditHuntView(EditHuntViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
            _viewModel = viewModel;
        }
    }

    public class EditHuntViewModel : HuntViewModelBase
    {
        private readonly IBaseService<Jakt> _huntService;
        private readonly IBaseService<Jeger> _hunterService;
        private readonly INavigator _navigator;
        private readonly IDialogService _dialogService;
        private readonly IFileManager _fileManager;
        private readonly IHuntFactory _huntFactory;
        private Action<Jakt> _callback;

        public Command CancelCommand { get; set; }
        public Command SaveCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command DateFromCommand { get; set; }
        public Command HuntersCommand { get; set; }
        public Command DogsCommand { get; set; }

        public CellAction ImageAction { get; set; }
        public CellAction MapAction { get; set; }

        public string PositionStatus { get; set; }
        public List<PickerItem> Hunters { get; set; }
        public List<PickerItem> Dogs { get; set; }

        public EditHuntViewModel(IBaseService<Jakt> huntService,
                                IBaseService<Jeger> hunterService,
                                INavigator navigator,
                                IDialogService dialogService,
                                IFileManager fileManager,
                                IHuntFactory hunterFactory)
        {
            _huntService = huntService;
            _hunterService = hunterService;
            _navigator = navigator;
            _dialogService = dialogService;
            _fileManager = fileManager;
            _huntFactory = hunterFactory;

            SaveCommand = new Command(async () => await Save());
            DeleteCommand = new Command(async () => await Delete());
            CancelCommand = new Command(async () => { await _navigator.PopAsync(); });

            DateFromCommand = new Command(async () => await EditDateFrom());

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

        //private async Task EditHunters()
        //{
        //    await _navigator.PushAsync<InputPickerViewModel>(
        //        beforeNavigate: async (arg) =>
        //        {
        //            var hunterPickers = await _huntFactory.CreateHunterPickerItems(_dto.JegerIds);
        //            await arg.InitializeAsync(hunterPickers,
        //                completeAction: async (value) =>
        //                {
        //                    HunterIds = value.Where(x => x.Selected).Select(v => v.ID).ToList();
        //                    await SetHunterNames();
        //                });
        //        });
        //}
        private async Task EditDateFrom()
        {
            await _navigator.PushAsync<InputDateViewModel>(
                beforeNavigate: async (arg) =>
                {
                    await arg.InitializeAsync(DateFrom,
                    completeAction: (value) =>
                    {
                        DateFrom = value;
                    });
                });
        }

        public override async Task AfterNavigate()
        {
            if (IsNew)
            {
#pragma warning disable CS4014
                SetPositionAsync();
#pragma warning restore CS4014
            }

            Hunters = await _huntFactory.CreateHunterPickerItems(_dto.JegerIds);
            Dogs = await _huntFactory.CreateDogPickerItems(_dto.DogIds);

            WriteAllImagesToConsole();
        }

        private void WriteAllImagesToConsole()
        {
            var files = _fileManager.GetAllFiles().Where(f => f.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase));
            Console.WriteLine("----Photos: " + files.Count() + "----");
            foreach (var file in files)
            {
                Console.WriteLine(file);
            }
        }

        private async Task SetPositionAsync()
        {
            Location = "Henter din posisjon...";

            var location = await PositionHelper.GetLocationAsync();
            if (location != null)
            {
                Latitude = location.Latitude;
                Longitude = location.Longitude;
                Location = await PositionHelper.GetLocationNameForPosition(location.Latitude, location.Longitude);
            }
            else
            {
                Location = string.Empty;
            }
        }

        public void SetState(Jakt hunt, Action<Jakt> callback)
        {
            SetStateFromDto(hunt ?? CreateNewHunt());
            Title = IsNew ? "Ny jakt" : "Rediger jakt";
            _callback = callback;
        }

        private Jakt CreateNewHunt()
        {
            return new Jakt
            {
                Created = DateTime.Now,
                DatoFra = DateTime.Now,
                DatoTil = DateTime.Now,
            };
        }

        private async Task Delete()
        {
            var ok = await _huntFactory.DeleteHunt(ID, ImagePath);
            if (ok) 
            {
                await _navigator.PopToRootAsync();
            }
        }

        private async Task Save()
        {
            Jakt dto = CreateHuntDto();
            if (MediaFile != null)
            {
                dto.ImagePath = SaveImage($"hunt_{ID}.jpg", _fileManager);
            }

            await _huntService.Save(dto);
            _callback.Invoke(dto);
            await _navigator.PopAsync();
        }

        protected Jakt CreateHuntDto()
        {
            return new Jakt
            {
                ID = ID ?? Guid.NewGuid().ToString(),
                Sted = Location,
                DatoFra = DateFrom,
                DatoTil = DateTo,
                JegerIds = Hunters.Where(x => x.Selected).Select(h => h.ID).ToList<string>(),
                DogIds = Dogs.Where(x => x.Selected).Select(h => h.ID).ToList<string>(),
                Latitude = Latitude.ToString(),
                Longitude = Longitude.ToString(),
                ImagePath = ImagePath,
                Notes = Notes
            };
        }
    }
}
