using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HTCPlatform.Domain.Models
{
   public class Product : BaseModel
    {
        public string Name { get; set; }
        public string Descride { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public bool IsEnable { get; set; }
        public long CategroyId { get; set; }
        public string Logo { get; set; }
        public long AdId { get; set; }
    }
}
