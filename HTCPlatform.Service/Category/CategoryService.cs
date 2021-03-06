﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using HTCPlatform.Dapper.Parameters;
using HTCPlatform.Dapper.Repositories;
using HTCPlatform.ServiceModel.Category;

namespace HTCPlatform.Service.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly IDapperRepository _dapperRepository;

        public CategoryService(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<int> AddAsync(AddCategoryRequest req)
        {
            var param = new DynamicParameters();
            param.Add("@Id", req.Id);
            param.Add("@Name", req.Name);
            param.Add("@ParentId", req.ParentId);
            param.Add("@Describe", req.Describe);
            var sql = @"INSERT Category INTO(Id,Name,ParentId,Describe)
                        VALUE(@Id,@Name,@ParentId,@Describe)";
            return await _dapperRepository.ExecuteAsync(sql, param);
        }

        public async Task<int> DeleteAsync(long Id)
        {
            var sql = "DELETE  Category WHERE Id=@Id";
            return await _dapperRepository.ExecuteAsync(sql, Id);
        }

        public async Task<CategoryResponse> GetCategoryAsync(long Id)
        {
            return await _dapperRepository.QueryFirstOrDefaultAsync<CategoryResponse>("Id", Id, "Category");
        }

        public async Task<IList<CategoryResponse>> GetCategoryListAsync()
        {
            return await _dapperRepository.QueryAsync<CategoryResponse>("Id,Name,ParentId,Describe", "Category", orderBy: "Id");
        }

        public async Task<int> UpdateAsync(UpdateCategoryRequest req)
        {
            var param = new DynamicParameters();
            param.Add("@Id", req.Id);
            param.Add("@Name", req.Name);
            param.Add("@ParentId", req.ParentId);
            param.Add("@Describe", req.Describe);
            var sql = @"UPDATE Category SET Id=@Id,Name=@Name,ParentId=@ParentId,Describe=@Describe WHERE Id=@Id";
            return await _dapperRepository.ExecuteAsync(sql, param);
        }
    }
}
