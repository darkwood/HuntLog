using System;
namespace HuntLog.Models 
{
    public class Logg
    {
        public string ID { get; set; }
        public int Treff { get; set; }
        public int Skudd { get; set; }
        public int Sett { get; set; }
        public DateTime Dato { get; set; }
        public string ArtId { get; set; }
        public string JegerId { get; set; }
        public string DogId { get; set; }
        public string JaktId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ImagePath { get; set; }
        public string Notes { get; set; }

        public string Gender { get; set; }
        public string Weather { get; set; }
        public string Age { get; set; }
        public string WeaponType { get; set; }
        public int Weight { get; set; }
        public int ButchWeight { get; set; }
        public int Tags { get; set; }
    }
}
