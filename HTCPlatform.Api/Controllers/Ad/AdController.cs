using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HTCPlatform.Api.Model;
using HTCPlatform.Service.Ad;
using Microsoft.AspNetCore.Mvc;
using HTCPlatform.ServiceModel.Ad;
using HTCPlatform.Common.Snowflake;

namespace HTCPlatform.Api.Controllers
{
    [Route("api/ad")]
    public class AdController : ControllerBase
    {
        private readonly IAdService _adService;
        public AdController(IAdService adService)
        {
            _adService = adService;
        }
        [HttpPost]
        [Route("GetAdList")]
        public async Task<ResultSuccess> GetAdListAsync(AdRequest req)
        {
            var result = new ResultSuccess();
            result.Data = await _adService.GetaAdListAsync(req);
            return result;
        }
        [HttpPost]
        [Route("Add")]
        public async Task<ResultSuccess> AddAsync(AdAddRequest req)
        {
            req.Id = Snowflake.NewID();
            var result = new ResultSuccess();
            result.Code=await _adService.AddAsync(req);
            return result;
        }
        [HttpPost]
        [Route("Update")]
        public async Task<ResultSuccess> UpdateAsync(UpdateAdRequest req)
        {
            var result = new ResultSuccess();
            result.Code=await _adService.UpdateAsync(req);
            return result;
        }
        [HttpGet]
        [Route("GetAd")]
        public async Task<ResultSuccess> GetAdAsync(long Id)
        {
            var result = new ResultSuccess();
            result.Data = await _adService.GetAdAsync(Id);
            return result;
        }
        [HttpGet]
        [Route("Delete")]
        public async Task<ResultSuccess> DeleteAsync(long Id)
        {
            var result = new ResultSuccess();
            result.Code=await _adService.DeleteAsync(Id);
            return result;
        }
    }
}