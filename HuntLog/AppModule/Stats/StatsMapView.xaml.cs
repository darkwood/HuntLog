using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Controls;
using HuntLog.Extensions;
using HuntLog.Models;
using HuntLog.Services;
using LightInject;
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
        private readonly IBaseService<Logg> _logService;
        private IEnumerable<Logg> _logs;

        public StatsFilterViewModel StatsFilterViewModel { get; set; }

        public StatsMapViewModel(StatsFilterViewModel statsFilterViewModel,
                              IBaseService<Logg> logService)
        {
            _logService = logService;
            StatsFilterViewModel = statsFilterViewModel;

            StatsFilterViewModel.DateChangedAction += () =>
            {
                AddPins();
                ZoomToShowAllPins();
            };
        }

        public async Task OnAppearing() 
        {
            _logs = await _logService.GetItems();
            AddPins();
            ZoomToShowAllPins();
        }

        private void AddPins()
        {
            var from = StatsFilterViewModel.DateFrom;
            var to = StatsFilterViewModel.DateTo;

            _mapView.Pins.Clear();

            var logs = _logs.Where(l => l.Dato >= from && l.Dato <= to).ToList();
            foreach (var log in logs)
            {
                if (!string.IsNullOrWhiteSpace(log.Latitude))
                {
                    _mapView.Pins.Add(new Pin
                    {
                        Position = new Position(double.Parse(log.Latitude), double.Parse(log.Longitude)),
                        Type = PinType.Generic,
                        Address = $"{log.Sett}/{log.Skudd}/{log.Treff}",
                        Label = log.Dato.ToNoString()
                    });
                }
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

                var radius = new Distance(Math.Max(spanLatitudeDelta, spanLongitudeDelta) * 20000);
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
