using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HuntLog.AppModule;
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
        protected Action<List<InputPickerItemViewModel>> _completeAction;

        public List<InputPickerItemViewModel> CurrentValue { get; set; }

        public InputPickerViewModel(INavigator navigator, IDialogService dialogService) : base(navigator, dialogService) 
        {
            DoneCommand = new Command(async () => await Done());
        }

        public async Task InitializeAsync(List<InputPickerItemViewModel> value, Action<List<InputPickerItemViewModel>> completeAction)
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

    public class InputPickerItemViewModel : ViewModelBase
    {
        public string ID { get; set; }
        public ImageSource ImageSource { get; set; }
        public bool Selected { get; set; }

        public Command Tapped { get; set; }

        public InputPickerItemViewModel()
        {
            Tapped = new Command(() => 
            {
                Selected = !Selected;
            });
        }
    }
}
