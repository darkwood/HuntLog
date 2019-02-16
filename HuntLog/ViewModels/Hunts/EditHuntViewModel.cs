using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace HuntLog.ViewModels.Hunts
{
    public class EditHuntViewModel : HuntViewModelBase
    {
        private readonly IHuntService _huntService;
        private readonly INavigator _navigator;
        private readonly IDialogService _dialogService;
        private Action<Jakt> _callback;

        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public EditHuntViewModel(IHuntService huntService, INavigator navigator, IDialogService dialogService)
        {
            _huntService = huntService;
            _navigator = navigator;
            _dialogService = dialogService;

            SaveCommand = new Command(async () => await Save());
            DeleteCommand = new Command(async () => await Delete());
            CancelCommand = new Command(async () => {
                await _navigator.PopModalAsync();
            });
        }

        public async Task SetState(Jakt hunt, Action<Jakt> callback)
        {
            SetStateFromDto(hunt ?? CreateNewHunt());
            Title = IsNew ? "Ny jakt" : "Rediger jakt";
            _callback = callback;
            await Task.CompletedTask;
        }

        private Jakt CreateNewHunt()
        {
            return new Jakt 
            { 
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
                 _navigator.PopModalAsync();
                await _navigator.PopToRootAsync(false);
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
