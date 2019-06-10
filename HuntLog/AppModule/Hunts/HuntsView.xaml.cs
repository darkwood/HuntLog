using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Factories;
using HuntLog.Helpers;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;


namespace HuntLog.AppModule.Hunts
{
    public partial class HuntsView : ContentPage
    {
        private readonly HuntsViewModel _viewModel;

        public HuntsView(HuntsViewModel viewModel)
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

    public class HuntGroup : ObservableCollection<HuntListItemViewModel>
    {
        public String Name { get; private set; }
        public String ShortName { get; private set; }

        public HuntGroup(String Name, String ShortName)
        {
            this.Name = Name;
            this.ShortName = ShortName;
        }
    }

    public class HuntsViewModel : ViewModelBase
    {
        private readonly IBaseService<Jakt> _huntService;
        private readonly Func<HuntListItemViewModel> _huntListItemViewModelFactory;
        private readonly INavigator _navigator;
        private readonly IHuntFactory _huntFactory;
        private readonly IDialogService _dialogService;

        public ObservableCollection<HuntGroup> HuntListItemViewModels { get; set; }
        public bool ListVisible => !EmptyList && !IsBusy;
        public bool EmptyList { get; set; }

        public Command AddCommand { get; set; }
        public Command DeleteItemCommand { get; set; }
        public Command RefreshCommand { get; set; }

        public HuntsViewModel(IBaseService<Jakt> huntService, 
                            Func<HuntListItemViewModel> huntListItemViewModelFactory, 
                            INavigator navigator, 
                            IHuntFactory huntFactory,
                            IDialogService dialogService)
        {
            _huntService = huntService;
            _huntListItemViewModelFactory = huntListItemViewModelFactory;
            _navigator = navigator;
            _huntFactory = huntFactory;
            _dialogService = dialogService;
            HuntListItemViewModels = new ObservableCollection<HuntGroup>();
            AddCommand = new Command(async () => await AddItem());
            DeleteItemCommand = new Command(async (args) => await DeleteItem(args));
            RefreshCommand = new Command(async () => await FetchData(true));
        }

        private async Task AddItem()
        {
            Action<Jakt> callback = (arg) => { };

            await _navigator.PushAsync<EditHuntViewModel>(
                    beforeNavigate: (vm) => vm.SetState(null, callback));
        }

        private async Task DeleteItem(object item)
        {
            var hunt = (item as HuntListItemViewModel);

            var ok = await _huntFactory.DeleteHunt(hunt.ID);
            if (ok)
            {
                await FetchData();
            }
        }
        public async Task InitializeAsync()
        {
            await FetchData();
        }

        public async Task FetchData(bool forceRefresh = false)
        {
            IsBusy = true;

            var hunts = await _huntService.GetItems(forceRefresh);

            var huntListViewModels = hunts.Select(hunt =>
            {
                var item = _huntListItemViewModelFactory();
                item.Initialize(hunt);
                return item;
            });

            var groups = huntListViewModels.GroupBy(g => g.DateFrom.Year).OrderByDescending(o => o.Key);
            HuntListItemViewModels.Clear();
            foreach (var g in groups)
            {
                var jg = new HuntGroup(g.Key.ToString(), "");
                foreach (var hunt in g.OrderByDescending(o => o.DateFrom))
                {
                    jg.Add(huntListViewModels.Single(h => h.ID == hunt.ID));
                }
                HuntListItemViewModels.Add(jg);
            }

            EmptyList = !huntListViewModels.Any();

            IsBusy = false;
        }
    }

    public class HuntListItemViewModel : ViewModelBase
    {
        private readonly INavigator _navigator;
        private readonly IBaseService<Jakt> _huntService;

        protected Jakt _dto { get; set; }

        public Command ItemTappedCommand { get; set; }

        public string Detail { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public HuntListItemViewModel(INavigator navigator, IBaseService<Jakt> huntService)
        {
            _navigator = navigator;
            _huntService = huntService;
            ItemTappedCommand = new Command(async () => await ShowHunt());
        }

        public void Initialize(Jakt dto)
        {
            _dto = dto;
            Title = _dto.Sted;
            ID = _dto.ID;
            ImagePath =_dto.ImagePath;
            ImageSource = Utility.GetImageSource(ImagePath);
            Detail = _dto.DatoFra.ToString("dd. MMM", new CultureInfo("nb-NO"));
            DateFrom = _dto.DatoFra;
            DateTo = _dto.DatoTil;
        }

        private async Task ShowHunt()
        {
            IsBusy = true;
            await _navigator.PushAsync<HuntViewModel>(
                beforeNavigate: (arg) => arg.SetState(_dto),
                afterNavigate: async (arg) => await arg.OnAppearing());
            IsBusy = false;
        }
    }
}
