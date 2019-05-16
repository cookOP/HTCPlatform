using System;
using System.Collections.Generic;
using System.Text;

namespace HTCPlatform.ServiceModel.Category
{
   public class CategoryResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ParentId { get; set; }
        public string Describe { get; set; }
        public List<CategoryResponse> Children { get; set; }
    }
}
