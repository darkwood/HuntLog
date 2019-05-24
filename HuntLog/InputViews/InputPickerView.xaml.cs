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

        private bool _multiSelect;

        public InputPickerViewModel(INavigator navigator, IDialogService dialogService) : base(navigator, dialogService) 
        {
            DoneCommand = new Command(async () => await Done());
        }

        public async Task InitializeAsync(List<PickerItem> value, 
                                        Action<List<PickerItem>> completeAction,
                                        bool multiSelect = false)
        {
            _completeAction = completeAction;
            CurrentValue = value;
            foreach(var val in CurrentValue)
            {
                val.ParentAction = HandleAction;
            }
            _multiSelect = multiSelect;
            await Task.CompletedTask;
        }

        async Task HandleAction(PickerItem item)
        {
            if (!_multiSelect)
            {
                foreach (var val in CurrentValue)
                {
                    val.Selected = false;
                }
                item.Selected = true;
                await Done();
            }
        }

        private async Task Done()
        {
            _completeAction(CurrentValue);
            await _navigator.PopAsync();
        }
    }
}
