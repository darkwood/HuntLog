using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.AppModule.Logs;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.AppModule.Hunts
{
    public partial class HuntView : ContentPage
    {
        private readonly HuntViewModel _viewModel;

        public HuntView(HuntViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
            _viewModel = viewModel;
        }
    }

    public class HuntViewModel : HuntViewModelBase
    {
        public ObservableCollection<LogViewModel> Items { get; set; }

        private readonly IBaseService<Jakt> _huntService;
        private readonly IBaseService<Jeger> _hunterService;
        private readonly IBaseService<Logg> _logService;
        private readonly INavigator _navigator;
        private readonly IDialogService _dialogService;
        private readonly Func<LogViewModel> _logItemViewModelFactory;
        private Jakt _dto;

        public Command EditCommand { get; set; }
        public Command AddCommand { get; set; }
        public HuntViewModel(IBaseService<Jakt> huntService, 
                        IBaseService<Jeger> hunterService, 
                        INavigator navigator, 
                        IBaseService<Logg> logService,
                        Func<LogViewModel> logItemViewModelFactory,
                        IDialogService dialogService)
        {
            _huntService = huntService;
            _hunterService = hunterService;
            _logService = logService;
            _navigator = navigator;
            _dialogService = dialogService;
            _logItemViewModelFactory = logItemViewModelFactory;

            EditCommand = new Command(async () => await EditItem());
            AddCommand = new Command(async () => await AddItem());
        }
        private async Task EditItem()
        {
            Action<Jakt> callback = (arg) => { SetState(arg); };

            await _navigator.PushAsync<EditHuntViewModel>(
                    beforeNavigate: (arg) => arg.SetState(_dto, callback),
                    afterNavigate: async (arg) => await arg.AfterNavigate());
        }

        public void SetState(Jakt dto)
        {
            _dto = dto;
            SetStateFromDto(_dto);
        }

        public async Task AfterNavigate() 
        {
            await FetchData();
        }

        private async Task AddItem()
        {
            Action<Jakt> callback = (arg) => { };

            await _navigator.PushAsync<LogViewModel>(
                    beforeNavigate: (vm) => vm.SetState(null, ID),
                    afterNavigate: (vm) => vm.AfterNavigate());
        }

        private async Task DeleteItem(object item)
        {
            var ok = await _dialogService.ShowConfirmDialog("Bekreft sletting", "Loggføringen blir permanent slettet. Er du sikker?");
            if (ok)
            {
                await _logService.Delete((item as HuntListItemViewModel).ID);
                await FetchData();
            }
        }

        public async Task FetchData()
        {
            IsBusy = true;

            Items = new ObservableCollection<LogViewModel>();
            var items = await _logService.GetItems();

            foreach(var i in items.Where(x => x.JaktId == ID).ToList())
            {
                var vm = _logItemViewModelFactory();
                vm.SetState(i);
                Items.Add(vm);
            }

            IsBusy = false;
        }
    }
}
