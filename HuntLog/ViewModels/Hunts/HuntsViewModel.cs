using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Models;
using HuntLog.Services;

namespace HuntLog.ViewModels.Hunts
{
    public class HuntsViewModel : ViewModelBase
    {
        private readonly IHuntService _huntService;
        private readonly Func<Hunt, HuntListItemViewModel> _huntViewModelFactory;
        private IEnumerable<HuntListItemViewModel> _huntViewModels;

        public IEnumerable<HuntListItemViewModel> HuntViewModels
        {
            get { return _huntViewModels; }
            set { SetProperty(ref _huntViewModels, value); }
        }

        private bool _dataLoaded;
        public bool DataLoaded
        {
            get => _dataLoaded; set => SetProperty(ref _dataLoaded, value);
        }

        public async Task Initialize()
        {
            await FetchHuntData();
        }

        public HuntsViewModel(IHuntService huntService,
                Func<Hunt, HuntListItemViewModel> huntViewModelFactory)
        {
            _huntService = huntService;
            _huntViewModelFactory = huntViewModelFactory;
            Title = "Hunts";
        }

        public async Task FetchHuntData()
        {
            var hunts = await _huntService.GetHunts();
            DataLoaded = true;
            HuntViewModels = hunts.Select(x => _huntViewModelFactory(x))
                .ToList();
        }
    }
}
