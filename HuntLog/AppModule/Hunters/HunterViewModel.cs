using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;
using HuntLog.Helpers;
using HuntLog.InputViews;
using Plugin.Media.Abstractions;

namespace HuntLog.AppModule.Hunters
{
    public class HunterViewModel : ViewModelBase
    {
        private readonly IBaseService<Jeger> _hunterService;
        private readonly INavigator _navigator;
        private readonly IDialogService _dialogService;
        private Jeger _dto;
        private MediaFile _mediaFile;

        public ObservableCollection<Jeger> Hunters { get; set; }

        public Command SaveCommand { get; set; }
        public Command CancelCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command DeleteCommand { get; set; }

        public Command ImageCommand { get; set; }

        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
        public ImageSource ImageSource { get; set; }
        public string ImagePath { get; set; }
        public bool IsNew { get; set; }

        public HunterViewModel(IBaseService<Jeger> hunterService, INavigator navigator, IDialogService dialogService)
        {
            _hunterService = hunterService;
            _navigator = navigator;
            _dialogService = dialogService;

            SaveCommand = new Command(async () => await Save());
            DeleteCommand = new Command(async () => await Delete());
            ItemTappedCommand = new Command(async () => await Tapped());
            CancelCommand = new Command(async () => { await PopAsync(); });

            ImageCommand = new Command(async (shortcut) => await EditImage(shortcut));
        }

        private async Task EditImage(object shortcut)
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
                            OnPropertyChanged(nameof(ImageSource));
                        },
                        deleteAction: () =>
                        {
                            _mediaFile?.Dispose();
                            ImageSource = null;
                            ImagePath = string.Empty;
                        });
                },
                afterNavigate: async (arg) => await arg.OnAfterNavigate(shortcut as string),
                shortcut == null);
        }

        private async Task PopAsync()
        {
            await _navigator.PopAsync();
        }

        private async Task Save()
        {
            Jeger dto = BuildDto();
            await _hunterService.Save(dto);
            await PopAsync();
        }

        private async Task Tapped()
        {
            await _navigator.PushAsync<HunterViewModel>(beforeNavigate: (arg) => arg.SetState(_dto));
        }

        private async Task Delete()
        {
            var ok = await _dialogService.ShowConfirmDialog("Bekreft sletting", "Jeger blir permanent slettet. Er du sikker?");
            if (ok)
            {
                await _hunterService.Delete(ID);
                await PopAsync();
            }
        }

        public void SetState(Jeger dto)
        {
            _dto = dto ?? new Jeger();
            ID = _dto.ID;
            Name = $"{_dto.Firstname} {_dto.Lastname}";
            Email = _dto.Email;
            Firstname = _dto.Firstname;
            Lastname = _dto.Lastname;
            Phone = _dto.Phone;
            ImagePath = _dto.ImagePath;
            ImageSource = Utility.GetImageSource(_dto.ImagePath);

            IsNew = ID == null;
            Title = ID == null ? "Ny jeger" : "Rediger jeger";
        }

        protected Jeger BuildDto()
        {
            return new Jeger
            {
                ID = ID ?? Guid.NewGuid().ToString(),
                Created = DateTime.Now,
                Email = Email,
                Phone = Phone,
                Firstname = Firstname,
                Lastname = Lastname,
                ImagePath = ImagePath,
            };
        }
    }
}