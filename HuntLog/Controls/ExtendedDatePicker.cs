using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace HuntLog.Controls
{
    public class ExtendedDatePicker : DatePicker
    {
        public static BindableProperty FocusCommandProperty =BindableProperty.Create(
            nameof(FocusCommand), 
            typeof(ICommand), 
            typeof(DatePicker), 
            null, 
            BindingMode.OneWayToSource);

        public ICommand FocusCommand
        {
            get { return (ICommand)GetValue(FocusCommandProperty); }
            set { SetValue(FocusCommandProperty, value); }
        }

        public ExtendedDatePicker()
        {
            FocusCommand = new Command(() => { this.Focus(); });
        }
    }
}

