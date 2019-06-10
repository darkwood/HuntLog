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

    }

    public class StatsSpeciesListViewModel : ViewModelBase
    {
        private readonly IBaseService<Art> _specieService;
        private readonly IBaseService<Logg> _logService;

        public List<StatsListItem> Items { get; set; }

        public StatsSpeciesListViewModel(IBaseService<Art> specieService,
                                         IBaseService<Logg> logService)
        {
            _specieService = specieService;
            _logService = logService;
        }
        public async Task OnAppearing()
        {
            IsBusy = true;
            var species = await _specieService.GetItems();
            var logs = await _logService.GetItems();

            var items = new List<StatsListItem>();
            foreach(var specie in species)
            {
                var sum = logs.Where(l => l.ArtId == specie.ID).Sum(s => s.Treff);
                if(sum > 0)
                {
                    items.Add(new StatsListItem{ 
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