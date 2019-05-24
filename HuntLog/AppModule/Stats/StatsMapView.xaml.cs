using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Controls;
using HuntLog.Extensions;
using HuntLog.Factories;
using HuntLog.Models;
using HuntLog.Services;
using LightInject;
using Plugin.Segmented.Control;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace HuntLog.AppModule.Stats
{
    public partial class StatsMapView : ContentPage
    {
        private readonly StatsMapViewModel _viewModel;

        public StatsMapView(StatsMapViewModel viewModel)
        {

            _viewModel = viewModel;
            BindingContext = _viewModel;

            InitializeComponent();

            _viewModel.SetMapView(ExtendedMapControl);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.OnAppearing();
        }
    }

    public class StatsMapViewModel : ViewModelBase
    {
        private ExtendedMap _mapView;
        private readonly IHuntFactory _huntFactory;
        private readonly IBaseService<Logg> _logService;
        private IEnumerable<Logg> _logs;
        private string _huntId;

        public StatsFilterViewModel StatsFilterViewModel { get; set; }
        public int SelectedSegment { get; set; }
        public bool ShowMap { get; set; }
        public bool ShowEmptyMessage { get; set; }
        public bool UseFilterView { get; set; }
        public Command SegmentSelectedCommand { get; set; }

        public StatsMapViewModel(StatsFilterViewModel statsFilterViewModel,
                              IHuntFactory huntFactory,
                              IBaseService<Logg> logService)
        {
            _huntFactory = huntFactory;
            _logService = logService;
            StatsFilterViewModel = statsFilterViewModel;

            StatsFilterViewModel.FilterChangedAction += async () =>
            {
                await AddPins();
                ZoomToShowAllPins();
            };
        }

        public void BeforeNavigate(string huntId) 
        {
            _huntId = huntId;
            UseFilterView = string.IsNullOrEmpty(_huntId);
            StatsFilterViewModel.SetVisibility(UseFilterView);
        }
       
        public async Task OnAppearing() 
        {
            _logs = await _logService.GetItems();

            if (!string.IsNullOrEmpty(_huntId))
            {
                _logs = _logs.Where(l => l.JaktId == _huntId);
            }

           await AddPins();
            ZoomToShowAllPins();
            SegmentSelectedCommand = new Command(async () => { await SelectedSegmentChanged(); });
        }

        private async Task SelectedSegmentChanged()
        {
            await AddPins();
            ZoomToShowAllPins();
        }


        private async Task AddPins()
        {
            var from = StatsFilterViewModel.DateFrom;
            var to = StatsFilterViewModel.DateTo;
            var hunter = StatsFilterViewModel.SelectedHunter;

            _mapView.Pins.Clear();


            var logs = UseFilterView 
                            ? _logs.Where(l => l.Dato >= from 
                                    && l.Dato <= to 
                                    && (hunter == null || l.JegerId == hunter.ID))
                                    .ToList()
                            : _logs;

            foreach (var log in logs)
            {
                if (!string.IsNullOrWhiteSpace(log.Latitude))
                {
                    if(IsValidSegment(log))
                    {
                        _mapView.Pins.Add(new Pin
                        {
                            Position = new Position(double.Parse(log.Latitude), double.Parse(log.Longitude)),
                            Type = PinType.SearchResult,
                            Address = await _huntFactory.CreateLogSummary(log),
                            Label = log.Dato.ToNoString()
                        });
                    }
                }
            }
            ShowMap = _mapView.Pins.Any();
            ShowEmptyMessage = !ShowMap;
        }

        private bool IsValidSegment(Logg log)
        {
            switch (SelectedSegment)
            {
                case 1:
                    return log.Sett > 0;
                case 2:
                    return log.Skudd > 0;
                case 3:
                    return log.Treff > 0;
                case 0:
                default:
                    return true;
            }
        }

        protected void ZoomToShowAllPins()
        {
            if (_mapView.Pins.Count > 0)
            {
                var southWest = new Position(_mapView.Pins[0].Position.Latitude, _mapView.Pins[0].Position.Longitude);
                var northEast = southWest;

                foreach (var pin in _mapView.Pins)
                {
                    southWest = new Position(Math.Min(southWest.Latitude, pin.Position.Latitude),
                                             Math.Min(southWest.Longitude, pin.Position.Longitude)
                                            );

                    northEast = new Position(Math.Max(northEast.Latitude, pin.Position.Latitude),
                                             Math.Max(northEast.Longitude, pin.Position.Longitude)
                                            );

                }

                var spanLatitudeDelta = Math.Abs(northEast.Latitude - southWest.Latitude) * 2;
                var spanLongitudeDelta = Math.Abs(northEast.Longitude - southWest.Longitude) * 2;

                var regionCenterLatitude = (southWest.Latitude + northEast.Latitude) / 2;
                var regionCenterLongitude = (southWest.Longitude + northEast.Longitude) / 2;

                var radius = new Distance(Math.Max(spanLatitudeDelta, spanLongitudeDelta) * 25000);
                var center = new Position(regionCenterLatitude, regionCenterLongitude);

                _mapView.MoveToRegion(MapSpan.FromCenterAndRadius(center, radius));
            }
        }

        public void SetMapView(ExtendedMap mapView)
        {
            _mapView = mapView;
        }


    }
}
