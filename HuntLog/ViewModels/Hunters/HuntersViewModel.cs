using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.ViewModels.Hunters
{
    public class HuntersViewModel : ViewModelBase
    {
        private readonly IHunterService _hunterService;
        private readonly INavigator _navigator;
        private readonly Func<HunterViewModel> _hunterViewModelFactory;
        private readonly IDialogService _dialogService;

        public ObservableCollection<HunterViewModel> Hunters { get; set; }

        public Command AddCommand { get; set; }
        public Command DeleteItemCommand { get; set; }

        public HuntersViewModel(IHunterService hunterService, INavigator navigator, Func<HunterViewModel> hunterViewModelFactory, IDialogService dialogService)
        {
            _hunterService = hunterService;
            _navigator = navigator;
            _hunterViewModelFactory = hunterViewModelFactory;
            _dialogService = dialogService;

            AddCommand = new Command(async () => await AddItem());
            DeleteItemCommand = new Command(async (item) => await DeleteItem(item));
        }

        private async Task AddItem() { await Task.CompletedTask; }

        private async Task DeleteItem(object item) { await Task.CompletedTask; }

        public async Task InitializeAsync()
        {
            await FetchData();
        }

        public async Task FetchData()
        {
            IsBusy = true;

            Hunters = new ObservableCollection<HunterViewModel>();
            var hunts = await _hunterService.GetItems();

            foreach(var hunt in hunts) 
            {
                var vm = _hunterViewModelFactory();
                await vm.SetState(hunt);
                Hunters.Add(vm);
            }

            IsBusy = false;
        }
    }
}