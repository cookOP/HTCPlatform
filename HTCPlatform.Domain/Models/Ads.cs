using System;
using System.Collections.Generic;
using System.Text;

namespace HTCPlatform.Domain.Models
{
    public class Ads : BaseModel
    {
        public string Title { get; set; }
        public string Descride { get; set; }
        public DateTime InactiveTime { get; set; }
        public DateTime ActiveTime { get; set; }
        public bool IsEoable { get; set; }
    }
}
