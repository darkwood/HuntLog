using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.InputViews;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.AppModule.Stats.Pages
{
    public partial class StatsSpeciesListView : ContentPage
    {
        private readonly StatsSpeciesListViewModel _viewModel;

        public StatsSpeciesListView(StatsSpeciesListViewModel viewModel)
        {
            _viewModel = viewModel;
            BindingContext = _viewModel;

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await _viewModel.OnAppearing();
            base.OnAppearing();
        }

    }

    public class StatsSpeciesListViewModel : ViewModelBase
    {
        private readonly IBaseService<Art> _specieService;
        private readonly IBaseService<Logg> _logService;

        public StatsFilterViewModel StatsFilterViewModel { get; set; }
        public List<StatsListItem> Items { get; set; }

        public StatsSpeciesListViewModel(StatsFilterViewModel statsFilterViewModel, 
                                         IBaseService<Art> specieService,
                                         IBaseService<Logg> logService)
        {
            StatsFilterViewModel = statsFilterViewModel;
            _specieService = specieService;
            _logService = logService;

            StatsFilterViewModel.FilterChangedAction += async () =>
            {
                await PopulateItems();
            };
        }
        public async Task OnAppearing()
        {
            await PopulateItems();
        }

        private async Task PopulateItems()
        {
            IsBusy = true;

            var from = StatsFilterViewModel.DateFrom;
            var to = StatsFilterViewModel.DateTo;
            var hunter = StatsFilterViewModel.SelectedHunter;

            var species = await _specieService.GetItems();
            var logs = await _logService.GetItems();
            var items = new List<StatsListItem>();

            logs = logs.Where(l => l.Dato >= from
                                    && l.Dato <= to
                                    && (hunter == null || l.JegerId == hunter.ID))
                                    .ToList();

            foreach (var specie in species)
            {
                var sum = logs.Where(l => l.ArtId == specie.ID).Sum(s => s.Treff);
                if (sum > 0)
                {
                    items.Add(new StatsListItem
                    {
                        Title = specie.Navn,
                        Sum = sum
                    });
                }
            }
            Items = items.OrderByDescending(o => o.Sum).ToList();
            IsBusy = false;
        }
    }

    public class StatsListItem
    {
        public string Title { get; set; }
        public int Sum { get; set; }
        public string Detail { get; set; }
    }
}