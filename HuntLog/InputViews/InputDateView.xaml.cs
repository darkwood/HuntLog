using System;
using System.Threading.Tasks;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.InputViews
{
    public partial class InputDateView : ContentPage
    {
        private readonly InputDateViewModel _viewModel;

        public InputDateView(InputDateViewModel viewModel)
        {

            _viewModel = viewModel;
            BindingContext = _viewModel;

            InitializeComponent();
        }
    }

    public class InputDateViewModel : InputViewBase
    {
        private Action<DateTime> _completeAction;

        public DateTime CurrentValue { get; set; }
        public InputDateViewModel(INavigator navigator, IDialogService dialogService) : base(navigator, dialogService)
        {
            DoneCommand = new Command(async () => await Done());
            CancelCommand = new Command(async () => { await _navigator.PopAsync(); });
        }

        public async Task InitializeAsync(DateTime value, Action<DateTime> completeAction)
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
