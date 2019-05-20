using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HTCPlatform.ServiceModel.Ad
{
   public class AdAddRequest
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Descride { get; set; }
        [Required]
        public DateTime InactiveTime { get; set; }
        [Required]
        public DateTime ActiveTime { get; set; }
        public bool IsEnable { get; set; }
        public long Id { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
