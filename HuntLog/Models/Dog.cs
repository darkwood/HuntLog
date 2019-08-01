using System;
namespace HuntLog.Models
{
    public class Dog : BaseDto
    {
        public string Navn { get; set; }
        public string Rase { get; set; }
        public string Lisensnummer { get; set; }
        public string ImagePath => $"dog_{ID}.jpg";
    }
}
