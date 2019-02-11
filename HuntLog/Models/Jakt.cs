using System;
using System.Collections.Generic;

namespace HuntLog.Models
{
    public class Jakt
    {
        public string ID { get; set; }  
        public string Sted { get; set; }
        public DateTime DatoFra = DateTime.Now;
        public DateTime DatoTil = DateTime.Now;
        public List<int> JegerIds = new List<int>();
        public List<int> DogIds = new List<int>();
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ImagePath { get; set; }
        public string Notes { get; set; }
    }
}
