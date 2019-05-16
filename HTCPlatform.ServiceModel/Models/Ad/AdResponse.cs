
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCPlatform.ServiceModel.Ad
{
   public class AdResponse : BaseModel
   {
       public string Title { get; set; }
       public string Descride { get; set; }
       public DateTime InactiveTime { get; set; }
       public DateTime ActiveTime { get; set; }
       public bool IsEnable { get; set; }
   }
}
