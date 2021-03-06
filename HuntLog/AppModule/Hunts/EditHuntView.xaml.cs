﻿using Xamarin.Forms;
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
using HuntLog.AppModule.Hunters;
using HuntLog.AppModule.Dogs;

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

        protected override void OnAppearing()
        {
            base.OnAppearing();
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
        public Command DateToCommand { get; set; }
        public Command HuntersCommand { get; set; }
        public Command DogsCommand { get; set; }

        public Command AddHuntersCommand { get; set; }
        public Command AddDogsCommand { get; set; }

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
            DateToCommand = new Command(async () => await EditDateTo());
            AddHuntersCommand = new Command(async () => {
                await _navigator.PushAsync<HuntersViewModel>(beforeNavigate: (arg) => {
                    arg.Callback = () =>
                    {
                        GetHunters();
                    };
                });
            });
            AddDogsCommand = new Command(async () => {
                await _navigator.PushAsync<DogsViewModel>(beforeNavigate: (arg) => {
                    arg.Callback = () =>
                    {
                        GetDogs();
                    };
                });
            });

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
                        if(DateFrom > DateTo)
                        {
                            DateTo = DateFrom;
                        }
                    });
                });
        }

        private async Task EditDateTo()
        {
            await _navigator.PushAsync<InputDateViewModel>(
                beforeNavigate: async (arg) =>
                {
                    await arg.InitializeAsync(DateTo,
                    completeAction: (value) =>
                    {
                        DateTo = value;
                        if (DateTo < DateFrom)
                        {
                            DateFrom = DateTo;
                        }
                    });
                });
        }

        public async Task InitializeAsync()
        {
            IsBusy = true;
            await GetHunters();
            await GetDogs();

            if (IsNew)
            {
                await SetPositionAsync();
            }
            IsBusy = false;
        }

        private async Task GetDogs()
        {
            Dogs = await _huntFactory.CreateDogPickerItems(_dto.DogIds);
        }

        private async Task GetHunters()
        {
            Hunters = await _huntFactory.CreateHunterPickerItems(_dto.JegerIds);
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
            IsBusy = true;
            _callback = callback;
            InitializeAsync();
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
            var ok = await _huntFactory.DeleteHunt(ID);
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
                SaveImage(dto.ImagePath, _fileManager);
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
                Sted = string.IsNullOrWhiteSpace(Location) ? $"Jakt {DateFrom.ToShortDateString()}" : Location,
                DatoFra = DateFrom,
                DatoTil = DateTo,
                JegerIds = Hunters.Where(x => x.Selected).Select(h => h.ID).ToList<string>(),
                DogIds = Dogs.Where(x => x.Selected).Select(h => h.ID).ToList<string>(),
                Latitude = Position.Latitude.ToString(),
                Longitude = Position.Longitude.ToString(),
                Notes = Notes
            };
        }
    }
}
