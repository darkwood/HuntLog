using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;
using HuntLog.Helpers;
using Plugin.Media.Abstractions;
using HuntLog.Cells;
using HuntLog.Factories;

namespace HuntLog.AppModule.Dogs
{
    public partial class DogView : ContentPage
    {
        private readonly DogViewModel _viewModel;

        public DogView(DogViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            _viewModel = viewModel;
        }
    }

    public class DogViewModel : ViewModelBase
    {
        private readonly IBaseService<Dog> _dogService;
        private readonly INavigator _navigator;
        private readonly IFileManager _fileManager;
        private readonly IHuntFactory _huntFactory;
        private Dog _dto;

        public ObservableCollection<Dog> Dogs { get; set; }

        public Command SaveCommand { get; set; }
        public Command CancelCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command DeleteCommand { get; set; }

        public CellAction ImageAction { get; set; }

        public string Name { get; set; }
        public string Breed { get; set; }
        public string RegNo { get; set; }

        public DogViewModel(IBaseService<Dog> dogService, INavigator navigator, IFileManager fileManager, IHuntFactory huntFactory)
        {
            _dogService = dogService;
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
                ImagePath = string.Empty;
            };
        }

        private async Task PopAsync()
        {
            await _navigator.PopAsync();
        }

        private async Task Save()
        {
            Dog dto = BuildDto();
            if (MediaFile != null)
            {
                SaveImage($"dog_{ID}.jpg", _fileManager);
            }
            await _dogService.Save(dto);
            await PopAsync();
        }

        private async Task Tapped()
        {
            await _navigator.PushAsync<DogViewModel>(beforeNavigate: (arg) => arg.SetState(_dto));
        }

        private async Task Delete()
        {
            var ok = await _huntFactory.DeleteDog(ID, ImagePath);
            if (ok)
            {
                await PopAsync();
            }
        }

        public void SetState(Dog dto)
        {
            _dto = dto ?? new Dog();
            ID = _dto.ID;
            Name = _dto.Navn;
            Breed = _dto.Rase;
            RegNo = _dto.Lisensnummer;
            ImagePath = $"dog_{ID}.jpg";
            ImageSource = Utility.GetImageSource(_dto.ImagePath);

            Title = ID == null ? "Ny hund" : "Rediger hund";
        }

        protected Dog BuildDto()
        {
            return new Dog
            {
                ID = ID ?? Guid.NewGuid().ToString(),
                Created = DateTime.Now,
                Navn = Name,
                Rase = Breed,
                Lisensnummer = RegNo,
                ImagePath = ImagePath,
            };
        }
    }
}