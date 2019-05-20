using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System.Text;
using FluentValidation.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using FluentValidation;
using HTCPlatform.ServiceModel.Validators.Models.Product;
using HTCPlatform.ServiceModel.Product;
using System.Collections.Generic;
using System.Reflection;
using log4net.Repository;
using log4net.Config;
using log4net;
using HTCPlatform.Api.Filters;
using HTCPlatform.Dapper.Repositories;
using AutoMapper;
using HTCPlatform.ViewModel.Models.Product;
using HTCPlatform.ViewModel.Validators.Product;

namespace HTCPlatform.Api
{
    public class Startup
    {
        public static ILoggerRepository repository { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;           
            repository = LogManager.CreateRepository("NETCoreRepository");
            // 指定配置文件
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
        }

        
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //注入服务                  
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IAdService, AdService>();
            //services.AddScoped<ICategoryService, CategoryService>();
            //services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IDapperRepository, DapperRepository>();

            // #region 反射注入服务
            //集中注册服务
            foreach (var item in GetClassName("HTCPlatform.Service"))
            {
                foreach (var typeArray in item.Value)
                {
                    services.AddScoped(typeArray, item.Key);
                }
            }
            // #endregion 

            services.AddSingleton<IValidator<AddProductRequest>, AddProductRequestValidator>();

            // 添加验证器            
            var types = Assembly.Load("HTCPlatform.ViewModel").GetTypes().Where(p => p.BaseType.GetInterfaces().Any(x => x == typeof(IValidator)));

            foreach (var type in types)
            {
                var genericType = typeof(IValidator<>).MakeGenericType(type.BaseType.GenericTypeArguments[0]);
                services.AddSingleton(genericType, type);
            }

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddFluentValidation();
            //返回json数据区分大小写
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
               
            });
            services.AddMvc(options =>
            {
                options.Filters.Add<HttpGlobalExceptionFilter>(); //加入全局异常类
            });
            //AutoMapper 注入
            services.AddAutoMapper();

            //注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "平台管理API", Version = "v1" });
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                var xmlPath = Path.Combine(basePath, "HTCPlatform.Api.xml");
                c.IncludeXmlComments(xmlPath);
                //c.OperationFilter<AddRequiredHeaderParameter>();
            });
            // override modelstate
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (context) =>
                {
                    var errors = context.ModelState
                        .Values
                        .SelectMany(x => x.Errors
                            .Select(p => p.ErrorMessage))
                        .ToList();

                    var result = new
                    {
                        Code = "00009",
                        Message = "填写信息验证不通过！",
                        Errors = errors
                    };

                    return new BadRequestObjectResult(result);
                };
              
            });

            //配置授权
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";

            }).AddJwtBearer("JwtBearer",
                (jwtBearerOptions) =>
                {
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("C6A5C7C5-2F5B-4412-B5BB-56428F63759F")),//秘钥
                        ValidateIssuer = true,
                        ValidIssuer = "jwtIssuertest",
                        ValidateAudience = true,
                        ValidAudience = "jwtAudiencetest",
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };
                });
          
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();               
            }
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "平台管理API V1");
            });
        }
        /// <summary>  
        /// 获取程序集中的实现类对应的多个接口
        /// </summary>  
        /// <param name="assemblyName">程序集</param>
        public Dictionary<Type, Type[]> GetClassName(string assemblyName)
        {
            if (!string.IsNullOrEmpty(assemblyName))
            {
                Assembly assembly = Assembly.Load(assemblyName);
                List<Type> ts = assembly.GetTypes().ToList();

                var result = new Dictionary<Type, Type[]>();
                foreach (var item in ts.Where(s => !s.IsInterface))
                {
                    var interfaceType = item.GetInterfaces();
                    result.Add(item, interfaceType);
                }
                return result;
            }
            return new Dictionary<Type, Type[]>();
        }
    }
}
