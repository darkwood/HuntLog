using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.AppModule.Logs;
using HuntLog.Helpers;
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

        protected override async void OnAppearing()
        {
            await _viewModel.InitializeAsync();
            base.OnAppearing();
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

        public override async Task AfterNavigate()
        {
            await FetchData();
        }

        private async Task AddItem()
        {
            await _navigator.PushAsync<LogViewModel>(
                    (vm) => vm.BeforeNavigate(null, ID),
                    (vm) => vm.AfterNavigate());
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

            foreach (var i in items.Where(x => x.JaktId == ID).ToList())
            {
                var vm = _logItemViewModelFactory();
                vm.BeforeNavigate(i, null);
                Items.Add(vm);
            }

            SelectedHunters = new List<InputViews.InputPickerItemViewModel>();
            var hunters = await _hunterService.GetItems();
            foreach(var h in hunters.Where(x => HunterIds.Any(xx => xx == x.ID)))
            {
                SelectedHunters.Add(new InputViews.InputPickerItemViewModel
                {
                    ID = h.ID,
                    Title = h.Firstname,
                    ImageSource = Utility.GetImageSource(h.ImagePath)
                });
            }

            IsBusy = false;
        }

        public async Task InitializeAsync()
        {
            await FetchData();
        }
    }
}
