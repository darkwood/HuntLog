using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HuntLog.AppModule;
using HuntLog.Helpers;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.AppModule.Hunts
{
    public class HuntListItemViewModel : ViewModelBase
    {
        private readonly INavigator _navigator;
        private readonly IBaseService<Jakt> _huntService;

        private Jakt _dto { get; set; }

        public ICommand ItemTappedCommand { get; set; }

        public string ID => _dto.ID;
        public string Detail => _dto.DatoFra.ToShortDateString();
        public DateTime DateFrom => _dto.DatoFra;
        public DateTime DateTo => _dto.DatoTil;
        public ImageSource ImageSource => Utility.GetImageSource(_dto.ImagePath);

        public HuntListItemViewModel(INavigator navigator, IBaseService<Jakt> huntService)
        {
            _navigator = navigator;
            _huntService = huntService;
            ItemTappedCommand = new Command(async () => await ShowHunt());
        }

        public void Initialize(Jakt dto)
        {
            _dto = dto;
            Title = _dto.Sted;
        }

        private async Task ShowHunt()
        {
            await _navigator.PushAsync<HuntViewModel>(
                beforeNavigate: (arg) => arg.SetState(_dto),
                afterNavigate: async(arg) => await arg.AfterNavigate());
        }
    }
}
