using System;
using System.Threading.Tasks;
using HuntLog.Helpers;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.ViewModels.Hunts
{
    public class HuntViewModel : ViewModelBase
    {
        private readonly IHuntService _huntService;
        private readonly INavigator _navigator;

        private Jakt _huntDataModel { get; set; }
        public Command EditCommand { get; set; }

        public string Location { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public HuntViewModel(IHuntService huntService, INavigator navigator)
        {
            _huntService = huntService;
            _navigator = navigator;
            EditCommand = new Command(async () => await EditItem());
        }

        public async Task OnAppearing()
        {

        }

        private async Task EditItem()
        {
            var clonedDto = ObjectCloner.Clone(_huntDataModel);
            Action<Jakt> callback = async (arg) => { await SetState(arg); };
            await _navigator.PushModalAsync<EditHuntViewModel>(beforeNavigate: async (arg) => await arg.SetState(clonedDto, callback));
        }

        public async Task SetState(Jakt hunt)
        {
            _huntDataModel = hunt;
            OnPropertyChanged("");
            await Task.CompletedTask;
        }

        private void Save()
        {
            _huntService?.Save(_huntDataModel);
        }
    }
}
