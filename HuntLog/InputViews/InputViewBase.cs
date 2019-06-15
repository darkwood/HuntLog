using System;
using System.Threading.Tasks;
using HuntLog.AppModule;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.InputViews
{
    public class InputViewBase : ViewModelBase
    {
        protected readonly INavigator _navigator;
        protected readonly IDialogService _dialogService;

        public Command CancelCommand { get; set; }
        public Command DoneCommand { get; set; }

        public InputViewBase(INavigator navigator, IDialogService dialogService)
        {
            _navigator = navigator;
            _dialogService = dialogService;
            CancelCommand = new Command(async () => { await _navigator.PopAsync(); });
        }

        public virtual async Task OnAfterNavigate()
        {
            await Task.CompletedTask;
        }

    }
}
