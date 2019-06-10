using System;
using System.Collections.Generic;
using HuntLog.AppModule;
using HuntLog.Helpers;
using HuntLog.InputViews;
using HuntLog.Models;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace HuntLog.AppModule.Hunts
{
    public class HuntViewModelBase : ViewModelBase
    {
        protected Jakt _dto;
        public string Location { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public List<string> DogIds { get; set; }
        public Position Position { get; set; }
        public string Notes { get; set; }

        public bool HasLocation => Position.Latitude > 0 && Position.Longitude > 0;

        public List<PickerItem> SelectedHuntersAndDogs { get; set; }

        protected void SetStateFromDto(Jakt dto)
        {
            _dto = dto;
            ID = dto.ID;
            Location = dto.Sted;
            DateFrom = dto.DatoFra;
            DateTo = dto.DatoTil;
            DogIds = dto.DogIds;
            Position = new Position(Utility.MapStringToDouble(dto.Latitude), Utility.MapStringToDouble(dto.Longitude));
            ImagePath = $"jakt_{ID}.jpg";
            ImageSource = Utility.GetImageSource(ImagePath);
            Notes = dto.Notes;
        }
    }
}
