using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace HTCPlatform.ServiceModel.Category
{
   public class UpdateCategoryRequest
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }       
        public string Describe { get; set; }
    }
}
