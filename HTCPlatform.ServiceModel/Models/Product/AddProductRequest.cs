using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HTCPlatform.ServiceModel.Product
{
   public class AddProductRequest
    {       
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Descritbe { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]     
        [DefaultValue(19249880041680900)]                 
        public long CategoryId { get; set; } = 19249880041680900;
        [Required]
        public string Logo { get; set; }
        public bool IsEnabled { get; set; }
    }
}
