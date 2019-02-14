using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Controls;
using HuntLog.Helpers;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace HuntLog.ViewModels.Hunts
{
    public class HuntViewModel : HuntViewModelBase
    {
        private readonly IHuntService _huntService;
        private readonly INavigator _navigator;
        private Jakt _dto;
        private ExtendedMap _map;
        public Command EditCommand { get; set; }
        public bool MapIsVisible { get; set; }
        public ImageSource MapImageSource => ImageSource.FromResource("HuntLog.Assets.placeholder_map.png");

        public HuntViewModel(IHuntService huntService, INavigator navigator)
        {
            _huntService = huntService;
            _navigator = navigator;
            EditCommand = new Command(async () => await EditItem());
        }

        public void SetMap(ExtendedMap map)
        {
            _map = map;
        }

        private async Task EditItem()
        {
            Action<Jakt> callback = async (arg) => { await SetState(arg); };

            await _navigator.PushModalAsync<EditHuntViewModel>(
                    beforeNavigate: async (arg) => 
                        await arg.SetState(_dto, callback));
        }

        public async Task SetState(Jakt dto)
        {
            _dto = dto;
            SetStateFromDto(_dto);

            if (HasLocation)
            {
                var position = new Position(Latitude, Longitude);
                _map.MoveToRegion(MapSpan.FromCenterAndRadius(position,
                        Distance.FromMeters(1000)));
                _map.Pins.Add(new Pin
                {
                    Label = Location,
                    Position = position,
                    Address = DateFrom.ToShortDateString()
                });
                MapIsVisible = true;
            }

            await Task.CompletedTask;
        }
    }
}
