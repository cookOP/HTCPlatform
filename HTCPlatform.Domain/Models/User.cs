using System;
using System.Collections.Generic;
using System.Text;

namespace HTCPlatform.Domain.Models
{
   public class User 
    {
        public long  Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsEoable { get; set; }
        public DateTime CreateTime  { get; set; }
    }
}
