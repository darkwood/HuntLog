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
        public string ID { get; set; }
        public string Location { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public List<string> HunterIds { get; set; }
        public List<string> DogIds { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ImagePath { get; set; }
        public ImageSource ImageSource { get; set; }
        public string Notes { get; set; }

        public bool HasLocation => Latitude > 0 && Longitude > 0;
        public bool IsNew => string.IsNullOrEmpty(ID);
        public string HuntersNames { get; set; }
        public string DogsNames { get; set; }
        public List<InputPickerItemViewModel> SelectedHunters { get; set; }

        protected void SetStateFromDto(Jakt dto)
        {
            ID = dto.ID;
            Location = dto.Sted;
            DateFrom = dto.DatoFra;
            DateTo = dto.DatoTil;
            HunterIds = dto.JegerIds;
            DogIds = dto.DogIds;
            Latitude = Utility.MapStringToDouble(dto.Latitude);
            Longitude = Utility.MapStringToDouble(dto.Longitude);
            ImagePath = dto.ImagePath;
            ImageSource = Utility.GetImageSource(dto.ImagePath);
            Notes = dto.Notes;
        }

        protected Jakt CreateHuntDto()
        {

            return new Jakt
            {
                ID = ID ?? Guid.NewGuid().ToString(),
                Sted = Location,
                DatoFra = DateFrom,
                DatoTil = DateTo,
                JegerIds = HunterIds,
                DogIds = DogIds,
                Latitude = Latitude.ToString(),
                Longitude = Longitude.ToString(),
                ImagePath = ImagePath,
                Notes = Notes
            };
        }
    }
}
