using System;
using System.Collections.Generic;
using System.Text;

namespace HTCPlatform.ServiceModel.Ad
{
    public class UpdateAdRequest:AdAddRequest
    {
        public DateTime UpdateTime  { get; set; }
    }
}
