using System;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.ViewModels.Hunts
{
    public class EditHuntViewModel : HuntViewModelBase
    {
        private readonly IHuntService _huntService;
        private readonly IHunterService _hunterService;
        private readonly INavigator _navigator;
        private readonly IDialogService _dialogService;
        private Action<Jakt> _callback;

        public Command CancelCommand { get; set; }
        public Command SaveCommand { get; set; }
        public Command DeleteCommand { get; set; }

        public EditHuntViewModel(IHuntService huntService, IHunterService hunterService, INavigator navigator, IDialogService dialogService)
        {
            _huntService = huntService;
            _hunterService = hunterService;
            _navigator = navigator;
            _dialogService = dialogService;

            SaveCommand = new Command(async () => await Save());
            DeleteCommand = new Command(async () => await Delete());
            CancelCommand = new Command(async () => {
                await _navigator.PopModalAsync();
            });
        }

        public async Task OnAppearing()
        {
            await SetHunterNames();
        }

        private async Task SetHunterNames()
        {
            var hunters = await _hunterService.GetItems(HunterIds);
            HuntersNames = string.Join(", ", hunters.Select(h => h.Firstname));
        }

        public void SetState(Jakt hunt, Action<Jakt> callback)
        {
            SetStateFromDto(hunt ?? CreateNewHunt());
            Title = IsNew ? "Ny jakt" : "Rediger jakt";
            _callback = callback;
        }

        private Jakt CreateNewHunt()
        {
            return new Jakt
            {
                Created = DateTime.Now,
                DatoFra = DateTime.Now,
                DatoTil = DateTime.Now,
            };
        }

        private async Task Delete()
        {
            var ok = await _dialogService.ShowConfirmDialog("Bekreft sletting", "Jakta blir permanent slettet. Er du sikker?");
            if (ok)
            { 
                await _huntService.Delete(ID);
                await _navigator.PopModalAsync();
                await _navigator.PopAsync();
            }
        }

        private async Task Save()
        {
            Jakt dto = CreateHuntDto();
            await _huntService.Save(dto);
            _callback.Invoke(dto);

            await _navigator.PopModalAsync();

        }
    }
}
