using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HTCPlatform.ServiceModel.Product
{
   public class ProductRequest: PageBase
    {
       public string Name { get; set; }
       public DateTime? CreateTime { get;set; }
       public  DateTime? UpdateTime { get; set; }
       public bool? IsEnbale { get; set; }
    }
}
