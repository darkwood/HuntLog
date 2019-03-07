using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using HuntLog.AppModule.InputViews;
using HuntLog.Models;
using HuntLog.Services;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace HuntLog.AppModule.Hunts
{
    public class EditHuntViewModel : HuntViewModelBase
    {
        private readonly IHuntService _huntService;
        private readonly IHunterService _hunterService;
        private readonly INavigator _navigator;
        private readonly IDialogService _dialogService;
        private readonly IFileManager _fileManager;
        private Action<Jakt> _callback;
        private bool _hasNewImage;
        private MediaFile _mediaFile;

        public Command CancelCommand { get; set; }
        public Command SaveCommand { get; set; }
        public Command DeleteCommand { get; set; }

        public Command ImageCommand { get; set; }

        public EditHuntViewModel(IHuntService huntService, IHunterService hunterService, INavigator navigator, IDialogService dialogService, IFileManager fileManager)
        {
            _huntService = huntService;
            _hunterService = hunterService;
            _navigator = navigator;
            _dialogService = dialogService;
            _fileManager = fileManager;

            SaveCommand = new Command(async () => await Save());
            DeleteCommand = new Command(async () => await Delete());
            CancelCommand = new Command(async () => {
                await _navigator.PopAsync();
            });
            ImageCommand = new Command(async () =>
            {
                await _navigator.PushAsync<InputImageViewModel>(
                beforeNavigate: async (arg) => 
                {
                    await arg.InitializeAsync(ImageSource, 
                    completeAction: (mediaFile) => 
                    {
                        _mediaFile = mediaFile;
                        ImageSource = ImageSource.FromStream(() =>
                        {
                            var stream = _mediaFile.GetStreamWithImageRotatedForExternalStorage();
                            return stream;
                        });
                    },
                    deleteAction: () => 
                    {
                        _mediaFile?.Dispose();
                        ImageSource = null;
                        ImagePath = string.Empty;
                    });
                });
            });
        }

        public async Task OnAppearing()
        {
            await SetHunterNames();

            Console.Write("/n------ ALL FILES --------/n");
            string[] files = _fileManager.GetAllFiles();
            foreach(var file in files) {
                Console.WriteLine(file);
            }
        }

        private async Task SetHunterNames()
        {
            var hunters = await _hunterService.GetItems(HunterIds);
            HuntersNames = string.Join(", ", hunters.Select(h => h.Firstname));
        }

        public void SetState(Jakt hunt, Action<Jakt> callback)
        {
            SetStateFromDto(hunt ?? CreateNewHunt());
            Title = IsNew ? "Ny jakt" : "Rediger jakt";
            _callback = callback;
        }

        private Jakt CreateNewHunt()
        {
            return new Jakt
            {
                Created = DateTime.Now,
                DatoFra = DateTime.Now,
                DatoTil = DateTime.Now,
            };
        }

        private async Task Delete()
        {
            var ok = await _dialogService.ShowConfirmDialog("Bekreft sletting", "Jakta blir permanent slettet. Er du sikker?");
            if (ok)
            { 
                await _huntService.Delete(ID);
                await _navigator.PopToRootAsync();
            }
        }

        private async Task Save()
        {

            Jakt dto = CreateHuntDto();
            if (_mediaFile != null)
            {
                dto.ImagePath = SaveImage($"hunt_{DateTime.Now}.jpg");
            }

            await _huntService.Save(dto);
            _callback.Invoke(dto);
            await _navigator.PopAsync();

        }

        private string SaveImage(string filename)
        {
            using (var memoryStream = new MemoryStream())
            {
                _mediaFile.GetStreamWithImageRotatedForExternalStorage().CopyTo(memoryStream);
                _mediaFile.Dispose();
                _fileManager.SaveImage(filename, memoryStream.ToArray());
                return filename;
            }
        }
    }
}
