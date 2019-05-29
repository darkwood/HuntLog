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
        private Action<DateTime> _completeAction;

        public TimeSpan CurrentTime { get; set; }
        public DateTime CurrentDate { get; set; }
        public DateTime MaxDate { get; set; }
        public DateTime MinDate { get; set; }

        public InputTimeViewModel(INavigator navigator, IDialogService dialogService) : base(navigator, dialogService)
        {
            DoneCommand = new Command(async () => await Done());
            CancelCommand = new Command(async () => { await _navigator.PopAsync(); });
            MaxDate = DateTime.MaxValue;
            MinDate = DateTime.MinValue;
        }

        public async Task InitializeAsync(DateTime value, DateTime from, DateTime to, Action<DateTime> completeAction)
        {
            _completeAction = completeAction;
            CurrentTime = value.TimeOfDay;
            CurrentDate = value;
            MaxDate = from;
            MinDate = to;
            await Task.CompletedTask;
        }

        private async Task Done()
        {
            var date = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day, 
                                    CurrentTime.Hours, CurrentTime.Minutes, CurrentTime.Seconds);

            _completeAction(date);
            await _navigator.PopAsync();
        }
    }
}
