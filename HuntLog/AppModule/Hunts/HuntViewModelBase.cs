using System;
using System.Collections.Generic;
using HuntLog.AppModule;
using HuntLog.Helpers;
using HuntLog.InputViews;
using HuntLog.Models;
using Xamarin.Forms;

namespace HuntLog.AppModule.Hunts
{
    public class HuntViewModelBase : ViewModelBase
    {
        protected Jakt _dto;

        public string Location { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public List<string> DogIds { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Notes { get; set; }

        public bool HasLocation => Latitude > 0 && Longitude > 0;

        public List<PickerItem> SelectedHuntersAndDogs { get; set; }

        protected void SetStateFromDto(Jakt dto)
        {
            _dto = dto;
            ID = dto.ID;
            Location = dto.Sted;
            DateFrom = dto.DatoFra;
            DateTo = dto.DatoTil;
            DogIds = dto.DogIds;
            Latitude = Utility.MapStringToDouble(dto.Latitude);
            Longitude = Utility.MapStringToDouble(dto.Longitude);
            ImagePath = dto.ImagePath;
            ImageSource = Utility.GetImageSource(dto.ImagePath);
            Notes = dto.Notes;
        }
    }
}
