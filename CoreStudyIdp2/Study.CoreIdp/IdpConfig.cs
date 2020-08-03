using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Study.CoreIdp
{
    public class IdpConfig
    {
        /// <summary>
        /// 用户认证信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetApiResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResources.Email(),
                new IdentityResources.Phone(),
                new IdentityResource("roles","角色",new List<string>{ JwtClaimTypes.Role}),
            };
        }
        /// <summary>
        /// API 资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API",
                //给api1 User 传递 Claim
                    new List<string>{ "roles","address"})
                    {
                    //用户 客户端使用 AccessTokenType.Reference 验证
                        ApiSecrets = new List<Secret>
                                     {
                                        new Secret("api1 secret".Sha256())
                                     }
                    }
            };
        }

        /// <summary>
        /// 客户端应用
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    // 客户端ID 这个很重要
                    ClientId = "client",
                    //AccessToken 过期时间，默认3600秒，注意这里直接设置5秒过期是不管用的，解决方案继续看下面单独解答
                    //AccessTokenLifetime=5, 
                    // 没有交互性用户，使用 clientid/secret 实现认证。
                    AllowedGrantTypes = GrantTypes.ClientCredentials, 
                    // 用于认证的密码
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    // 客户端有权访问的范围（Scopes）
                    AllowedScopes = { "api1" }
                },
                new Client
                {
                    // 客户端ID 这个很重要
                    ClientId = "client pwd",  
                    //资源所有者密码授权客户端定义
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,  
                    // 用于认证的密码
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    // 客户端有权访问的范围（Scopes）
                    AllowedScopes = {
                        "api1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Phone
                    }
                },
                new Client{
                    ClientId="mvc client", //客户端Id
                    ClientName="测试客户端", //客户端名称 随便写
                    AllowedGrantTypes=GrantTypes.Code,//验证模式
                    ClientSecrets=
                    {
                        new Secret("mvc secret".Sha256()) //客户端验证密钥
                    },
                    // 授权以后 我们重定向的地址(客户端地址)，
                    // {客户端地址}/signin-oidc是系统默认的不用改，也可以改，这里就用默认的
                    RedirectUris = { "http://localhost:5000/signin-oidc" },
                    //注销重定向的url
                    PostLogoutRedirectUris = { "http://localhost:5000/signout-callback-oidc" },
                    //是否允许申请 Refresh Tokens
                    //参考地址 https://identityserver4.readthedocs.io/en/latest/topics/refresh_tokens.html
                    AllowOfflineAccess=true,
                    //将用户claims 写人到IdToken,客户端可以直接访问
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AccessTokenLifetime=10,
                    //客户端访问权限
                    AllowedScopes =
                    {
                        "api1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    }
                },
                new Client{
                    ClientId="mvc client implicit", //客户端Id
                    ClientName="测试客户端 Implicit", //客户端名称 随便写
                    //Implicit 模式 因为token 是通过浏览器发送给客户端的，这里必须启用
                   AllowAccessTokensViaBrowser=true,

                    AllowedGrantTypes=GrantTypes.Implicit,//验证模式 
                    RedirectUris = {
                        "http://localhost:5003/callback.html",
                         // AccessToken 有效期比较短，刷新 AccessToken 的页面
                        "http://localhost:5003/silentref.html",
                    },
                    //是否需要用户点击同意，这里需要设置为 false，不然客户端静默刷新不可用
                    RequireConsent=false,
                    AllowedCorsOrigins={ "http://localhost:5003" },
                    //注销重定向的url
                    PostLogoutRedirectUris = { "http://localhost:5003" },
                    AccessTokenLifetime=10, 
                    //客户端访问权限
                    AllowedScopes =
                    {
                        "api1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },
                new Client{
                    ClientId="mvc client Hybrid", //客户端Id
                    ClientName="测试客户端 Hybrid", //客户端名称 随便写 
                    ClientSecrets={ new Secret("mvc secret Hybrid".Sha256()) },

                    AllowedGrantTypes=GrantTypes.Hybrid,//验证模式 

                    // 如果客户端 response_type 包含 token 这里必须启用
                    //AllowAccessTokensViaBrowser=true,

                    RedirectUris = { "http://localhost:5003/signin-oidc" },  
                    //注销重定向的url
                    PostLogoutRedirectUris = { "http://localhost:5003/signout-callback-oidc" },

                    AllowOfflineAccess=true,
                    AlwaysIncludeUserClaimsInIdToken=true,
                      
                    //客户端访问权限
                    AllowedScopes =
                    {
                        "api1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                    }
                }
            };
        }
    }
}
/*
                    * 默认使用jwt验证 AccessTokenType.Jwt
                    * AccessTokenType.Reference 每次客户端请求资源都需要来服务端验证，这样可以随时撤销客户端的token有效性。但通信频繁
                    * 参考 https://www.cnblogs.com/irving/p/9357539.html 详细解释
                    */
//AccessTokenType=AccessTokenType.Reference,