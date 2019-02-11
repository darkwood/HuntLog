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
    public class HuntViewModel : ViewModelBase
    {
        private readonly IHuntService _huntService;
        private readonly INavigator _navigator;
        private Jakt _dto { get; set; }

        public Command EditCommand { get; set; }

        public string Location => _dto?.Sted;
        public DateTime? DateFrom => _dto?.DatoFra;
        public DateTime? DateTo => _dto?.DatoTil;
        public ImageSource ImageSource => Utility.GetImageSource(_dto?.ImagePath);
        public List<Jeger> Jegere { get; set; }
        public List<Dog> Dogs { get; set; }

        public HuntViewModel(IHuntService huntService, INavigator navigator)
        {
            _huntService = huntService;
            _navigator = navigator;
            EditCommand = new Command(async () => await EditItem());
            Jegere = new List<Jeger>();
        }

        public async Task OnAppearing(ExtendedMap map)
        {
            double lat;
            double lon;

            if( double.TryParse(_dto.Latitude.Replace('.', ','), out lat) && 
                double.TryParse(_dto.Longitude.Replace('.', ','), out lon))
            {
                //var addressQuery = "Høylandet";

                //var positions = (await(new Geocoder()).GetPositionsForAddressAsync(addressQuery)).ToList();
                //if (!positions.Any())
                //return;

                //var position = positions.First();
                var position = new Position(lat, lon);
                map.MoveToRegion(MapSpan.FromCenterAndRadius(position,
                        Distance.FromMeters(1000)));
                map.Pins.Add(new Pin
                {
                    Label = Location,
                    Position = position,
                    Address = DateFrom?.ToShortDateString()
                });
            }

            await Task.CompletedTask;
        }

        private async Task EditItem()
        {
            var clonedDto = ObjectCloner.Clone(_dto);
            Action<Jakt> callback = async (arg) => { await SetState(arg); };
            await _navigator.PushModalAsync<EditHuntViewModel>(beforeNavigate: async (arg) => await arg.SetState(clonedDto, callback));
        }

        public async Task SetState(Jakt hunt)
        {
            _dto = hunt;
            if (_dto.JegerIds.Any())
            {
                Jegere = _dto.JegerIds.Select(j => new Jeger
                {
                    ID = j.ToString(),
                    Fornavn = "Test " + j
                }).ToList();
            }

            OnPropertyChanged("");
            await Task.CompletedTask;
        }

        private void Save()
        {
            _huntService?.Save(_dto);
        }
    }
}
