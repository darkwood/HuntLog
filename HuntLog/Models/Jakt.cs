using System;
using System.Collections.Generic;

namespace HuntLog.Models
{
    public class Jakt :BaseDto
    {  
        public string Sted { get; set; }
        public DateTime DatoFra = DateTime.Now;
        public DateTime DatoTil = DateTime.Now;
        public List<string> JegerIds = new List<string>();
        public List<string> DogIds = new List<string>();
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ImagePath => $"jakt_{ID}.jpg";
        public string Notes { get; set; }
    }
}
