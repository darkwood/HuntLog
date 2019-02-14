using System;
using System.Collections.Generic;
using HuntLog.Models;
using Xamarin.Forms;

namespace HuntLog.ViewModels.Hunts
{
    public class HuntViewModelBase : ViewModelBase
    {
        public string ID { get; set; }
        public string Location { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public List<int> Hunters { get; set; }
        public List<int> Dogs { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ImagePath { get; set; }
        public ImageSource ImageSource { get; set; }
        public string Notes { get; set; }

        public bool HasLocation => Latitude > 0 && Longitude > 0;
        public bool IsNew => string.IsNullOrEmpty(ID);

        protected void SetStateFromDto(Jakt dto)
        {
            ID = dto.ID;
            Location = dto.Sted;
            DateFrom = dto.DatoFra;
            DateTo = dto.DatoTil;
            Hunters = dto.JegerIds;
            Dogs = dto.DogIds;
            Latitude = MapStringToDouble(dto.Latitude);
            Longitude = MapStringToDouble(dto.Longitude);
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
                JegerIds = Hunters,
                DogIds = Dogs,
                Latitude = Latitude.ToString(),
                Longitude = Longitude.ToString(),
                ImagePath = ImagePath,
                Notes = Notes
            };
        }

        private double MapStringToDouble(string doubleString)
        {
            double value;
            double.TryParse(doubleString?.Replace('.', ','), out value);
            return value;
        }
    }
}
