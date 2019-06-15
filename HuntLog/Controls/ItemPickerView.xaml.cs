using System;
using System.Collections.Generic;
using System.Windows.Input;
using HuntLog.AppModule;
using Xamarin.Forms;

namespace HuntLog.Controls
{
    public class ItemPickerViewModel : ViewModelBase
    {
        public Command Command { get; set; }
        public string Text { get; set; }
    }
    public partial class ItemPickerView : ContentView
    {
        public static readonly BindableProperty CommandProperty =
                BindableProperty.Create(
                    nameof(Command),
                    typeof(Command),
                    typeof(ItemPickerView),
                    null,
                    propertyChanged: (bindable, oldValue, newValue) => {
                        ((ItemPickerView)bindable)._viewModel.Command = newValue as Command;
                    }
                );

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(
                nameof(Text),
                typeof(string),
                typeof(ItemPickerView),
                null,
                propertyChanged: (bindable, oldValue, newValue) => {
                    ((ItemPickerView)bindable)._viewModel.Text = newValue as string;
                }
            );

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        private ItemPickerViewModel _viewModel;
        public ItemPickerView()
        {
            BindingContext = _viewModel = new ItemPickerViewModel();
            InitializeComponent();
        }
    }
}
