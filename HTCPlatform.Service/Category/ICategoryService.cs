using HTCPlatform.ServiceModel.Category;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HTCPlatform.Service.Category
{
   public interface ICategoryService
   {
       Task<IList<CategoryResponse>> GetCategoryListAsync();

       Task<int> AddAsync(AddCategoryRequest req);

       Task<int> UpdateAsync(UpdateCategoryRequest req);

       Task<CategoryResponse> GetCategoryAsync(long Id);

       Task<int> DeleteAsync(long Id);
   }
}
