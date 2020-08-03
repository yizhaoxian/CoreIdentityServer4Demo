using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Study.CoreApi
{
    public class StartupImplicit
    {
        public IConfiguration Configuration { get; }
        public StartupImplicit(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
             
            services.AddCors(options =>
            {
                options.AddPolicy("client1", policy =>
                {
                    //客户端地址
                    policy.WithOrigins("http://localhost:5003");
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                });
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                // IdentityServer 地址
                options.Authority = "http://localhost:5002";
                //不需要https
                options.RequireHttpsMetadata = false;
                //这里要和 IdentityServer 定义的 api1 保持一致
                options.Audience = "api1";
                //token 默认容忍5分钟过期时间偏移，这里设置为0，
                //这里就是为什么定义客户端设置了过期时间为5秒，过期后仍可以访问数据
                options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
                options.Events = new JwtBearerEvents
                {
                    //AccessToken 验证失败
                    OnChallenge = op =>
                    {
                        //跳过所有默认操作
                        op.HandleResponse();
                        //下面是自定义返回消息
                        //op.Response.Headers.Add("token", "401");
                        op.Response.ContentType = "application/json";
                        op.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        op.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            status = StatusCodes.Status401Unauthorized,
                            msg = "token无效",
                            error = op.Error
                        }));
                        return Task.CompletedTask;
                    }
                };
            });

            #region Autofac
            // Create a container-builder and register dependencies 
            var builder = new ContainerBuilder();

            // Populate the service-descriptors added to `IServiceCollection`
            // BEFORE you add things to Autofac so that the Autofac
            // registrations can override stuff in the `IServiceCollection`
            // as needed
            builder.Populate(services);

            // Register your own things directly with Autofac
            builder.RegisterModule(new RegisterServices());
            //builder.RegisterType<EmployeRepository>().As<IEmployeeRepository>();
            var container = builder.Build();
            return new AutofacServiceProvider(container);
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            //这里注意 一定要在 UseMvc前面，顺序不可改变
            app.UseAuthentication();
            app.UseCors("client1");
            app.UseMvcWithDefaultRoute();
        }
    }
}
