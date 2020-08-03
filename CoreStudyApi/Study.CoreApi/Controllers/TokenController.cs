using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Study.IRepository;

namespace Study.CoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        JwtSettings setting;//Token 配置信息
        IMerchantRepository _merchantRepository;

        //IOptions<JwtSettings> 是在 appsetting.json 的配置项
        //需要提前在 StartUp.ConfigureServices注册 
        public TokenController(IOptions<JwtSettings> jwtSettings, IMerchantRepository merchantRepository)
        {
            setting = jwtSettings.Value;
            _merchantRepository = merchantRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string appid, string appsecret)
        {
            try
            {
                var merchants = await _merchantRepository.LoadAll();
                var merchant = merchants.FirstOrDefault(l => l.AppId == appid && l.AppSecret == appsecret);
                if (merchant == null)
                {
                    return new JsonResult(new
                    {
                        status = 400,
                        msg = "无效的用户",
                        token = string.Empty
                    }); ;
                }

                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, merchant.Name));
                var roles = merchant.Roles.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < roles.Length; i++)
                {
                    //添加用户的角色
                    claims.Add(new Claim(ClaimTypes.Role, roles[i]));
                }

                //SecretKey 必须>= 16位
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.SecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                //设置 token 生成元素
                var token = new JwtSecurityToken(
                    issuer: setting.Issuer,
                    audience: setting.Audience,
                    claims: claims,
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: creds);
                var result = new JwtSecurityTokenHandler().WriteToken(token);
                return new JsonResult(new
                {
                    status = 200,
                    msg = "成功",
                    token = result
                });
            }
            catch (Exception ex)
            { 
                return new JsonResult(new
                {
                    status = 500,
                    msg = "服务器开小差了",
                    err = ex.Message
                });
            }
        }
    }
}
/*
 var claims = new Claim[]
                {
                     new Claim(ClaimTypes.Name, "Lsl"),
                     new Claim(ClaimTypes.Role, "Admin,System"),
                   new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                 new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                 new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                 // 这个就是过期时间，目前是过期1000秒，可自定义，注意JWT有自己的缓冲过期时间
                 new Claim(JwtRegisteredClaimNames.Exp, $"{new DateTimeOffset(DateTime.Now.AddSeconds(1000)).ToUnixTimeSeconds()}"),
                 new Claim(JwtRegisteredClaimNames.Iss, setting.Issuer),
                 new Claim(JwtRegisteredClaimNames.Aud, setting.Audience),
                };
     */
