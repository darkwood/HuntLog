using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;
using HuntLog.Helpers;
using HuntLog.InputViews;
using Plugin.Media.Abstractions;
using HuntLog.Cells;
using HuntLog.Factories;

namespace HuntLog.AppModule.Hunters
{
    public partial class HunterView : ContentPage
    {
        private readonly HunterViewModel _viewModel;

        public HunterView(HunterViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            _viewModel = viewModel;
        }
    }

    public class HunterViewModel : ViewModelBase
    {
        private readonly IBaseService<Jeger> _hunterService;
        private readonly INavigator _navigator;
        private readonly IFileManager _fileManager;
        private readonly IHuntFactory _huntFactory;
        private Jeger _dto;

        public ObservableCollection<Jeger> Hunters { get; set; }

        public Command SaveCommand { get; set; }
        public Command CancelCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command DeleteCommand { get; set; }

        public Command FirstnameCommand { get; set; }
        public Command LastnameCommand { get; set; }
        public Command PhoneCommand { get; set; }
        public Command EmailCommand { get; set; }

        public CellAction ImageAction { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }

        public HunterViewModel(IBaseService<Jeger> hunterService, INavigator navigator, IFileManager fileManager, IHuntFactory huntFactory)
        {
            _hunterService = hunterService;
            _navigator = navigator;
            _fileManager = fileManager;
            _huntFactory = huntFactory;

            SaveCommand = new Command(async () => await Save());
            DeleteCommand = new Command(async () => await Delete());
            ItemTappedCommand = new Command(async () => await Tapped());
            CancelCommand = new Command(async () => { await PopAsync(); });

            CreateImageActions();
        }

        private void CreateImageActions()
        {
            ImageAction = new CellAction();
            ImageAction.Save += (object obj) =>
            {
                MediaFile = (MediaFile)obj;
                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = MediaFile.GetStreamWithImageRotatedForExternalStorage();
                    return stream;
                });
                OnPropertyChanged(nameof(ImageSource));
            };

            ImageAction.Delete += () =>
            {
                MediaFile?.Dispose();
                ImageSource = null;
            };
        }

        private async Task PopAsync()
        {
            await _navigator.PopAsync();
        }

        private async Task Save()
        {
            Jeger dto = BuildDto();
            if (MediaFile != null)
            {
                SaveImage(dto.ImagePath, _fileManager);
            }
            await _hunterService.Save(dto);
            await PopAsync();
        }

        private async Task Tapped()
        {
            await _navigator.PushAsync<HunterViewModel>(beforeNavigate: (arg) => arg.SetState(_dto));
        }

        private async Task Delete()
        {
            var ok = await _huntFactory.DeleteHunter(ID);
            if (ok)
            {
                await PopAsync();
            }
        }

        public void SetState(Jeger dto)
        {
            _dto = dto ?? new Jeger();
            ID = _dto.ID;
            Name = $"{_dto.Fornavn} {_dto.Etternavn}";
            Email = _dto.Email;
            Firstname = _dto.Fornavn;
            Lastname = _dto.Etternavn;
            Phone = _dto.Phone;
            ImageSource = Utility.GetImageSource(_dto.ImagePath);

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
                Fornavn = Firstname,
                Etternavn = Lastname
            };
        }
    }
}