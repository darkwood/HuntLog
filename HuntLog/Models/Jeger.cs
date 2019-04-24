using System;
namespace HuntLog.Models
{
    public class Jeger : BaseDto
    {
        public string Fornavn { get; set; }
        public string Etternavn { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsMe { get; set; }
        public string ImagePath => $"jeger_{ID}.jpg";
    }
}
