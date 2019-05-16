using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HTCPlatform.ServiceModel.Product;
using Microsoft.EntityFrameworkCore;

namespace HTCPlatform.Service.Product
{
    public interface IProductService
    {
        Task<IPagedList<ProductResponse>> GetProductListAsync(ProductRequest req);

        Task<int> AddAsync(AddProductRequest req);

        Task<int> UpdateAsync(UpdateProductRequest req);

        Task<ProductResponse> GetProductAsync(long Id);

        Task<int> DeleteAsync(long Id);
    }
}
