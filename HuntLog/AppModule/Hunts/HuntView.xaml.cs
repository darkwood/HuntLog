using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.AppModule.Logs;
using HuntLog.AppModule.Stats;
using HuntLog.AppModule.Stats.Pages;
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
            base.OnAppearing();
            await _viewModel.FetchData();
        }
    }

    public class LogGroup : ObservableCollection<LogListItemViewModel>
    {
        public String Name { get; private set; }
        public String ShortName { get; private set; }

        public LogGroup(String Name, String ShortName)
        {
            this.Name = Name;
            this.ShortName = ShortName;
        }
    }

    public class HuntViewModel : HuntViewModelBase
    {
        public ObservableCollection<LogGroup> LogListItemViewModels { get; set; }
        public ObservableCollection<LogListItemViewModel> Items { get; set; }
        private LogListItemViewModel selectedItem;
        public LogListItemViewModel SelectedItem
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
        private readonly Func<LogListItemViewModel> _logListItemViewModelFactory;
        private readonly IHuntFactory _huntFactory;

        public string MapIcon => FontAwesomeIcons.MapMarkedAlt;

        public Command EditCommand { get; set; }
        public Command MapCommand { get; set; }
        public Command AddCommand { get; set; }
        public Command RefreshCommand { get; set; }

        public bool ShowMapButton => Items.Any();

        public HuntViewModel(IBaseService<Jeger> hunterService,
                        INavigator navigator,
                        IBaseService<Logg> logService,
                        Func<LogListItemViewModel> logItemViewModelFactory,
                        IHuntFactory huntFactory)
        {
            _hunterService = hunterService;
            _logService = logService;
            _navigator = navigator;
            _logListItemViewModelFactory = logItemViewModelFactory;
            _huntFactory = huntFactory;


            EditCommand = new Command(async () => await EditItem());
            AddCommand = new Command(async () => await AddItem());
            MapCommand = new Command(async () => await ShowMap());
            RefreshCommand = new Command(async () => await FetchData());

            LogListItemViewModels = new ObservableCollection<LogGroup>();
            Items = new ObservableCollection<LogListItemViewModel>();
        }

        private async Task ShowMap()
        {
            await _navigator.PushAsync<StatsMapViewModel>(
                    beforeNavigate: (arg) => arg.BeforeNavigate(ID));
        }

        private async Task EditItem()
        {
            Action<Jakt> callback = (arg) => { SetState(arg); };

            await _navigator.PushAsync<EditHuntViewModel>(
                    beforeNavigate: (arg) => arg.SetState(_dto, callback));
        }

        public void SetState(Jakt dto)
        {
            _dto = dto;
            SetStateFromDto(_dto);
        }

        private async Task AddItem()
        {
            await _navigator.PushAsync<LogViewModel>(
                    (vm) => vm.BeforeNavigate(null, _dto),
                    (vm) => vm.AfterNavigate());
        }

        private async Task DeleteItem(object item)
        {
            var ok = await _huntFactory.DeleteLog(ID);
            if (ok)
            {
                await FetchData();
            }
        }

        public async Task FetchData()
        {
            IsBusy = true;

            Items = new ObservableCollection<LogListItemViewModel>();
            var items = new List<LogListItemViewModel>();
            var dtos = await _logService.GetItems();

            foreach (var i in dtos.Where(x => x.JaktId == ID).ToList())
            {
                var vm = _logListItemViewModelFactory();
                vm.BeforeNavigate(i);
                await vm.OnAppearing();
                items.Add(vm);
            }
            items = items.OrderByDescending(o => o.Date).ToList();
            var groups = items.GroupBy(g => g.Date.ToShortDateString()).OrderByDescending(o => o.Key);
            LogListItemViewModels.Clear();
            foreach (var g in groups)
            {
                var jg = new LogGroup(g.Key, "");
                foreach (var item in g.OrderByDescending(o => o.Date))
                {
                    jg.Add(items.Single(h => h.ID == item.ID));
                }
                LogListItemViewModels.Add(jg);
            }

            //items.ForEach(i => Items.Add(i));

            var hunters = await _huntFactory.CreateHunterPickerItems(_dto.JegerIds);
            var dogs = await _huntFactory.CreateDogPickerItems(_dto.DogIds);

            SelectedHuntersAndDogs = hunters.Where(h => h.Selected).ToList();
            SelectedHuntersAndDogs.AddRange(dogs.Where(h => h.Selected).ToList());

            IsBusy = false;
        }

    }
}
