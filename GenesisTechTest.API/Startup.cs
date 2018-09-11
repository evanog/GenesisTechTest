using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AutoMapper;
using GenesisTechTest.Common.Models;
using GenesisTechTest.DataAccess.Interfaces;
using GenesisTechTest.DataAccess.Repository;
using GenesisTechTest.Domain.Interfaces;
using GenesisTechTest.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace GenesisTechTest.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            var appSettingsSection = Configuration.GetSection("AppConfig");
            services.Configure<AppConfig>(appSettingsSection);
            services.AddApiVersioning();
            services.AddMemoryCache();

            ConfigureJWT(services, appSettingsSection);
            ConfigureSwagger(services);
            ConfigureRepoDependencies(services);
            ConfigureServiceDependencies(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Genesis Tech Test API");
            });
        }

        private void ConfigureJWT(IServiceCollection services, IConfigurationSection configurationSection)
        {
            var appConfig = configurationSection.Get<AppConfig>();

            var key = Encoding.ASCII.GetBytes(appConfig.JWTSecret);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            // Enable Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Version 1",
                    Version = "v1",
                });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Please enter JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    { "Bearer", Enumerable.Empty<string>() },
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                var xmlPath = Path.Combine("XMLDocumentation", xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        private void ConfigureRepoDependencies(IServiceCollection services)
        {
            services.AddSingleton<IStorageRepository, SimpleJSONFileStorage>();
        }

        private void ConfigureServiceDependencies(IServiceCollection services)
        {
            services.AddSingleton<IPasswordHashService, PasswordHashService>();
            services.AddSingleton<IIdentityService, IdentityService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IValidationService, ValidationService>();
        }
    }
}
