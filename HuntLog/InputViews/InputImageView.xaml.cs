using System;
using System.Threading.Tasks;
using HuntLog.AppModule;
using HuntLog.Helpers;
using HuntLog.Services;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace HuntLog.InputViews
{
    public partial class InputImageView : ContentPage
    {
        private readonly InputImageViewModel _viewModel;

        public InputImageView(InputImageViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            _viewModel = viewModel;
        }
    }

    public class InputImageViewModel : InputViewBase
    {
        private readonly IMediaService _mediaService;
        private Action<MediaFile> _completeAction;
        private Action _deleteAction;

        public ImageSource Source { get; set; }

        public Command CaptureCommand { get; set; }
        public Command LibraryCommand { get; set; }
        public Command DeleteCommand { get; set; }


        public InputImageViewModel(INavigator navigator, IDialogService dialogService, IMediaService mediaService) 
                                    : base(navigator, dialogService)
        {
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

        public async Task OnAfterNavigate(string shortcut)
        {
            if (shortcut == HuntConfig.OpenLibrary) 
            { 
                await OpenLibrary(true); 
            }
            if (shortcut == HuntConfig.CapturePhoto) 
            { 
                await CapturePhoto(true); 
            }
        }

        private async Task CapturePhoto(bool isShortCut = false)
        {
            MediaFile = await _mediaService.TakePhotoAsync();
            await AfterMediaRetrieved(isShortCut);
        }

        private async Task OpenLibrary(bool isShortCut = false)
        {
            MediaFile = await _mediaService.OpenLibraryAsync();
            await AfterMediaRetrieved(isShortCut);
        }

        private async Task AfterMediaRetrieved(bool isShortCut)
        {
            Source = ImageSource.FromStream(() =>
            {
                var stream = MediaFile.GetStreamWithImageRotatedForExternalStorage();
                return stream;
            });

            DoneCommand.ChangeCanExecute();

            if (isShortCut)
            {
                //await _navigator.PopAsync();
            }
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
