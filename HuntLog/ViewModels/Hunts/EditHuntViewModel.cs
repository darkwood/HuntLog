using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.ViewModels.Hunts
{
    public class EditHuntViewModel : ViewModelBase
    {
        private readonly IHuntService _huntService;
        private readonly INavigation _navigation;

        private Jakt _huntDataModel { get; set; }

        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public string Location
        {
            get => _huntDataModel?.Sted;
            set
            {
                _huntDataModel.Sted = value;
                OnPropertyChanged(nameof(Location));
            }
        }

        public DateTime DateFrom
        {
            get => _huntDataModel?.DatoFra ?? DateTime.Now;
            set
            {
                _huntDataModel.DatoFra = value;
                OnPropertyChanged(nameof(DateFrom));
            }
        }

        public DateTime DateTo
        {
            get => _huntDataModel?.DatoTil ?? DateTime.Now;
            set
            {
                _huntDataModel.DatoTil = value;
                OnPropertyChanged(nameof(DateTo));
            }
        }

        public bool IsNew => _huntDataModel.ID == string.Empty;

        public EditHuntViewModel(IHuntService huntService, INavigation navigation)
        {
            _huntService = huntService;
            _navigation = navigation;
            SaveCommand = new Command(Save);
            CancelCommand = new Command(() => {
                _navigation.PopModalAsync();
            });
        }

        public async Task SetState(Jakt hunt)
        {
            _huntDataModel = hunt;
            Title = IsNew ? "Ny jakt" : "Rediger";
            OnPropertyChanged("");
            await Task.CompletedTask;
        }

        private void Save()
        {
            if (IsNew) {
                _huntDataModel.ID = Guid.NewGuid().ToString();
            }

            _huntService?.Save(_huntDataModel);
            _navigation.PopModalAsync();
        }
    }
}
