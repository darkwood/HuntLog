using System;
using System.Threading.Tasks;
using HuntLog.Models;
using HuntLog.Services;

namespace HuntLog.ViewModels.Hunts
{
    public class HuntViewModel : ViewModelBase
    {
        private readonly IHuntService _huntService;

        private Jakt _huntDataModel = new Jakt();

        public string Location
        {
            get => _huntDataModel?.Sted;
            set
            {
                _huntDataModel.Sted = value;
                Save();
                OnPropertyChanged(nameof(Location));
            }
        }

        public DateTime DateFrom
        {
            get => _huntDataModel?.DatoFra ?? DateTime.Now;
            set
            {
                _huntDataModel.DatoFra = value;
                Save();
                OnPropertyChanged(nameof(DateFrom));
            }
        }

        public DateTime DateTo
        {
            get => _huntDataModel?.DatoTil ?? DateTime.Now;
            set
            {
                _huntDataModel.DatoTil = value;
                Save();
                OnPropertyChanged(nameof(DateTo));
            }
        }
        public HuntViewModel(IHuntService huntService)
        {
            _huntService = huntService;
        }


        public async Task InitializeAsync(Jakt hunt)
        {
            IsBusy = true;
            {
                await Task.Delay(1000);
                _huntDataModel = hunt;
                OnPropertyChanged(nameof(Location));
                IsBusy = false;
            }
}

        private void Save()
        {
            _huntService?.Save(_huntDataModel);
        }
    }
}
