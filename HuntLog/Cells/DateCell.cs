using System;
using System.Windows.Input;
using HuntLog.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace HuntLog.Cells
{
    public class DateCell : ViewCell
    {
        public bool ShowDateView { get; set; }

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
            var viewLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = 10,
                MinimumHeightRequest = 60,
                BackgroundColor = ShowDateView ? Color.Red : Color.White
            };

            TextLabel = new Label
            {
                VerticalOptions = LayoutOptions.Center
            };
            viewLayout.Children.Add(TextLabel);

            Text2Label = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.EndAndExpand };
            viewLayout.Children.Add(Text2Label);

            var gestureRecognizer = new TapGestureRecognizer();

            gestureRecognizer.Tapped += (s, e) =>
            {
                ShowDateView = !ShowDateView;
                RenderView();
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
