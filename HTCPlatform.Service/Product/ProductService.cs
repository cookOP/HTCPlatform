using HTCPlatform.ServiceModel.Product;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using HTCPlatform.Dapper.Parameters;
using HTCPlatform.Dapper.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HTCPlatform.Service.Product
{
    public class ProductService : IProductService
    {
        private readonly IDapperRepository _dapperRepository;
        public ProductService(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;

        }

        public async Task<IPagedList<ProductResponse>> GetProductListAsync(ProductRequest req)
        {
            var param = new List<Parameter>();
            if (!string.IsNullOrEmpty(req.Name))
                param.Add(new Parameter("Name", req.Name, OperateType.Like));
            if (req.CreateTime.HasValue)
                param.Add(new Parameter("UpdateTime", req.CreateTime.Value, OperateType.GreaterEqual));
            if (req.UpdateTime.HasValue)
                param.Add(new Parameter("UpdateTime", req.UpdateTime, OperateType.LessEqual));
            if ((req.IsEnbale.HasValue))
                param.Add(new Parameter("IsEnabled", req.IsEnbale.Value));

            return await _dapperRepository.GetPagedListQueryAsync<ProductResponse>("Product",
                "Id,Name,Describe,Amount,Price,IsEnabled,'' CategoryName,CreateTime,UpdateTime,Logo,Quantity", req.PageIndex, req.PageSize, req.OrderBy,parameters: param.ToArray());

        }

        public async Task<int> AddAsync(AddProductRequest req)
        {
            var param=new DynamicParameters();
            param.Add("@Id", req.Id);
            param.Add("@Name",req.Name);
            param.Add("@Describe",req.Descritbe);
            param.Add("@Amount",req.Amount);
            param.Add("@Price",req.Price);
            param.Add("@IsEnabled",req.IsEnabled);
            param.Add("@CategoryId",req.CategoryId);
            param.Add("@Logo",req.Logo);
            var sql = @"INSERT Product INTO(Id,Name,Describe,Amount,Price,IsEnabled,CategoryId,CreateTime,Logo) 
                        VALUE(@Id,@Name,@Describe,@Amount,@Price,@IsEnabled,@CategoryId,GETDATE(),@Logo)";
            return  await _dapperRepository.ExecuteAsync(sql,param);
        }

        public async Task<int> UpdateAsync(UpdateProductRequest req)
        {
            var param = new DynamicParameters();           
            param.Add("@Name", req.Name);
            param.Add("@Describe", req.Descritbe);
            param.Add("@Amount", req.Amount);
            param.Add("@Price", req.Price);
            param.Add("@IsEnabled", req.IsEnabled);
            param.Add("@CategoryId", req.CategoryId);
            param.Add("@Logo", req.Logo);
            var sql = @"UPDATE Product SET Name=@Name,
                        Describe=@Describe,
                        Amount=@Amount,
                        Price=@Price,
                        IsEnabled=@IsEnabled,
                        CategoryId=@CategoryId,
                        UpdateTime=GETDATE(),
                        Logo=@Logo 
                        WHERE Id=@Id ";
            return await _dapperRepository.ExecuteAsync(sql, param);
        }

        public async Task<ProductResponse> GetProductAsync(long Id)
        {           
            return await _dapperRepository.QueryFirstOrDefaultAsync<ProductResponse>("Id",Id,"Product");
        }

        public async Task<int> DeleteAsync(long Id)
        {
            return await _dapperRepository.ExecuteAsync("delete from Product where Id=@Id", Id);
        }
    }
}
