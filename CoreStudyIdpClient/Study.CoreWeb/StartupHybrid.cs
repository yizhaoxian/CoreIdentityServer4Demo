using System.IdentityModel.Tokens.Jwt;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Study.CoreWeb
{
    public class StartupHybrid
    {
        public StartupHybrid(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            //关闭了 JWT 身份信息类型映射
            //这样就允许 well-known 身份信息（比如，“sub” 和 “idp”）无干扰地流过。
            //这个身份信息类型映射的“清理”必须在调用 AddAuthentication()之前完成
            //区别可参考下面截图，
            //简单理解 
            //jwt 的 key 映射出来是 http://xxxxxxxxxxxxxxx
            //well-known 映射出来是 sub idp 这样简洁的字符
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //添加认证信息
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
            options =>
            {
                options.AccessDeniedPath = "/Authorization/NoPermission";
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
             {
                 //IdentityServer4 服务器地址
                 options.Authority = "http://localhost:5002";
                 options.ClientId = "mvc client Hybrid";
                 options.ClientSecret = "mvc secret Hybrid";
                 options.RequireHttpsMetadata = false;
                 options.SaveTokens = true;
                 options.ResponseType = OidcConstants.ResponseTypes.CodeIdToken;

                 //如果请求token 就必须再定义客户端的时候设置运行通过浏览器来返回AccessToken
                 //options.ResponseType = OidcConstants.ResponseTypes.CodeToken;
                 //options.ResponseType = OidcConstants.ResponseTypes.CodeIdTokenToken;

                 options.Scope.Clear();
                 options.Scope.Add("api1");
                 options.Scope.Add(OidcConstants.StandardScopes.OpenId);
                 options.Scope.Add(OidcConstants.StandardScopes.Email);
                 options.Scope.Add(OidcConstants.StandardScopes.Phone);
                 options.Scope.Add(OidcConstants.StandardScopes.Address);
                 options.Scope.Add(OidcConstants.StandardScopes.Profile);
                 options.Scope.Add(OidcConstants.StandardScopes.OfflineAccess);
                 options.Scope.Add("roles"); 

                 //去掉默认过滤的 claim，这样 User.Claims 里就会出现这个 claim
                 options.ClaimActions.Remove("nbf");

                 //增加过滤的 claim，这样 User.Claims 里就会删除这个 claim
                 options.ClaimActions.DeleteClaim("sid");

                 options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                 {
                     //映射 User.Name
                     NameClaimType = JwtClaimTypes.Name,
                     RoleClaimType = JwtClaimTypes.Role
                 };
             });

            #region 自定义权限
            /*
             * 如果想测试可取消注释 TestController.Bob() 
             * [Authorize(Policy = "bob2")][Authorize(Policy = "bob")]  
             */
            services.AddSingleton<IAuthorizationHandler, BobAuthorizationHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("bob", builder =>
                {
                    builder.RequireAuthenticatedUser();
                    builder.RequireClaim(JwtClaimTypes.FamilyName, "Smith", "Smith1", "Smith2", "Smith3");
                });
                options.AddPolicy("bob2", builder =>
                {
                    builder.Requirements.Add(new BobRequirement());
                });
            }); 
            #endregion

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();
            //写在 UseMvc() 前面
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
