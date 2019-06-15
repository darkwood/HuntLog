using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.InputViews
{
    public partial class InputTextView : ContentPage
    {
        private readonly InputTextViewModel _viewModel;

        public InputTextView(InputTextViewModel viewModel)
        {
            _viewModel = viewModel;
            BindingContext = _viewModel;
            InitializeComponent();

        }
        protected override void OnAppearing()
        {
            MyEntry.Focus();
            base.OnAppearing();
        }
    }

    public class InputTextViewModel : InputViewBase
    {
        private Action<string> _completeAction;

        public string CurrentValue { get; set; }
        public Keyboard Keyboard { get; private set; }

        public InputTextViewModel(INavigator navigator, IDialogService dialogService) : base(navigator, dialogService)
        {
            DoneCommand = new Command(async () => await Done());
        }

        public async Task InitializeAsync(string title, string value, Action<string> completeAction, Keyboard keyboard = null)
        {
            _completeAction = completeAction;
            Title = title;
            CurrentValue = value;
            Keyboard = keyboard ?? Keyboard.Default;
            await Task.CompletedTask;
        }

        private async Task Done()
        {
            if(Keyboard == Keyboard.Numeric)
            {
                int val = 0;
                if (string.IsNullOrWhiteSpace(CurrentValue)) { CurrentValue = "0"; }
                if(!int.TryParse(CurrentValue, out val))
                {
                    await _dialogService.ShowAlert("Ikke gyldig verdi", 
                                                   "Verdien angitt er ikke et tall, prøv igjen.");
                    return;
                }
            }
            _completeAction(CurrentValue);
            await _navigator.PopAsync();
        }
    }
}
