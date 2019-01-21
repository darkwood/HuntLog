using System;
namespace HuntLog.Models
{
    public class Hunt
    {
        public string ID { get; set; }
        public DateTime DatoFra { get; set; }
        public DateTime DatoTil { get; set; }
        public string Sted { get; set; }
    }
}
