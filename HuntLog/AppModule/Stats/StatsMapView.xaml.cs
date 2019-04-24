using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HuntLog.AppModule.Stats
{
    public partial class StatsMapView : ContentPage
    {
        private readonly StatsMapViewModel _viewModel;

        public StatsMapView(StatsMapViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            _viewModel = viewModel;
        }
    }

    public class StatsMapViewModel : StatsViewModelBase
    {
        public StatsFilterViewModel StatsFilterViewModel { get; set; }
        public string DateRange => StatsFilterViewModel?.DateFrom.ToShortDateString();

        public StatsMapViewModel(StatsFilterViewModel statsFilterViewModel)
        {
            StatsFilterViewModel = statsFilterViewModel;
        }
    }
}
