using System;
using System.Threading.Tasks;
using HuntLog.AppModule;
using HuntLog.Helpers;
using HuntLog.Services;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace HuntLog.AppModule.InputViews
{
    public class InputImageViewModel : ViewModelBase
    {
        private readonly INavigator _navigator;
        private readonly IDialogService _dialogService;
        private readonly IMediaService _mediaService;
        private Action<MediaFile> _completeAction;
        private Action _deleteAction;
        private MediaFile MediaFile { get; set; }

        public ImageSource Source { get; set; }

        public Command CaptureCommand { get; set; }
        public Command LibraryCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command CancelCommand { get; set; }
        public Command DoneCommand { get; set; }


        public InputImageViewModel(INavigator navigator, IDialogService dialogService, IMediaService mediaService)
        {
            _navigator = navigator;
            _dialogService = dialogService;
            _mediaService = mediaService;

            DoneCommand = new Command(async () => await Done(), () => { return MediaFile != null; });
            DeleteCommand = new Command(async () => await Delete());
            LibraryCommand = new Command(async () => await OpenLibrary());
            CaptureCommand = new Command(async () => await CapturePhoto());
            CancelCommand = new Command(async () => {
                await _navigator.PopAsync();
            });
        }

        public async Task InitializeAsync(ImageSource source, Action<MediaFile> completeAction, Action deleteAction)
        {
            _completeAction = completeAction;
            _deleteAction = deleteAction;
            Source = source;
            await Task.CompletedTask;
        }

        private async Task CapturePhoto()
        {
            MediaFile = await _mediaService.TakePhotoAsync();
            Source = ImageSource.FromStream(() =>
            {
                var stream = MediaFile.GetStreamWithImageRotatedForExternalStorage();
                return stream;
            });

            DoneCommand.ChangeCanExecute();
        }

        private async Task OpenLibrary()
        {
            MediaFile = await _mediaService.OpenLibraryAsync();
            Source = ImageSource.FromStream(() =>
            {
                var stream = MediaFile.GetStreamWithImageRotatedForExternalStorage();
                return stream;
            });

            DoneCommand.ChangeCanExecute();
        }

        private async Task Delete()
        {
            var ok = await _dialogService.ShowConfirmDialog("Bekreft sletting", "Bildet blir permanent slettet. Er du sikker?");
            if (ok)
            {
                _deleteAction.Invoke();
            }
        }

        private async Task Done()
        {
            _completeAction(MediaFile);
            await _navigator.PopAsync();
        }
    }
}