using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using HuntLog.Models;
using HuntLog.Models.ListModels;
using HuntLog.Services;

namespace HuntLog.ViewModels.Hunts
{
    public class HuntViewModel : ViewModelBase
    {
        private readonly IHuntService _huntService;

        private Jakt _huntDataModel { get; set; }

        public ObservableCollection<CellData> Items { get; set; }

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

        public async Task SetState(Jakt hunt)
        {
            _huntDataModel = hunt;
            Items = new ObservableCollection<CellData>
            {
                new TextCellData
                {
                    Label = "Sted",
                    Value = _huntDataModel.Sted,
                },
                new DateCellData
                {
                    Label = "Dato fra",
                    Value = _huntDataModel.DatoFra.ToShortDateString(),
                },
                new DateCellData
                {
                    Label = "Dato til",
                    Value = _huntDataModel.DatoTil.ToShortDateString(),
                }
            };
            OnPropertyChanged("");
            await Task.CompletedTask;
        }

        private void Save()
        {
            _huntService?.Save(_huntDataModel);
        }
    }
}
