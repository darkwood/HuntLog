using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HuntLog.AppModule;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.AppModule.Hunters
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

        private async Task AddItem() 
        {
            await _navigator.PushAsync<HunterViewModel>(
                    beforeNavigate: (vm) => vm.SetState(null));
        }

        private async Task DeleteItem(object item)
        {
            var ok = await _dialogService.ShowConfirmDialog("Bekreft sletting", "Jeger blir permanent slettet. Er du sikker?");
            if (ok)
            {
                await _hunterService.Delete((item as HunterViewModel).ID);
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

            Hunters = new ObservableCollection<HunterViewModel>();
            var hunts = await _hunterService.GetItems();

            foreach(var hunt in hunts) 
            {
                var vm = _hunterViewModelFactory();
                vm.SetState(hunt);
                Hunters.Add(vm);
            }

            IsBusy = false;
        }
    }
}