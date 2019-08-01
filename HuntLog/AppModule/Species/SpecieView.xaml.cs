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

namespace HuntLog.AppModule.Species
{
    public partial class SpecieView : ContentPage
    {
        private readonly SpecieViewModel _viewModel;

        public SpecieView(SpecieViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            _viewModel = viewModel;
        }
    }

    public class SpecieViewModel : ViewModelBase
    {
        private readonly IBaseService<Art> _specieService;
        private readonly INavigator _navigator;
        private readonly ISelectService<Art> _selectService;
        private readonly IHuntFactory _huntFactory;
        private Art _dto;

        public ObservableCollection<Art> Species { get; set; }

        public Command SaveCommand { get; set; }
        public Command CancelCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command DeleteCommand { get; set; }

        public CellAction ImageAction { get; set; }

        public string Name { get; set; }
        public int GroupId { get; set; }
        public bool Selected { get; private set; }

        public SpecieViewModel(IBaseService<Art> specieService, INavigator navigator, ISelectService<Art> selectService, IHuntFactory huntFactory)
        {
            _specieService = specieService;
            _navigator = navigator;
            _selectService = selectService;
            _huntFactory = huntFactory;

            //SaveCommand = new Command(async () => await Save());
            //DeleteCommand = new Command(async () => await Delete());
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
            Art dto = BuildDto();
            await _specieService.Save(dto);
            await PopAsync();
        }

        private async Task Tapped()
        {
            if (Selected)
            {
                await _selectService.Delete(ID);
            } 
            else 
            {
                await _selectService.Add(ID);
            }

            Selected = !Selected;
            //await _navigator.PushAsync<SpecieViewModel>(beforeNavigate: (arg) => arg.SetState(_dto, Selected));
        }

        //private async Task Delete()
        //{
        //    var ok = await _huntFactory.DeleteSpecie(ID, ImagePath);
        //    if (ok)
        //    {
        //        await PopAsync();
        //    }
        //}

        public void SetState(Art dto, bool selected)
        {
            _dto = dto ?? new Art();
            ID = _dto.ID;
            Name = _dto.Navn;
            GroupId = _dto.GroupId;
            Title = ID == null ? "Ny art" : _dto.Navn;
            Selected = selected;
        }

        protected Art BuildDto()
        {
            return new Art
            {
                ID = ID ?? Guid.NewGuid().ToString(),
                Created = DateTime.Now,
                Navn = Name,
            };
        }
    }
}