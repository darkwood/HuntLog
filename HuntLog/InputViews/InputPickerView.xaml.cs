using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.InputViews
{
    public partial class InputPickerView : ContentPage
    {
        private readonly InputPickerViewModel _viewModel;

        public InputPickerView(InputPickerViewModel viewModel)
        {
            _viewModel = viewModel;
            BindingContext = _viewModel;

            InitializeComponent();
        }
    }

    public class InputPickerViewModel : InputViewBase
    {
        protected Action<List<PickerItem>> _completeAction;

        public List<PickerItem> CurrentValue { get; set; }

        public InputPickerViewModel(INavigator navigator, IDialogService dialogService) : base(navigator, dialogService) 
        {
            DoneCommand = new Command(async () => await Done());
        }

        public async Task InitializeAsync(List<PickerItem> value, Action<List<PickerItem>> completeAction)
        {
            _completeAction = completeAction;
            CurrentValue = value;
            await Task.CompletedTask;
        }

        private async Task Done()
        {
            _completeAction(CurrentValue);
            await _navigator.PopAsync();
        }
    }
}
