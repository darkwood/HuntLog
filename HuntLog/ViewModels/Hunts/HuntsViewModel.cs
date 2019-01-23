using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Models;
using HuntLog.Services;

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

        private ObservableCollection<HuntGroup> _huntListItemViewModels;
        public ObservableCollection<HuntGroup> HuntListItemViewModels
        {
            get { return _huntListItemViewModels; }
            set { SetProperty(ref _huntListItemViewModels, value); }
        }

        public async Task InitializeAsync()
        {
            await FetchHuntData();
        }


        public HuntsViewModel(IHuntService huntService, Func<HuntListItemViewModel> huntListItemViewModelFactory)
        {
            _huntService = huntService;
            _huntListItemViewModelFactory = huntListItemViewModelFactory;
            Title = "Jaktloggen";
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
