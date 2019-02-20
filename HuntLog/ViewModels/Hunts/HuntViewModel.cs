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
        private readonly IHunterService _hunterService;
        private readonly INavigator _navigator;
        private Jakt _dto;
        private ExtendedMap _map;
        public Command EditCommand { get; set; }
        public bool MapIsVisible { get; set; }
        public ImageSource MapImageSource => ImageSource.FromResource("HuntLog.Assets.placeholder_map.png");


        public HuntViewModel(IHuntService huntService, IHunterService hunterService, INavigator navigator)
        {
            _huntService = huntService;
            _hunterService = hunterService;
            _navigator = navigator;
            EditCommand = new Command(async () => await EditItem());
        }

        public void SetMap(ExtendedMap map)
        {
            _map = map;
        }

        private async Task EditItem()
        {
            Action<Jakt> callback = (arg) => { SetState(arg); };

            await _navigator.PushModalAsync<EditHuntViewModel>(
                    beforeNavigate: (arg) => arg.SetState(_dto, callback),
                    afterNavigate: async (arg) => await arg.OnAppearing());
        }

        public void SetState(Jakt dto)
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
        }
    }
}
