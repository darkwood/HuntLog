using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Helpers;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.AppModule.Stats.Pages
{
    public partial class StatsBestHunterView : ContentPage
    {
        private readonly StatsBestHunterViewModel _viewModel;

        public StatsBestHunterView(StatsBestHunterViewModel viewModel)
        {
            _viewModel = viewModel;
            BindingContext = _viewModel;

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.OnAppearing();
        }
    }

    public class StatsBestHunterViewModel : ViewModelBase
    {
        private readonly IBaseService<Jeger> _hunterService;
        private readonly IBaseService<Logg> _logService;

        public StatsFilterViewModel StatsFilterViewModel { get; set; }
        public List<StatsListItem> Items { get; set; }

        public StatsBestHunterViewModel(StatsFilterViewModel statsFilterViewModel,
                                         IBaseService<Jeger> hunterService,
                                         IBaseService<Logg> logService)
        {
            StatsFilterViewModel = statsFilterViewModel;
            _hunterService = hunterService;
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
            
            var hunters = await _hunterService.GetItems();
            var logs = await _logService.GetItems();
            var items = new List<StatsListItem>();

            logs = logs.Where(l => l.Dato >= from
                                    && l.Dato <= to
                                    && (hunter == null || l.JegerId == hunter.ID))
                                    .ToList();

            foreach (var h in hunters)
            {
                var hits = logs.Where(l => l.JegerId == h.ID).Sum(s => s.Treff);
                var shots = logs.Where(l => l.JegerId == h.ID).Sum(s => s.Skudd);
                var percent = shots > 0 ? Decimal.Round(hits * 100 / shots) : 0;

                items.Add(new StatsListItem
                {
                    Title = h.Fornavn,
                    Sum = percent,
                    Text2 = percent + "%",
                    Detail = $"{shots} skudd, {hits} treff",
                    ImageSource = Utility.GetImageSource(h.ImagePath)
            }); 
            }
            Items = items.OrderByDescending(o => o.Sum).ToList();
            IsBusy = false;
        }
    }
}
