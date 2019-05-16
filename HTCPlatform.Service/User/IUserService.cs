using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HTCPlatform.ServiceModel.Models.User;

namespace HTCPlatform.Service.User
{
    public interface IUserService
    {
        Task<UserResponse> GetUser(UserRequest req);

    }
}
