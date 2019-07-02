using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HTCPlatform.ViewModel.Models.Product
{
    
    public class AddProductViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Describe { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        [DefaultValue(typeof(string),"19401638097022976")]
        public long CategoryId { get; set; }
        [Required]
        public string Logo { get; set; }
        [Required]
        public int Quantity { get; set; }
        public bool IsEnabled { get; set; }
    }
}
