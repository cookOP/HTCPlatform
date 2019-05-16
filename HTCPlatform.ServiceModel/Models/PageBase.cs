using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HTCPlatform.ServiceModel
{
   public class PageBase
    {
        [Required]
        [DefaultValue(20)]
        public int PageSize { get; set; }
        [Required]
        [DefaultValue(0)]
        public int PageIndex { get; set; }
        [Required]
        [DefaultValue("Id DESC")]
        public string OrderBy { get; set; }
    }
}
