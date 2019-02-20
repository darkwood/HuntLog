using System;
namespace HuntLog.Models
{
    public class Jeger : BaseDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsMe { get; set; }
        public string ImagePath { get; set; }
    }
}
