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
        private readonly IHunterFactory _hunterFactory;
        private Action<Jakt> _callback;

        public Command CancelCommand { get; set; }
        public Command SaveCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command PositionCommand { get; set; }
        public Command DateFromCommand { get; set; }
        public Command HuntersCommand { get; set; }
        public Command DogsCommand { get; set; }

        public Action<MediaFile> SaveImageAction { get; set; }
        public Action DeleteImageAction { get; set; }

        public string PositionStatus { get; set; }

        public EditHuntViewModel(IBaseService<Jakt> huntService,
                                IBaseService<Jeger> hunterService,
                                INavigator navigator,
                                IDialogService dialogService,
                                IFileManager fileManager,
                                IHunterFactory hunterFactory)
        {
            _huntService = huntService;
            _hunterService = hunterService;
            _navigator = navigator;
            _dialogService = dialogService;
            _fileManager = fileManager;
            _hunterFactory = hunterFactory;
            SaveCommand = new Command(async () => await Save());
            DeleteCommand = new Command(async () => await Delete());
            CancelCommand = new Command(async () =>
            {
                await _navigator.PopAsync();
            });

            PositionCommand = new Command(async () => await EditPosition());
            DateFromCommand = new Command(async () => await EditDateFrom());
            HuntersCommand = new Command(async () => await EditHunters());
            DogsCommand = new Command(async () => await EditDogs());

            CreateImageActions();
        }

        private void CreateImageActions()
        {
            SaveImageAction += (MediaFile obj) =>
            {
                MediaFile = obj;
                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = MediaFile.GetStreamWithImageRotatedForExternalStorage();
                    return stream;
                });
                OnPropertyChanged(nameof(ImageSource));
            };

            DeleteImageAction += () =>
            {
                MediaFile?.Dispose();
                ImageSource = null;
                ImagePath = string.Empty;
            };
        }

        private async Task EditDogs() { await Task.CompletedTask; }

        private async Task EditHunters()
        {
            await _navigator.PushAsync<InputPickerViewModel>(
                beforeNavigate: async (arg) =>
                {
                    var hunterPickers = await _hunterFactory.CreateHunterPickerItems(HunterIds);
                    await arg.InitializeAsync(hunterPickers,
                        completeAction: async (value) =>
                        {
                            HunterIds = value.Where(x => x.Selected).Select(v => v.ID).ToList();
                            await SetHunterNames();
                        });
                });
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
        private async Task EditPosition()
        {
            await _navigator.PushAsync<InputPositionViewModel>(
                beforeNavigate: async (arg) =>
                {
                    await arg.InitializeAsync(new Position(Latitude, Longitude),
                    completeAction: (position) =>
                    {
                        Latitude = position.Latitude;
                        Longitude = position.Longitude;
                    },
                    deleteAction: () =>
                    {
                        Latitude = 0;
                        Longitude = 0;
                    });
                });
        }

        public override async Task AfterNavigate()
        {
            await SetHunterNames();

            if (IsNew)
            {
                await SetPositionAsync();
            }

            foreach (var file in _fileManager.GetAllFiles().Where(f => f.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase)))
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

        private async Task SetHunterNames()
        {
            var hunters = await _hunterService.GetItems();
            var myHunters = hunters.Where(h => hunters.Any(hh => hh.ID == h.ID));
            HuntersNames = string.Join(", ", myHunters.Select(h => h.Firstname));
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
            var ok = await _dialogService.ShowConfirmDialog("Bekreft sletting", "Jakta blir permanent slettet. Er du sikker?");
            if (ok)
            {
                await _huntService.Delete(ID);
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
    }
}
