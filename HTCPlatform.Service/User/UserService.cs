using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using HTCPlatform.Dapper.Repositories;
using HTCPlatform.ServiceModel.Models.User;

namespace HTCPlatform.Service.User
{
    public class UserService : IUserService
    {
        private readonly IDapperRepository _dapperRepository;
        public UserService(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<UserResponse> GetUser(UserRequest req)
        {
            try
            {
                var sql = @"SELECT Id, Name UserName,IsEnable,CreateTime FROM  [dbo].[User]  WHERE  Name=@Name and Password=@Password";
                var dp = new DynamicParameters();
                dp.Add("@Name", req.UserName);
                dp.Add("@Password", req.Password);
             
                return await _dapperRepository.QueryFirstOrDefaultAsync<UserResponse>(sql, dp);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
          
        }
    }
}
