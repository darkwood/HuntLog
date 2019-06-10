using System;
using System.Windows.Input;
using HuntLog.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace HuntLog.Cells
{
    public class DateCell : BaseCell
    {
        public static readonly BindableProperty ShowDatePickerProperty =
            BindableProperty.Create(
                nameof(ShowDatePicker),
                typeof(bool),
                typeof(DateCell),
                false,
                propertyChanged: (bindable, oldValue, newValue) => {
                    var pickerView = ((DateCell)bindable).DatePickerView;
                    var h = 200;
                    var startHeight = (bool)newValue ? 0 : h;
                    var endHeight = (bool)newValue ? h : 0;
                    var animate = new Animation(d => pickerView.HeightRequest = d, startHeight, endHeight);
                    animate.Commit(pickerView, "showPicker", 16, 400, Easing.CubicOut);
                    //((DateCell)bindable).ForceUpdateSize();
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
        private DatePicker _datePicker { get; set; }

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


        public static readonly BindableProperty DateProperty = BindableProperty.Create(
            nameof(Date),
            typeof(DateTime),
            typeof(DateCell),
            DateTime.MinValue,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var date = (DateTime) newValue;
                ((DateCell)bindable).Text2Label.IsVisible = true;
                ((DateCell)bindable).Text2Label.Text = date.ToString();
            });

        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set
            {
                SetValue(DateProperty, value);
            }
        }

        public Label Text2Label { get; private set; }


        public DateCell()
        {
            RenderView();
        }

        private void RenderView()
        {
            ViewLayout.HeightRequest = 60;

            TextLabel = new Label
            {
                VerticalOptions = LayoutOptions.Start
            };
            ViewLayout.Children.Add(TextLabel);

            Text2Label = new Label { VerticalOptions = LayoutOptions.Start, HorizontalOptions = LayoutOptions.EndAndExpand };
            ViewLayout.Children.Add(Text2Label);

            _datePicker = new DatePicker { IsVisible = false };
            _datePicker.SetBinding(DatePicker.DateProperty, nameof(Date), BindingMode.TwoWay);
            ViewLayout.Children.Add(_datePicker);

            var gestureRecognizer = new TapGestureRecognizer();

            gestureRecognizer.Tapped += (s, e) =>
            {
                _datePicker.Focus();
                if (Command != null && Command.CanExecute(null))
                {
                    Command.Execute(null);
                }
            };

            ViewLayout.GestureRecognizers.Add(gestureRecognizer);
            ViewLayout.Children.Add(ViewLayout);

            //DatePickerView = new Grid {
            //    HeightRequest = 60,
            //    BackgroundColor = Color.LightBlue
            //};

            //ViewLayout.Children.Add(DatePickerView); 

            View = ViewLayout;
        }
    }
}
