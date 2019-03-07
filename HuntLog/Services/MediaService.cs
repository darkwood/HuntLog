using System;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace HuntLog.Services
{
    public interface IMediaService
    {
        Task<MediaFile> TakePhotoAsync();
        Task<MediaFile> OpenLibraryAsync();
    }
    public class MediaService : IMediaService
    {
        private readonly IDialogService _dialogService;

        public MediaService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task<MediaFile> TakePhotoAsync()
        {
            await CrossMedia.Current.Initialize();

            //var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            //var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            //if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            //{
            //    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage });
            //    cameraStatus = results[Permission.Camera];
            //    storageStatus = results[Permission.Storage];
            //}

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await _dialogService.ShowAlert("No Camera", "No camera available.");
                return null;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "",
                CompressionQuality = 92,
                PhotoSize = PhotoSize.Large,
                AllowCropping = true,
            });
            return file;
        }

        public async Task<MediaFile> OpenLibraryAsync()
        {
            //if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
            //{
            //    var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            //    {
            //        Directory = "Sample",
            //        Name = "test.jpg"
            //    });
            //}
            //else
            //{
            //    await DisplayAlert("Permissions Denied", "Unable to take photos.", "OK");
            //    //On iOS you may want to send your user to the settings screen.
            //    //CrossPermissions.Current.OpenAppSettings();
            //}

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await _dialogService.ShowAlert("Bilder støttes ikke", "Tilgang er ikke gitt til bildebiblioteket. Kan endres i innstillinger på telefonen.");
                return null;
            }

            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = PhotoSize.Large,
                CompressionQuality = 92
            });

            return file;
        }
    }
}
