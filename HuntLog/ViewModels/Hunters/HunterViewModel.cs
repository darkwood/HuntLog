using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.ViewModels.Hunters
{
    public class HunterViewModel : ViewModelBase
    {
        private readonly IHunterService _hunterService;
        private readonly INavigator _navigator;
        private readonly IDialogService _dialogService;
        private Jeger _dto;

        public ObservableCollection<Jeger> Hunters { get; set; }

        public Command SaveCommand { get; set; }
        public Command ItemTappedCommand { get; set; }

        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
        public ImageSource ImageSource { get; set; }
        public string ImagePath { get; set; }

        public HunterViewModel(IHunterService hunterService, INavigator navigator, IDialogService dialogService)
        {
            _hunterService = hunterService;
            _navigator = navigator;
            _dialogService = dialogService;

            SaveCommand = new Command(async () => await Save());
            ItemTappedCommand = new Command(async () => await Tapped());
        }

        private async Task Save()
        {
            await Task.CompletedTask;
        }

        private async Task Tapped()
        {
            await _navigator.PushAsync<HunterViewModel>(beforeNavigate: async (arg) => await arg.SetState(_dto));
            await Task.CompletedTask;
        }

        public async Task SetState(Jeger dto)
        {
            _dto = dto;
            ID = dto.ID;
            Name = $"{dto.Firstname} {dto.Lastname}";
            Email = dto.Email;
            Firstname = dto.Firstname;
            Lastname = dto.Lastname;
            Phone = dto.Phone;
            ImagePath = dto.ImagePath;
            ImageSource = Utility.GetImageSource(dto.ImagePath);

            await Task.CompletedTask;
        }
    }
}