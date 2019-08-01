using System;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

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
            await InitializeAndCheckPermissions();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await _dialogService.ShowAlert("Ingen kamera", "Kamera ikke tilgjengelig.");
                return null;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "",
                CompressionQuality = 92,
                PhotoSize = PhotoSize.Large,
                AllowCropping = true,
                SaveToAlbum = false
            });
            return file;
        }

        private static async Task InitializeAndCheckPermissions()
        {
            await CrossMedia.Current.Initialize();

            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage });
                cameraStatus = results[Permission.Camera];
                storageStatus = results[Permission.Storage];
            }
        }

        public async Task<MediaFile> OpenLibraryAsync()
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await _dialogService.ShowAlert("Bilder støttes ikke", "Tilgang er ikke gitt til bildebiblioteket. Kan endres i innstillinger på telefonen.");
                CrossPermissions.Current.OpenAppSettings();
                return null;
            }

            var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            {
                PhotoSize = PhotoSize.Large,
                CompressionQuality = 92,
            });
            return file;
        }
    }
}
