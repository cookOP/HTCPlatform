using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using HTCPlatform.Dapper.Parameters;
using HTCPlatform.Dapper.Repositories;
using HTCPlatform.ServiceModel.Ad;
using Microsoft.EntityFrameworkCore;

namespace HTCPlatform.Service.Ad
{
    public class AdService : IAdService
    {
        private readonly IDapperRepository _dapperRepository;
        public AdService(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }
        public async Task<int> AddAsync(AdAddRequest req)
        {
            var param = new DynamicParameters();
            param.Add("@Id", req.Id);
            param.Add("@Title", req.Title);
            param.Add("@Describe", req.Descride);
            param.Add("@InactiveTime", req.InactiveTime);
            param.Add("@ActiveTime", req.ActiveTime);
            param.Add("@CreateTime", req.CreateTime);
            var sql = @"INSERT Ad INTO(Id,Title,Descirbe,InactiveTime,ActiveTime,IsEnable,CreateTime)                    
                        VAULE(Id=@Id,Title=@Title,Descirbe=@Descirbe,InactiveTime=@InactiveTime,ActiveTime=@ActiveTime,IsEnable=@IsEnable,CreateTime=@CreateTime)";
            return await _dapperRepository.ExecuteAsync(sql, param);

        }

        public async Task<int> DeleteAsync(long Id)
        {
            return await _dapperRepository.ExecuteAsync("DELETE FROM Ad WHERE Id=@Id", Id);
        }

        public async Task<IPagedList<AdResponse>> GetaAdListAsync(AdRequest req)
        {
            var param = new List<Parameter>();
            if (!string.IsNullOrEmpty(req.Title))
                param.Add(new Parameter("Title", req.Title));

            if (req.ActiveTime.HasValue)
                param.Add(new Parameter("ActiveTime", req.ActiveTime.Value, OperateType.GreaterEqual));

            if (req.InacticeTime.HasValue)
                param.Add(new Parameter("InactiveTime", req.InacticeTime.Value, OperateType.LessEqual));

            if (req.IsEnable.HasValue)
                param.Add(new Parameter("IsEnable", req.IsEnable));

            return await _dapperRepository.GetPagedListQueryAsync<AdResponse>("Ad", "Id,Title,Describe,InactiveTime,ActiveTime,IsEnable,CreateTime,UpdateTime", req.PageIndex, req.PageSize, "CreateTime DESC", parameters: param.ToArray());
        }

        public async Task<AdResponse> GetAdAsync(long Id)
        {
            return await _dapperRepository.QueryFirstOrDefaultAsync<AdResponse>("Id", Id, "Ad");
        }

        public async Task<int> UpdateAsync(UpdateAdRequest req)
        {
            var param = new DynamicParameters();
            param.Add("@Id", req.Id);
            param.Add("@Title", req.Title);
            param.Add("@Describe", req.Descride);
            param.Add("@InactiveTime", req.InactiveTime);
            param.Add("@ActiveTime", req.ActiveTime);
            param.Add("@UpdateTime", req.UpdateTime);
            var sql = @"UPDATE Ad SET Id=@Id,Title=@Title,Descirbe=@Descirbe,InactiveTime=@InactiveTime,ActiveTime=@ActiveTime,IsEnable=@IsEnable,UpdateTime=@UpdateTime WHERE Id=@Id";
            return await _dapperRepository.ExecuteAsync(sql, param);
        }
    }
}
