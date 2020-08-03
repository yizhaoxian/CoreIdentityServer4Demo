using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Study.CoreApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            var setting = new JwtSettings();
            Configuration.Bind("JwtSettings", setting);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            #region Swagger 
            services.AddSwaggerGen(c =>
            {
                //添加 接口文档说明
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API",
                    Version = "v1",
                    Description = "Core Web Api 学习"
                });
                // 生成文档显示文字说明,第二个参数是设置controller的注释是否显示 
                c.IncludeXmlComments(Path.Combine(Directory.GetCurrentDirectory(), "Study.CoreApi.xml"), true);
                // 也可以添加Model层的xml文件名
                //var xmlModelPath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "Study.CoreApi.Models.xml"));
                //c.IncludeXmlComments(xmlModelPath);

                //所有接口都会 先走这个过滤器  添加httpHeader参数
                //c.OperationFilter<HttpHeaderOperation>();


                // 设置访问Token  
                var security = new OpenApiSecurityRequirement();
                security.Add(new OpenApiSecurityScheme()
                {
                    Name = JwtBearerDefaults.AuthenticationScheme,
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                }, new List<string> { });
                c.AddSecurityRequirement(security);
                c.AddSecurityDefinition(
                    name: JwtBearerDefaults.AuthenticationScheme,
                    securityScheme: new OpenApiSecurityScheme
                    {
                        /* 这里配置swagger 右上角 Authorize 点开的文字说明 */
                        Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）",
                        // jwt默认的参数名称
                        Name = "Authorization",
                        // jwt默认存放Authorization信息的位置(请求头中)
                        In = ParameterLocation.Header,
                        //Scheme= JwtBearerDefaults.AuthenticationScheme,
                        Type = SecuritySchemeType.ApiKey
                    }
                );
            });
            #endregion

            //策略授权
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", po => po.RequireRole("Admin"));
                options.AddPolicy("Client", policy => policy.RequireRole("Client"));
                options.AddPolicy("Systems", policy => policy.RequireRole("Admin", "System", "Client"));
            });

            //JWT 身份验证
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.SecretKey)),
                    ValidateAudience = true,
                    ValidAudience = setting.Audience,
                    ValidateIssuer = true,
                    ValidIssuer = setting.Issuer,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true,
                    RequireExpirationTime = true
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        //跳过所有默认的逻辑
                        context.HandleResponse();
                        var result = new
                        {
                            status = 401,
                            msg = "无效的Token",
                            err = "无效的Token"
                        };
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        context.Response.WriteAsync(JsonConvert.SerializeObject(result)); 
                        return Task.CompletedTask;
                    }
                };
            });

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
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            //这里注意 一定要在 UseMvc前面，顺序不可改变
            app.UseAuthentication();
            app.UseMvc();

            app.Run(async context =>
            {
                await context.Response.WriteAsync("This is Web API !");
            });
        }
    }
}
