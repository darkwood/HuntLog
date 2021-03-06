﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.AppModule;
using HuntLog.Factories;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;
using static HuntLog.AppModule.Hunters.HunterView;

namespace HuntLog.AppModule.Hunters
{
    public partial class HuntersView : ContentPage
    {
        private readonly HuntersViewModel _viewModel;

        public HuntersView(HuntersViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            await _viewModel.InitializeAsync();
            base.OnAppearing();
        }
    }

    public class HuntersViewModel : ViewModelBase
    {
        private readonly IBaseService<Jeger> _hunterService;
        private readonly INavigator _navigator;
        private readonly Func<HunterViewModel> _hunterViewModelFactory;
        private readonly IHuntFactory _huntFactory;

        public ObservableCollection<HunterViewModel> Hunters { get; set; }
        public bool ListVisible => !EmptyList && !IsBusy;
        public bool EmptyList { get; set; }

        public Command AddCommand { get; set; }
        public Command DeleteItemCommand { get; set; }
        public Action Callback { get; set; }

        public HuntersViewModel(IBaseService<Jeger> hunterService, INavigator navigator, Func<HunterViewModel> hunterViewModelFactory, IHuntFactory huntFactory)
        {
            _hunterService = hunterService;
            _navigator = navigator;
            _hunterViewModelFactory = hunterViewModelFactory;
            _huntFactory = huntFactory;

            AddCommand = new Command(async () => await AddItem());
            DeleteItemCommand = new Command(async (item) => await DeleteItem(item));

            IsBusy = true;
        }

        private async Task AddItem()
        {
            await _navigator.PushAsync<HunterViewModel>(
                    beforeNavigate: (vm) => vm.SetState(null));
        }

        private async Task DeleteItem(object item)
        {
            IsBusy = true;
            var ok = await _huntFactory.DeleteHunter((item as HunterViewModel).ID);
            if (ok)
            {
                await FetchData();
            }
            IsBusy = false;
        }
        public async Task InitializeAsync()
        {
            await FetchData();
        }

        public async Task FetchData()
        {
            IsBusy = true;
            EmptyList = true;

            Hunters = new ObservableCollection<HunterViewModel>();
            var hunts = await _hunterService.GetItems();

            foreach (var hunt in hunts)
            {
                var vm = _hunterViewModelFactory();
                vm.SetState(hunt);
                Hunters.Add(vm);
            }
            EmptyList = !Hunters.Any();
            Callback?.Invoke();
            IsBusy = false;
        }
    }
}