using System;
using System.Threading.Tasks;

namespace HuntLog.Services
{
    public interface IDialogService
    {
        Task ShowAlert(string title, string message);
        Task<bool> ShowConfirmDialog(string title, string message);
    }

    public class DialogService : IDialogService
    {
        public async Task ShowAlert(string title, string message)
        {
            await App.Current.MainPage.DisplayAlert(title, message, "OK");
        }

        public async Task<bool> ShowConfirmDialog(string title, string message)
        {
            var confirm = await App.Current.MainPage.DisplayAlert(
                title,
                message,
                "OK",
                "Avbryt");

            return confirm;

        }
    }
}
