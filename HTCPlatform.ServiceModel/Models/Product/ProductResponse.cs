using System;
using System.Collections.Generic;
using System.Text;

namespace HTCPlatform.ServiceModel.Product
{
    public class ProductResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }        
        public string Describe { get; set; }
        public int Amount { get; set; }
        public  decimal Price { get; set; }
        public bool  IsEnabled { get; set; }
        public  string CategoryName { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set;}
        public  string Logo { get; set; }
    }
}
