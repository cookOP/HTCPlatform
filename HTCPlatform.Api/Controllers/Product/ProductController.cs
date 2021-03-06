﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using HTCPlatform.Api.Model;
using HTCPlatform.Service.Product;
using HTCPlatform.ServiceModel.Product;
using Microsoft.AspNetCore.Mvc;
using HTCPlatform.Common.Snowflake;
using Microsoft.AspNetCore.Authorization;
using HTCPlatform.ServiceModel.Validators.Models.Product;
using FluentValidation.AspNetCore;

namespace HTCPlatform.Api.Controllers.Product
{
    //[Authorize]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Route("GetProductList")]
        public async Task<ResultSuccess> GetProductListAsync(ProductRequest req)
        {
            req.PageSize = 20;
            req.PageIndex = 0;
            req.OrderBy = "Id Desc";
            var result = new ResultSuccess();
            result.Data = await _productService.GetProductListAsync(req);
            return result;
        }
        [HttpPost]
        [Route("Add")]
        public async Task<ResultSuccess> AddAsync(AddProductRequest req)
        {
            var result = new ResultSuccess();
            req.Id = Snowflake.NewID();          
            result.Code = await _productService.AddAsync(req);
            return result;
        }
        [HttpPost]
        [Route("Update")]
        public async Task<ResultSuccess> UpdateAsync(UpdateProductRequest req)
        {
            var result = new ResultSuccess();
            result.Code=await _productService.UpdateAsync(req);
            return result;
        }
        [HttpGet]
        [Route("GetProduct")]
        public async Task<ResultSuccess> GetProductAsync(long Id)
        {
            var result = new ResultSuccess();
            result.Data = await _productService.GetProductAsync(Id);
            return result;
        }
        [HttpGet]
        [Route("Delete")]
        public async Task<ResultSuccess> DeleteAsync(long Id)
        {
            var result = new ResultSuccess();
            result.Code=await _productService.DeleteAsync(Id);
            return result;
        }
    }
}