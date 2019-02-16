using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.ViewModels.Hunts
{
    public class HuntListItemViewModel : ViewModelBase
    {
        private readonly INavigator _navigator;
        private readonly IHuntService _huntService;

        private Jakt _huntDataModel { get; set; }

        public ICommand ItemTappedCommand { get; set; }

        public string ID => _huntDataModel.ID;
        public string Detail => _huntDataModel.DatoFra.ToShortDateString();
        public DateTime DateFrom => _huntDataModel.DatoFra;
        public DateTime DateTo => _huntDataModel.DatoTil;

        public HuntListItemViewModel(INavigator navigator, IHuntService huntService)
        {
            _navigator = navigator;
            _huntService = huntService;
            ItemTappedCommand = new Command(async () => await ShowHunt());
        }

        public void Initialize(Jakt hunt)
        {
            _huntDataModel = hunt;
            Title = _huntDataModel.Sted;
        }

        private async Task ShowHunt()
        {
            await _navigator.PushAsync<HuntViewModel>(beforeNavigate: async (arg) => await arg.SetState(_huntDataModel));
        }
    }
}
