using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool MultiSelect { get; set; }

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
            MultiSelect = multiSelect;
            await Task.CompletedTask;
        }

        async Task HandleAction(PickerItem item)
        {
            if (!MultiSelect)
            {
                foreach (var val in CurrentValue.Where(x => x.ID != item.ID))
                {
                    val.Selected = false;
                }

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
