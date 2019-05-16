using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HTCPlatform.Domain.Models
{
   public class Category
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long Parent { get; set; }
        public  string Descride { get; set; }
    }
}
