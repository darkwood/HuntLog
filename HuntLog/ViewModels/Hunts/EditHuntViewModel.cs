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
        private readonly INavigation _navigation;
        private Action<Jakt> _callback;

        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public EditHuntViewModel(IHuntService huntService, INavigation navigation)
        {
            _huntService = huntService;
            _navigation = navigation;
            SaveCommand = new Command(async () => await Save());
            DeleteCommand = new Command(async () => await Delete());
            CancelCommand = new Command(async () => {
                await _navigation.PopModalAsync();
            });
        }

        public async Task OnAppearing() 
        {
            //var addressQuery = "Høylandet";
            //new Geocoder().GetAddressesForPositionAsync()
            //var positions = (await(new Geocoder()).GetPositionsForAddressAsync(addressQuery)).ToList();
            //if (!positions.Any())
            //return;

            //var position = positions.First();
            //var position = new Position(lat, lon);
            await Task.CompletedTask;
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
            //TODO: Add confirm message
            await _huntService.Delete(ID);
            _navigation.PopModalAsync();
            await _navigation.PopToRootAsync(false);
        }

        private async Task Save()
        {
            Jakt dto = CreateHuntDto();
            await _huntService.Save(dto);
            _callback.Invoke(dto);

            await _navigation.PopModalAsync();

        }
    }
}
