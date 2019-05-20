using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HTCPlatform.Domain.Models
{
   public class Products : BaseModel
    {
        public string Name { get; set; }
        public string Describe { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public bool IsEnable { get; set; }
        public long CategoryId { get; set; }
        public string Logo { get; set; }
        public int Quantity { get; set; }
    }
}
