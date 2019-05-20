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
using HTCPlatform.Domain.Models;

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
                param.Add(new Parameter("a.Name", req.Name, OperateType.Like));
            if (req.CreateTime.HasValue)
                param.Add(new Parameter("a.UpdateTime", req.CreateTime.Value, OperateType.GreaterEqual));
            if (req.UpdateTime.HasValue)
                param.Add(new Parameter("a.UpdateTime", req.UpdateTime, OperateType.LessEqual));
            if ((req.IsEnbale.HasValue))
                param.Add(new Parameter("a.IsEnabled", req.IsEnbale.Value));

            return await _dapperRepository.GetPagedListQueryAsync<ProductResponse>("Products AS a INNER JOIN Categorys  AS b on a.CategoryId=b.Id ",
                "a.Id,a.[Name],a.Describe,a.Amount,a.Price,a.IsEnabled,b.[Name] AS CategoryName,a.CreateTime,a.UpdateTime,a.Logo,a.Quantity ", req.PageIndex, req.PageSize, req.OrderBy,parameters: param.ToArray());

        }

        public async Task<int> AddAsync(Products req)
        {
            var param=new DynamicParameters();
            param.Add("@Id", req.Id);
            param.Add("@Name",req.Name);
            param.Add("@Describe",req.Describe);
            param.Add("@Amount",req.Amount);
            param.Add("@Price",req.Price);
            param.Add("@IsEnabled",req.IsEnable);
            param.Add("@CategoryId",req.CategoryId);
            param.Add("@Logo",req.Logo);
            param.Add("@Quantity", req.Quantity);
            var sql = @"INSERT INTO [dbo].[Products]
                               ([Id]
                               ,[Name]
                               ,[Describe]
                               ,[Amount]
                               ,[Price]
                               ,[IsEnabled]
                               ,[CategoryId]
                               ,[CreateTime]                            
                               ,[Logo]
                               ,[Quantity])
                         VALUES
                               (@Id
                               ,@Name
                               ,@Describe
                               ,@Amount
                               ,@Price
                               ,@IsEnabled
                               ,@CategoryId
                               ,GETDATE()        
                               ,@Logo
                               ,@Quantity)";
            return  await _dapperRepository.ExecuteAsync(sql,param);
        }

        public async Task<int> UpdateAsync(Products req)
        {
            var param = new DynamicParameters();           
            param.Add("@Name", req.Name);
            param.Add("@Describe", req.Describe);
            param.Add("@Amount", req.Amount);
            param.Add("@Price", req.Price);
            param.Add("@IsEnabled", req.IsEnable);
            param.Add("@CategoryId", req.CategoryId);
            param.Add("@Logo", req.Logo);
            param.Add("@Quantity", req.Quantity);
            var sql = @"UPDATE Products SET Name=@Name,
                        Describe=@Describe,
                        Amount=@Amount,
                        Price=@Price,
                        IsEnabled=@IsEnabled,
                        CategoryId=@CategoryId,
                        UpdateTime=GETDATE(),
                        Logo=@Logo ,
                        Quantity=@Quantity
                        WHERE Id=@Id ";
            return await _dapperRepository.ExecuteAsync(sql, param);
        }

        public async Task<ProductResponse> GetProductAsync(long Id)
        {           
            return await _dapperRepository.QueryFirstOrDefaultAsync<ProductResponse>("Id",Id,"Products");
        }

        public async Task<int> DeleteAsync(long Id)
        {
            return await _dapperRepository.ExecuteAsync("delete from Products where Id=@Id", Id);
        }
    }
}
