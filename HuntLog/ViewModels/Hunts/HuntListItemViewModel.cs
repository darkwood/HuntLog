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
        private Hunt _huntDataModel { get; set; }

        public ICommand ShowHuntCommand { get; set; }
        public string Detail { get; set; }
        public HuntListItemViewModel(Hunt hunt, INavigator navigator)
        {
            _huntDataModel = hunt;
            _navigator = navigator;
            ShowHuntCommand = new Command(async () => await ShowHunt());

            Title = hunt.Sted;
            Detail = hunt.DatoFra.ToShortDateString();
        }

        private async Task ShowHunt()
        {
            await _navigator.PushAsync<HuntViewModel>(_huntDataModel);
        }
    }
}
