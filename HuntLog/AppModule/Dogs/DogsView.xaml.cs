using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.AppModule;
using HuntLog.Factories;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;
using static HuntLog.AppModule.Dogs.DogView;

namespace HuntLog.AppModule.Dogs
{
    public partial class DogsView : ContentPage
    {
        private readonly DogsViewModel _viewModel;

        public DogsView(DogsViewModel viewModel)
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

    public class DogsViewModel : ViewModelBase
    {
        private readonly IBaseService<Dog> _dogService;
        private readonly INavigator _navigator;
        private readonly Func<DogViewModel> _dogViewModelFactory;
        private readonly IHuntFactory _huntFactory;

        public ObservableCollection<DogViewModel> Dogs { get; set; }

        public bool ListVisible => !EmptyList && !IsBusy;
        public bool EmptyList { get; set; }

        public Command AddCommand { get; set; }
        public Command DeleteItemCommand { get; set; }

        public DogsViewModel(IBaseService<Dog> dogService, INavigator navigator, Func<DogViewModel> dogViewModelFactory, IHuntFactory huntFactory)
        {
            _dogService = dogService;
            _navigator = navigator;
            _dogViewModelFactory = dogViewModelFactory;
            _huntFactory = huntFactory;

            AddCommand = new Command(async () => await AddItem());
            DeleteItemCommand = new Command(async (item) => await DeleteItem(item));

            IsBusy = true;
        }

        private async Task AddItem()
        {
            await _navigator.PushAsync<DogViewModel>(
                    beforeNavigate: (vm) => vm.SetState(null));
        }

        private async Task DeleteItem(object item)
        {
            var ok = await _huntFactory.DeleteDog((item as DogViewModel).ID);
            if (ok)
            {
                await FetchData();
            }
        }
        public async Task InitializeAsync()
        {
            await FetchData();
        }

        public async Task FetchData()
        {
            IsBusy = true;

            Dogs = new ObservableCollection<DogViewModel>();
            var hunts = await _dogService.GetItems();

            foreach (var hunt in hunts)
            {
                var vm = _dogViewModelFactory();
                vm.SetState(hunt);
                Dogs.Add(vm);
            }
            EmptyList = !Dogs.Any();
            IsBusy = false;
        }
    }
}