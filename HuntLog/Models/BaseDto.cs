using System;
namespace HuntLog.Models
{
    public class BaseDto
    {
        public string ID { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Changed { get; set; }
    }
}
