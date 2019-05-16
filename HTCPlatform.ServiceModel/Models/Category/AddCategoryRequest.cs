using System;
using System.Collections.Generic;
using System.Text;

namespace HTCPlatform.ServiceModel.Category
{
   public class AddCategoryRequest
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Describe { get; set; }
        public long ParentId { get; set; }
    }
}
