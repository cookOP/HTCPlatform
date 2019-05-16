using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HTCPlatform.Api.Model;
using HTCPlatform.Service.User;
using HTCPlatform.ServiceModel.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HTCPlatform.Api.Controllers.User
{
    
    
    public class UserController : Controller
    {

        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }
        [HttpPost]
        [Route("Login")]
        public async Task<ResultSuccess> Login(UserRequest req)
        {
            var result = new ResultSuccess();
            var user = await _userService.GetUser(req);
            if (user == null)
            {
                result.Code = 100;
                result.Message = "用户或者密码错误！";
                return result;
            }
            //验证用户名和密码
            var claims = new Claim[] { new Claim(ClaimTypes.Name, "John"), new Claim(JwtRegisteredClaimNames.Email, "john.doe@blinkingcaret.com") };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("C6A5C7C5-2F5B-4412-B5BB-56428F63759F"));
            var token = new JwtSecurityToken(
                issuer: "jwtIssuertest",
                audience: "jwtAudiencetest",
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddDays(28),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            result.Data = new JwtSecurityTokenHandler().WriteToken(token);//生成Token

            return result;
        }
    }
}