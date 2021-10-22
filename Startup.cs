using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using BaseCoreAPI.Data;
using BaseCoreAPI.Data.Entities;
using BaseCoreAPI.Infrastructure;
using Microsoft.Extensions.Hosting;

namespace BaseCoreAPI
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var mySqlVersion = new MySqlServerVersion(new Version(8, 0, 25));
            var connectionString = _config.GetConnectionString("BaseConnectionString");
            var jwtTokenConfig = _config.GetSection("Tokens").Get<JwtTokenConfig>();

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddDbContext<BaseContext>(cfg =>
            {
                cfg.UseMySql(connectionString, mySqlVersion);
            });

            services.AddDbContext<IdentityContext>(cfg =>
            {
                cfg.UseMySql(connectionString, mySqlVersion);
            });

            services.AddIdentity<User, IdentityRole>()  
                .AddEntityFrameworkStores<IdentityContext>()  
                .AddDefaultTokenProviders();  

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtTokenConfig.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Key)),
                    ValidAudience = jwtTokenConfig.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = jwtTokenConfig.AccessTokenExpiration > 0 ? TimeSpan.FromMinutes(jwtTokenConfig.AccessTokenExpiration) : TimeSpan.FromMinutes(1)
                };
            });

            // DependencyInjection registration.
            services.AddTransient<BaseSeeder>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseHsts();
            }
            
            app.UseSession();
            app.Use(async (context, next) =>
            {
                var token = context.Session.GetString("Token");
                if (!string.IsNullOrEmpty(token))
                {
                    // Adds token to HTTP header if it exists in the session.
                    context.Request.Headers.Add("Authorization", "Bearer " + token);
                }
                await next();
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(cfg =>
            {
                // Simple routing for API controllers. 
                cfg.MapControllers();
            });
        }
    }
}
