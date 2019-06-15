using System;
namespace HuntLog.Models
{
    public class Art : BaseDto
    {
        public string Navn { get; set; }
        public string Wikinavn { get; set; }
        public int GroupId { get; set; }
    }
}
