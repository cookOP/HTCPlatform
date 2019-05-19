using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HTCPlatform.Api.Model;
using HTCPlatform.Service.Ad;
using Microsoft.AspNetCore.Mvc;
using HTCPlatform.ServiceModel.Ad;
using HTCPlatform.Common.Snowflake;
using log4net;

namespace HTCPlatform.Api.Controllers
{
    [Route("api/ad")]
    public class AdController : ControllerBase
    {
        private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(AdController));
        private readonly IAdService _adService;
        public AdController(IAdService adService)
        {
            log.Info("构造记录信息");
            _adService = adService;
        }
        [HttpPost]
        [Route("GetAdList")]
        public async Task<ResultSuccess> GetAdListAsync(AdRequest req)
        {
            log.Info("构造记录信息");
            log.Debug("构造记录信息");
            log.Error("构造记录信息");
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