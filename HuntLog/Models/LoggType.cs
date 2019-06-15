using System;
namespace HuntLog.Models
{
    public class LoggType : BaseDto
    {
        public string Key { get; set; }
        public string Navn { get; set; }
        public string Beskrivelse { get; set; }
        public int GroupId { get; set; }
    }
}
