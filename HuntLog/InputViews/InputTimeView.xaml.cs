using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.InputViews
{
    public partial class InputTimeView : ContentPage
    {
        private readonly InputTimeViewModel _viewModel;

        public InputTimeView(InputTimeViewModel viewModel)
        {

            _viewModel = viewModel;
            BindingContext = _viewModel;

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            myTime.Focus();
            base.OnAppearing();
        }
    }

    public class InputTimeViewModel : InputViewBase
    {
        private Action<TimeSpan> _completeAction;

        public TimeSpan CurrentValue { get; set; }
        public InputTimeViewModel(INavigator navigator, IDialogService dialogService) : base(navigator, dialogService)
        {
            DoneCommand = new Command(async () => await Done());
            CancelCommand = new Command(async () => { await _navigator.PopAsync(); });
        }

        public async Task InitializeAsync(TimeSpan value, Action<TimeSpan> completeAction)
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
