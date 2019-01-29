using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
        private ObservableCollection<HuntGroup> _huntListItemViewModels;
        public ObservableCollection<HuntGroup> HuntListItemViewModels
        {
            get { return _huntListItemViewModels; }
            set { SetProperty(ref _huntListItemViewModels, value); }
        }

        public Command AddCommand { get; set; }

        public HuntsViewModel(IHuntService huntService, Func<HuntListItemViewModel> huntListItemViewModelFactory, INavigator navigator)
        {
            _huntService = huntService;
            _huntListItemViewModelFactory = huntListItemViewModelFactory;
            _navigator = navigator;
            AddCommand = new Command(async () => await AddHunt());
            Title = "Jaktloggen";
        }

        private async Task AddHunt()
        {
            await _navigator.PushModalAsync<EditHuntViewModel>(beforeNavigate: async (arg) => await arg.SetState(new Models.Jakt()));
        }

        public async Task InitializeAsync()
        {
            await FetchHuntData();
        }

        public async Task FetchHuntData()
        {
            IsBusy = true;

            HuntListItemViewModels = new ObservableCollection<HuntGroup>();
            var hunts = await _huntService.GetHunts();

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
