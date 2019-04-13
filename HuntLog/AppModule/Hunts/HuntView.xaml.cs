using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.AppModule.Logs;
using HuntLog.Factories;
using HuntLog.Helpers;
using HuntLog.InputViews;
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
        public LogViewModel SelectedItem
        {
            get
            {
                return selectedItem;
            }

            set
            {
                selectedItem = value;
                selectedItem.ItemTappedCommand.Execute(null);
            }
        }
        private readonly IBaseService<Jeger> _hunterService;
        private readonly IBaseService<Logg> _logService;
        private readonly INavigator _navigator;
        private readonly Func<LogViewModel> _logItemViewModelFactory;
        private readonly IHuntFactory _huntFactory;
        private LogViewModel selectedItem;

        public Command EditCommand { get; set; }
        public Command AddCommand { get; set; }

        public HuntViewModel(IBaseService<Jeger> hunterService,
                        INavigator navigator,
                        IBaseService<Logg> logService,
                        Func<LogViewModel> logItemViewModelFactory,
                        IHuntFactory huntFactory)
        {
            _hunterService = hunterService;
            _logService = logService;
            _navigator = navigator;
            _logItemViewModelFactory = logItemViewModelFactory;
            _huntFactory = huntFactory;

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
            var ok = await _huntFactory.DeleteLog(ID, ImagePath);
            if (ok)
            {
                await FetchData();
            }
        }

        public async Task FetchData()
        {
            IsBusy = true;

            Items = new ObservableCollection<LogViewModel>();
            var items = new List<LogViewModel>();
            var dtos = await _logService.GetItems();

            foreach (var i in dtos.Where(x => x.JaktId == ID).ToList())
            {
                var vm = _logItemViewModelFactory();
                vm.BeforeNavigate(i, null);
                await vm.AfterNavigate();
                items.Add(vm);
            }
            items = items.OrderByDescending(o => o.Date).ToList();
            items.ForEach(i => Items.Add(i));

            var hunters = await _huntFactory.CreateHunterPickerItems(_dto.JegerIds);
            var dogs = await _huntFactory.CreateDogPickerItems(_dto.DogIds);

            SelectedHuntersAndDogs = hunters.Where(h => h.Selected).ToList();
            SelectedHuntersAndDogs.AddRange(dogs.Where(h => h.Selected).ToList());

            IsBusy = false;
        }

        public async Task InitializeAsync()
        {
            await FetchData();
        }
    }
}
