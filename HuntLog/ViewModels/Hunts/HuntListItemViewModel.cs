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
        public HuntListItemViewModel(INavigator navigator)
        {
            _navigator = navigator;
            ShowHuntCommand = new Command(async () => await ShowHunt());
        }

        public void Initialize(Hunt hunt)
        {
            _huntDataModel = hunt;

            Title = hunt.Sted;
            Detail = hunt.DatoFra.ToShortDateString();
        }

        private async Task ShowHunt()
        {
            await _navigator.PushAsync<HuntViewModel>(afterNavigate: async (arg) => await arg.InitializeAsync(_huntDataModel));
        }
    }
}
