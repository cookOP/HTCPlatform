using System;
using System.Collections.Generic;
using System.Text;

namespace HTCPlatform.ServiceModel.Ad
{
   public class AdRequest:PageBase
    {
        public string Title { get; set; }
        public bool? IsEnable { get; set; }
        public DateTime? ActiveTime { get; set; }
        public DateTime? InacticeTime  { get; set; }

    }
}
