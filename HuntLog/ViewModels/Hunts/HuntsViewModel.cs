using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.ViewModels.Hunts
{
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
        private readonly IHuntService _huntService;
        private readonly Func<HuntListItemViewModel> _huntListItemViewModelFactory;
        private readonly INavigator _navigator;
        private readonly IDialogService _dialogService;
        private ObservableCollection<HuntGroup> _huntListItemViewModels;
        public ObservableCollection<HuntGroup> HuntListItemViewModels
        {
            get { return _huntListItemViewModels; }
            set { SetProperty(ref _huntListItemViewModels, value); }
        }

        public Command AddCommand { get; set; }
        public Command DeleteItemCommand { get; set; }
        public HuntsViewModel(IHuntService huntService, Func<HuntListItemViewModel> huntListItemViewModelFactory, INavigator navigator, IDialogService dialogService)
        {
            _huntService = huntService;
            _huntListItemViewModelFactory = huntListItemViewModelFactory;
            _navigator = navigator;
            _dialogService = dialogService;

            AddCommand = new Command(async () => await AddItem());
            DeleteItemCommand = new Command(async (args) => await DeleteItem(args));

        }

        private async Task AddItem()
        {
            Action<Jakt> afterSaveAction = async (hunt) => {
                await _navigator.PushAsync<HuntViewModel>(
                    beforeNavigate: (vm) => vm.SetState(hunt));
            };
            await _navigator.PushModalAsync<EditHuntViewModel>(
                    beforeNavigate: (vm) => vm.SetState(null, afterSaveAction));
        }

        private async Task DeleteItem(object item)
        {
            var ok = await _dialogService.ShowConfirmDialog("Bekreft sletting", "Jakta blir permanent slettet. Er du sikker?");
            if (ok)
            {
                await _huntService.Delete((item as HuntListItemViewModel).ID);
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

            HuntListItemViewModels = new ObservableCollection<HuntGroup>();
            var hunts = await _huntService.GetItems();

            var huntListViewModels = hunts.Select(hunt =>
                {
                    var item = _huntListItemViewModelFactory();
                    item.Initialize(hunt);
                    return item;
                });

            var groups = huntListViewModels.GroupBy(g => g.DateFrom.Year).OrderByDescending(o => o.Key);
            foreach (var g in groups)
            {
                var jg = new HuntGroup(g.Key.ToString(), "");
                foreach (var hunt in g.OrderByDescending(o => o.DateFrom))
                {
                    jg.Add(huntListViewModels.Single(h => h.ID == hunt.ID));
                }
                HuntListItemViewModels.Add(jg);
            }

            IsBusy = false;
        }
    }
}
