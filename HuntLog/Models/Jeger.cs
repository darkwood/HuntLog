using System;
namespace HuntLog.Models
{
    public class Jeger
    {
        public string ID { get; set; }
        public string Fornavn { get; set; }
        public string Etternavn { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ImagePath { get; set; }
        public bool IsMe { get; set; }
    }
}
