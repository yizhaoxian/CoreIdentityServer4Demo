using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using Study.CoreWeb.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Study.CoreWeb.Controllers
{
    //[Authorize]
    public class TestController : Controller
    {
        /// <summary>
        /// 测试从服务端认证
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "System,Admin")]

        public async Task<IActionResult> Private()
        {
            //var user = User;
            //var aa = User.IsInRole("System");
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var idToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            var code = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.Code);

            var model = new HomeViewModel
            {
                Infos = new Dictionary<string, string>()
                {
                    {"AccessToken", accessToken },
                    {"IdToken", idToken },
                    {"RefreshToken", refreshToken },
                    {"Code", code } //code 是空 因为code 是一次性的
                }
            };
            return View(model);
        }

        [Authorize(Roles = "System")]
        public IActionResult System()
        {
            return Content("访问成功，测试角色访问");
        }

        //[Authorize(Policy = "bob")]
        //[Authorize(Policy = "bob2")]
        public IActionResult Bob()
        {
            return Content("访问成功，测试策略访问");
        }

        /// <summary>
        /// 测试请求API资源(api1)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SuiBian()
        {
            var accesstoken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            return await SuibianApi(accesstoken);
        }

        /// <summary>
        /// 测试请求API资源(api1)，刷新过期 AccessToken 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SuiBianRef()
        {
            var accesstoken = await GetNewAccessToken();
            return await SuibianApi(accesstoken);
        }

        private async Task<IActionResult> SuibianApi(string accesstoken)
        {
            if (string.IsNullOrEmpty(accesstoken))
            {
                return Json(new { msg = "accesstoken 获取失败" });
            }
            var client = new HttpClient();
            client.SetBearerToken(accesstoken);
            var httpResponse = await client.GetAsync("http://localhost:5001/api/suibian");
            var result = await httpResponse.Content.ReadAsStringAsync();
            if (!httpResponse.IsSuccessStatusCode)
            {
                return Json(new { msg = "请求 api1 失败。", error = result });
            }
            return Json(new
            {
                msg = "成功",
                data = JsonConvert.DeserializeObject(result)
            });
        }

        public async Task<string> GetNewAccessToken()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5002");
            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }
            /* 必传参数
             client_id=s6BhdRkqt3
            &client_secret=some_secret12345
            &grant_type=refresh_token
            &refresh_token=8xLOxBtZp8
            &scope=openid%20profile
             */
            //disco.TokenEndpoint
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            var newToken = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                RefreshToken = refreshToken,
                ClientId = "mvc client",
                ClientSecret = "mvc secret",
                GrantType = OpenIdConnectGrantTypes.RefreshToken,
                //scope 之间用空格隔开
                Scope = $"api1 {OidcConstants.StandardScopes.OpenId} {OidcConstants.StandardScopes.Email} {OidcConstants.StandardScopes.Address} {OidcConstants.StandardScopes.Phone} {OidcConstants.StandardScopes.Profile} {OidcConstants.StandardScopes.OfflineAccess}"
            });
            if (newToken.IsError)
            {
                throw new Exception(newToken.Error);
            }

            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var exp = DateTime.Now + TimeSpan.FromSeconds(newToken.ExpiresIn);
            var ttttttt = exp.ToString("o", CultureInfo.InvariantCulture);
            var tokenArr = new List<AuthenticationToken>()
            {
                new AuthenticationToken
                {
                     Name=OpenIdConnectParameterNames.AccessToken,
                     Value=newToken.AccessToken
                },
                new AuthenticationToken
                {
                     Name=OpenIdConnectParameterNames.IdToken,
                     Value=newToken.IdentityToken
                },
                new AuthenticationToken
                {
                     Name=OpenIdConnectParameterNames.RefreshToken,
                     Value=newToken.RefreshToken
                },
                new AuthenticationToken
                {
                     //Name=OpenIdConnectParameterNames.ExpiresIn,
                     Name="expires_in",
                     Value=exp.ToString("o",CultureInfo.InvariantCulture)
                }
            };
            //更新 Properties 里保存的 相关信息
            authenticateResult.Properties.StoreTokens(tokenArr);

            //重新登陆，刷新cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authenticateResult.Principal, authenticateResult.Properties);

            return newToken.AccessToken;
        }
    }
}
