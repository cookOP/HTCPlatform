using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HTCPlatform.Api.Model;
using HTCPlatform.Common.Snowflake;
using HTCPlatform.Service.Category;
using HTCPlatform.ServiceModel.Category;
using Microsoft.AspNetCore.Mvc;

namespace HTCPlatform.Api.Controllers.Category
{
    [Route("api/category")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        [Route("GetCategoryTreeList")]
        public async Task<ResultSuccess> GetCategoryListAsync()
        {
            var result = new ResultSuccess();
            var list = await _categoryService.GetCategoryListAsync();           
            result.Data = GetCategoryList(0, list);
            return result;
        }

        private List<CategoryResponse> GetCategoryList(long parentId, IList<CategoryResponse> categorys)
        {
            if (categorys.Count <= 0) return new List<CategoryResponse>();

            var newCategorys = new List<CategoryResponse>();
            var list = categorys.Where(a => a.ParentId == parentId);

            foreach (var item in list)
            {

                var category = new CategoryResponse();
                category.Id = item.Id;
                category.Name = item.Name;
                category.Describe = item.Describe;
                category.Children = GetCategoryList(item.Id, categorys);
                newCategorys.Add(category);

            }
            return newCategorys;
        }
        [HttpPost]
        [Route("Add")]
        public async Task<ResultSuccess> AddAsync(AddCategoryRequest req)
        {
            req.Id = Snowflake.NewID();
            var result = new ResultSuccess();
            result.Code=await _categoryService.AddAsync(req);
            result.Data = req.Id;
            return result;
        }
        [HttpPost]
        [Route("Update")]
        public async Task<ResultSuccess> UpdateAsync(UpdateCategoryRequest req)
        {
            var result = new ResultSuccess();
            result.Code=await _categoryService.UpdateAsync(req);
            return result;

        }
        [HttpGet]
        [Route("GetCategory")]
        public async Task<ResultSuccess> GetCategoryAsync(long Id)
        {
            var result = new ResultSuccess();
            result.Data = await _categoryService.GetCategoryAsync(Id);
            return result;
        }
        [HttpGet]
        [Route("Delete")]
        public async Task<ResultSuccess> DeleteAsync(long Id)
        {
            var result = new ResultSuccess();
            result.Code=await _categoryService.DeleteAsync(Id);
            return result;
        }
    }
}