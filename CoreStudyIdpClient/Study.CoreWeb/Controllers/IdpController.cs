using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Study.CoreWeb.Controllers
{
    public class IdpController : Controller
    {
        //内存缓存 需要提前注册  services.AddMemoryCache();
        private IMemoryCache _memoryCache;
        private static readonly string _idpBaseUrl = "http://localhost:5002";
        private static readonly string _apiBaseUrl = "http://localhost:5001";
        public IdpController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        
        #region 客户端授权模式
        
        public async Task<IActionResult> Token()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(_idpBaseUrl);
            if (disco.IsError)
            {
                return Content("获取发现文档失败。error：" + disco.Error);
            }
            var token = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret",
                Scope = "api1"
            });
            if (token.IsError)
            {
                return Content("获取 AccessToken 失败。error：" + disco.Error);
            }
            //将token 临时存储到 缓存中
            _memoryCache.Set("AccessToken", token.AccessToken);
            return Content("获取 AccessToken 成功。Token:" + token.AccessToken);
        }

        public async Task<IActionResult> SuiBian()
        {
            string token, apiurl = GetApiUrl("suibian");
            _memoryCache.TryGetValue("AccessToken", out token);
            if (string.IsNullOrEmpty(token))
            {
                return Content("token is null");
            }
            var client = new HttpClient();
            client.SetBearerToken(token);
            var response = await client.GetAsync(apiurl);
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                _memoryCache.Remove("AccessToken");
                return Content($"获取 {apiurl} 失败。StatusCode：{response.StatusCode} \r\n Token:{token} \r\n result:{result}");
            }
            return Json(new
            {
                code = response.StatusCode,
                data = result
            });
        }
        #endregion

        #region 账号密码授权模式

        public async Task<IActionResult> TokenPwd()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(_idpBaseUrl);
            if (disco.IsError)
            {
                return Content("获取发现文档失败。error：" + disco.Error);
            }

            #region 第一种方式请求 token
            //var tokenclient = new TokenClient(client, new TokenClientOptions
            //{
            //    ClientId = "client pwd",
            //    ClientSecret = "secret",
            //    Address = disco.TokenEndpoint,
            //});
            //var token = await tokenclient.RequestPasswordTokenAsync("alice", "alice", "api1"); 
            #endregion

            var token = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                //下面2个属性对应的是 IdentityServer定义的测试用户，这里应是 Action 参数传递进来的，为了方便直接写死的
                UserName = "alice",
                Password = "alice",
                //下面3个属性对应的是 IdentityServer定义的客户端
                ClientId = "client pwd",
                ClientSecret = "secret",
                Scope = "api1 openid profile email phone address"
            });
            if (token.IsError)
            {
                return Content("获取 AccessToken 失败。error：" + token.Error);
            }
            //将token 临时存储到 缓存中
            _memoryCache.Set("AccessToken_Pwd", token.AccessToken);
            return Content("获取 AccessToken 成功。Token:" + token.AccessToken);
        }

        public async Task<IActionResult> SuiBianPwd()
        {
            string token, apiurl = GetApiUrl("suibian");
            _memoryCache.TryGetValue("AccessToken_Pwd", out token);
            if (string.IsNullOrEmpty(token))
            {
                return Content("token is null");
            }
            var client = new HttpClient();
            client.SetBearerToken(token);
            var response = await client.GetAsync(apiurl);
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                _memoryCache.Remove("AccessToken");
                return Content($"获取 {apiurl} 失败。StatusCode：{response.StatusCode} \r\n Token:{token} \r\n result:{result}");
            }
            return Json(JsonConvert.DeserializeObject(result));
        }

        public async Task<IActionResult> IdentityInfoPwd()
        {
            string token;
            _memoryCache.TryGetValue("AccessToken_Pwd", out token);
            if (string.IsNullOrEmpty(token))
            {
                return Content("token is null");
            }

            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(_idpBaseUrl);
            if (disco.IsError)
            {
                return Content("获取发现文档失败。error：" + disco.Error);
            }

            client.SetBearerToken(token);
            var response = await client.GetAsync(disco.UserInfoEndpoint);
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                _memoryCache.Remove("AccessToken");
                return Content($"获取 UserInfo 失败。StatusCode：{response.StatusCode} \r\n Token:{token} \r\n result:{result}");
            }
            return Json(JsonConvert.DeserializeObject(result));
        }

        #endregion


        private string GetApiUrl(string address)
        {
            return _apiBaseUrl + "/api/" + address;
        }
    }
}