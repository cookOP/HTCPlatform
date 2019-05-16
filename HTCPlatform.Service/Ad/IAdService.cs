using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HTCPlatform.ServiceModel.Ad;
using Microsoft.EntityFrameworkCore;

namespace HTCPlatform.Service.Ad
{
  public  interface IAdService
  {
      Task<IPagedList<AdResponse>> GetaAdListAsync(AdRequest req);

      Task<int> AddAsync(AdAddRequest req);

      Task<int> UpdateAsync(UpdateAdRequest req);

      Task<AdResponse> GetAdAsync(long Id);

      Task<int> DeleteAsync(long Id);

  }
}
