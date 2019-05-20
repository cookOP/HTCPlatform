using System;
using System.Collections.Generic;
using System.Text;

namespace HTCPlatform.Domain.Models
{
   public class Users :BaseModel
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsEoable { get; set; }
    }
}
