using System;
namespace HuntLog.Models
{
    public class Jeger
    {
        public string ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsMe { get; set; }
        public string ImagePath { get; set; }
    }
}
