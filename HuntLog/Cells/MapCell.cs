using System;
using System.Windows.Input;
using HuntLog.Controls;
using HuntLog.Helpers;
using HuntLog.InputViews;
using HuntLog.Services;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace HuntLog.Cells
{
    public class MapCell : BaseCell
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(
                nameof(Text),
                typeof(string),
                typeof(MapCell),
                null,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((MapCell)bindable).TextLabel.Text = newValue as string;
                }
            );

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Label TextLabel { get; private set; }

        /***************************************************************************/

        public static readonly BindableProperty PositionProperty = BindableProperty.Create(
            nameof(Position),
            typeof(Position),
            typeof(MapCell),
            new Position(),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((MapCell)bindable).SetMapPosition((Position)newValue);
            });

        public Position Position
        {
            get { return (Position)GetValue(PositionProperty); }
            set
            {
                SetValue(PositionProperty, value);
            }
        }

        /***************************************************************************/


        public ActivityIndicator ActivityIndicator { get; private set; }
        public ExtendedMap MyMap { get; private set; }
        public Label InfoText { get; set; }

        private void SetMapPosition(Position newPosition)
        {
            Position = newPosition;
            var hasPosition = Position.Latitude != 0 && Position.Longitude != 0;
            MyMap.IsVisible = hasPosition;
            InfoText.IsVisible = !hasPosition;
            ActivityIndicator.IsVisible = false;

            if (!hasPosition)
            {
                return;
            }

            MyMap.MoveToRegion(
                MapSpan.FromCenterAndRadius(
                    Position, Distance.FromMeters(250)));

            var pin = new Pin()
            {
                Position = Position,
                Label = "Valgt posisjon"
            };
            MyMap.Pins.Clear();
            MyMap.Pins.Add(pin);
        }

        public MapCell() : base()
        {
            ViewLayout.HeightRequest = 90;

            TextLabel = new Label
            {
                VerticalOptions = LayoutOptions.Center
            };
            ViewLayout.Children.Add(TextLabel);


            MyMap = new ExtendedMap
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                MapType = MapType.Hybrid,
                HeightRequest = 80,
                WidthRequest = 220,
                IsEnabled = false,
                IsVisible = false
            };
            ViewLayout.Children.Add(MyMap);

            InfoText = new Label
            {
                IsVisible = true,
                Text = "Sett posisjon",
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Utility.PRIMARY_COLOR
            };
            ViewLayout.Children.Add(InfoText);

            ActivityIndicator = new ActivityIndicator
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center,
                IsRunning = true,
                IsVisible = false
            };
            ViewLayout.Children.Add(ActivityIndicator);

            var gestureRecognizer = new TapGestureRecognizer();

            gestureRecognizer.Tapped += async (s, e) =>
            {
                await _navigator.PushAsync<InputPositionViewModel>(
                beforeNavigate: async (arg) =>
                {
                    await arg.InitializeAsync(Position, CellAction.Save, CellAction.Delete);
                });
            };

            ViewLayout.GestureRecognizers.Add(gestureRecognizer);

            View = ViewLayout;
        }
    }
}
