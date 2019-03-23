using System;
using System.Windows.Input;
using HuntLog.Controls;
using HuntLog.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace HuntLog.Cells
{
    public class MapCell : ViewCell
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(Command), typeof(MapCell), null);

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(
                nameof(Text), 
                typeof(string), 
                typeof(MapCell), 
                null,
                propertyChanged: (bindable, oldValue, newValue) => {
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

        public static readonly BindableProperty LatitudeProperty = BindableProperty.Create(
            nameof(Latitude), 
            typeof(double), 
            typeof(MapCell), 
            (double)-1,
            propertyChanged: (bindable, oldValue, newValue) => 
            {
                ((MapCell)bindable).SetMapPosition();
            });

        public double Latitude
        {
            get { return (double)GetValue(LatitudeProperty); }
            set { 
                SetValue(LatitudeProperty, value);
            }
        }

        /***************************************************************************/


        public static readonly BindableProperty LongitudeProperty = BindableProperty.Create(
            nameof(Longitude),
            typeof(double),
            typeof(MapCell),
            (double)-1,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((MapCell)bindable).SetMapPosition();
            });

        public double Longitude
        {
            get { return (double)GetValue(LongitudeProperty); }
            set
            {
                SetValue(LongitudeProperty, value);
            }
        }


        public ActivityIndicator ActivityIndicator { get; private set; }
        public ExtendedMap MyMap { get; private set; }
        public Position Position => new Position(Latitude, Longitude);
        public Label InfoText { get; set; }

        private void SetMapPosition()
        {
            var hasPosition = Longitude > 0 || Latitude > 0;
            MyMap.IsVisible = hasPosition;
            InfoText.IsVisible = !hasPosition;
            ActivityIndicator.IsVisible = false;

            if(!hasPosition){
                return;
            }

            MyMap.MoveToRegion(
                MapSpan.FromCenterAndRadius(
                    Position, Distance.FromMeters(50)));

            var pin = new Pin()
            {
                Position = Position,
                Label = "Valgt posisjon"
            };
            MyMap.Pins.Clear();
            MyMap.Pins.Add(pin);
        }

        public MapCell()
        {
            var viewLayout = new StackLayout
            {
                Orientation=StackOrientation.Horizontal,
                Padding = 10,
                HeightRequest = 80
            };

            TextLabel = new Label{
                VerticalOptions = LayoutOptions.Center
            };
            viewLayout.Children.Add(TextLabel);


            MyMap = new ExtendedMap
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                MapType = MapType.Hybrid,
                HeightRequest = 80,
                WidthRequest = 150,
                IsEnabled = false,
                IsVisible = false
            };
            viewLayout.Children.Add(MyMap);

            InfoText = new Label { 
                IsVisible = true, 
                Text = "Sett posisjon", 
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Utility.PRIMARY_COLOR
            };
            viewLayout.Children.Add(InfoText);

            ActivityIndicator = new ActivityIndicator
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center,
                IsRunning = true,
                IsVisible = false
            };
            viewLayout.Children.Add(ActivityIndicator);

            var gestureRecognizer = new TapGestureRecognizer();

            gestureRecognizer.Tapped += (s, e) => {
                if (Command != null && Command.CanExecute(null))
                {
                    Command.Execute(null);
                }
            };

            viewLayout.GestureRecognizers.Add(gestureRecognizer);

            View = viewLayout;
        }
    }
}
