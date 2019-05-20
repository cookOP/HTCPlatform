using System;
using System.Collections.Generic;
using System.Text;

namespace HTCPlatform.Domain
{
   public class BaseModel
    {
        public long Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
