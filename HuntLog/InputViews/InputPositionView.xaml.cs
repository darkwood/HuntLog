using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;
using HuntLog.AppModule;
using HuntLog.Controls;
using HuntLog.Services;

namespace HuntLog.InputViews
{
    public partial class InputPositionView : ContentPage
    {
        private readonly InputPositionViewModel _viewModel;

        public InputPositionView(InputPositionViewModel viewModel)
        {
            _viewModel = viewModel;
            BindingContext = _viewModel;

            InitializeComponent();

            _viewModel.SetMapView(MyMap);
        }

        void MyMap_Tap(object sender, Controls.TapEventArgs e)
        {
           _viewModel.SetMapPosition(e.Position);
        }

        void OnSetPositionClicked(object sender, System.EventArgs e)
        {
            _viewModel.SetPin();
        }
    }

    public class InputPositionViewModel : ViewModelBase
    {
        private readonly INavigator _navigator;
        private readonly IDialogService _dialogService;
        private Action<object> _completeAction;
        private Action _deleteAction;
        private ExtendedMap _mapView;

        public Command DeleteCommand { get; set; }
        public Command CancelCommand { get; set; }
        public Command DoneCommand { get; set; }
        public Command GetCurrentPositionCommand { get; set; }

        public Position Position { get; set; }
        public string PositionText => Position.Latitude > 0 ? $"{Position.Latitude}, {Position.Latitude}" : string.Empty;
        public bool Loading { get; set; }

        public InputPositionViewModel(INavigator navigator, IDialogService dialogService)
        {
            _navigator = navigator;
            _dialogService = dialogService;

            DoneCommand = new Command(async () => await Done());
            DeleteCommand = new Command(async () => await Delete());
            CancelCommand = new Command(async () => {
                await _navigator.PopAsync();
            });
            GetCurrentPositionCommand = new Command(async () => await SetCurrentPosition());
        }

        public void SetPin()
        {
            var pin = new Pin()
            {
                Position = Position,
                Label = "Ny posisjon valgt",
                Type = PinType.SavedPin
            };
            _mapView.Pins.Clear();
            _mapView.Pins.Add(pin);
        }

        private async Task SetCurrentPosition()
        {
            Loading = true;
            var location = await Geolocation.GetLastKnownLocationAsync();
            if (location != null)
            {
                SetMapPosition(new Position(location.Latitude, location.Longitude));
            }
            Loading = false;
        }

        public async Task InitializeAsync(Position position, Action<object> completeAction, Action deleteAction)
        {
            _completeAction = completeAction;
            _deleteAction = deleteAction;

            if(position.Latitude > 0 && position.Longitude > 0)
            {
                SetMapPosition(position);
            }
            else
            {
                await SetCurrentPosition();
            }

            await Task.CompletedTask;
        }

        public void SetMapView(ExtendedMap mapView)
        {
            _mapView = mapView;
        }

        public void SetMapPosition(Position position)
        {
            Position = position;
            var distance = _mapView.VisibleRegion == null ? Distance.FromMeters(100) : _mapView.VisibleRegion.Radius;

            _mapView.MoveToRegion(
                MapSpan.FromCenterAndRadius(
                    Position, distance));

            var pin = new Pin()
            {
                Position = Position,
                Label = "Valgt posisjon",
                Type = PinType.SearchResult
            };
            _mapView.Pins.Clear();
            _mapView.Pins.Add(pin);
        }

        private async Task Delete()
        {
            var ok = await _dialogService.ShowConfirmDialog("Bekreft sletting", "Posisjonen blir permanent slettet. Er du sikker?");
            if (ok)
            {
                Position = new Position();
                _mapView.Pins.Clear();
                _deleteAction.Invoke();
            }
        }

        private async Task Done()
        {
            _completeAction(Position);
            await _navigator.PopAsync();
        }
    }
}
