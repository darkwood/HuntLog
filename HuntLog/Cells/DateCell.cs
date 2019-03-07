using System;
using System.Windows.Input;
using HuntLog.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace HuntLog.Cells
{
    public class DateCell : ViewCell
    {
        public static readonly BindableProperty ShowDatePickerProperty =
            BindableProperty.Create(
                nameof(ShowDatePicker),
                typeof(bool),
                typeof(DateCell),
                false,
                propertyChanged: (bindable, oldValue, newValue) => {
                    ((DateCell)bindable).DatePickerView.IsVisible = (bool)newValue;
                    ((DateCell)bindable).DatePickerView.HeightRequest = (bool)newValue ? 100 : 0;
                    ((DateCell)bindable).ForceUpdateSize();
                }
            );

        public bool ShowDatePicker
        {
            get { return (bool)GetValue(ShowDatePickerProperty); }
            set
            {
                SetValue(ShowDatePickerProperty, value);
            }
        }

        public Grid DatePickerView { get; set; }
        /***********************************************/

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(Command), typeof(DateCell), null);

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(
                nameof(Text), 
                typeof(string), 
                typeof(DateCell), 
                null,
                propertyChanged: (bindable, oldValue, newValue) => {
                    ((DateCell)bindable).TextLabel.Text = newValue as string;
                }
            );
        
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Label TextLabel { get; private set; }


        public static readonly BindableProperty Text2Property = BindableProperty.Create(
            nameof(Text2),
            typeof(string),
            typeof(DateCell),
            "",
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var text = (newValue as string);
                if (text != null && text.Length > 30)
                {
                    text = text.Substring(0, 26) + "...";
                }
            ((DateCell)bindable).Text2Label.IsVisible = true;
                ((DateCell)bindable).Text2Label.Text = text;
            });

        public string Text2
        {
            get { return (string)GetValue(Text2Property); }
            set
            {
                SetValue(Text2Property, value);
            }
        }

        public Label Text2Label { get; private set; }


        public DateCell()
        {
            RenderView();
        }

        private void RenderView()
        {
            var wrapperLayout = new StackLayout();

            var viewLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = 10,
                MinimumHeightRequest = 60
            };

            TextLabel = new Label
            {
                VerticalOptions = LayoutOptions.Start
            };
            viewLayout.Children.Add(TextLabel);

            Text2Label = new Label { VerticalOptions = LayoutOptions.Start, HorizontalOptions = LayoutOptions.EndAndExpand };
            viewLayout.Children.Add(Text2Label);

            var gestureRecognizer = new TapGestureRecognizer();

            gestureRecognizer.Tapped += (s, e) =>
            {
                ShowDatePicker = !ShowDatePicker;
                if (Command != null && Command.CanExecute(null))
                {
                    Command.Execute(null);
                }
            };

            viewLayout.GestureRecognizers.Add(gestureRecognizer);
            wrapperLayout.Children.Add(viewLayout);

            DatePickerView = new Grid {
                HeightRequest = 100,
                BackgroundColor = Color.Red,
                IsVisible = false
            };
            wrapperLayout.Children.Add(DatePickerView);
            View = wrapperLayout;
        }
    }
}
