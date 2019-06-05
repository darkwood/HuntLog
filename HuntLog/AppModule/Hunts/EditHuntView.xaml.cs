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
                Position = (Position)obj;
            };

            MapAction.Delete += () =>
            {
                Position = new Position();
            };
        }

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
                await SetPositionAsync();
            }

            Hunters = await _huntFactory.CreateHunterPickerItems(_dto.JegerIds);
            Dogs = await _huntFactory.CreateDogPickerItems(_dto.DogIds);

        }

        private async Task SetPositionAsync()
        {
            Location = "Henter din posisjon...";

            var location = await PositionHelper.GetLocationAsync();
            if (location != null)
            {
                Position = new Position(location.Latitude, location.Longitude);
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
                SaveImage($"jakt_{ID}.jpg", _fileManager);
            }

            await _huntService.Save(dto);
            _callback.Invoke(dto);
            await _navigator.PopAsync();
        }

        protected Jakt CreateHuntDto()
        {
            return new Jakt
            {
                ID = string.IsNullOrEmpty(ID) ? Guid.NewGuid().ToString() : ID,
                Sted = Location,
                DatoFra = DateFrom,
                DatoTil = DateTo,
                JegerIds = Hunters.Where(x => x.Selected).Select(h => h.ID).ToList<string>(),
                DogIds = Dogs.Where(x => x.Selected).Select(h => h.ID).ToList<string>(),
                Latitude = Position.Latitude.ToString(),
                Longitude = Position.Longitude.ToString(),
                ImagePath = ImagePath,
                Notes = Notes
            };
        }
    }
}
