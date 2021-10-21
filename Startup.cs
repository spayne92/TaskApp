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
using BaseCoreAPI.Services;
using Microsoft.AspNetCore.Authentication;

namespace BaseCoreAPI
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _config.GetConnectionString("BaseConnectionString");
            var mySqlVersion = new MySqlServerVersion(new Version(8, 0, 25));
            
            var jwtTokenConfig = _config.GetSection("Tokens").Get<JwtTokenConfig>();
            services.AddSingleton(jwtTokenConfig);
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddAuthentication(x =>
            {
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddIdentityServerJwt();
                // .AddJwtBearer(x =>
                // {
                //     x.RequireHttpsMetadata = true;
                //     x.SaveToken = true;
                //     x.TokenValidationParameters = new TokenValidationParameters
                //     {
                //         ValidateIssuer = true,
                //         ValidIssuer = jwtTokenConfig.Issuer,
                //         ValidateIssuerSigningKey = true,
                //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Key)),
                //         ValidAudience = jwtTokenConfig.Audience,
                //         ValidateAudience = true,
                //         ValidateLifetime = true,
                //         ClockSkew = jwtTokenConfig.AccessTokenExpiration > 0 ? TimeSpan.FromMinutes(jwtTokenConfig.AccessTokenExpiration) : TimeSpan.FromMinutes(1)
                //     };
                // });

            services.AddDbContext<BaseContext>(cfg =>
            {
                cfg.UseMySql(connectionString, mySqlVersion);
            });

            services.AddDbContext<IdentityContext>(cfg =>
            {
                cfg.UseMySql(connectionString, mySqlVersion);
            });

            services.AddDefaultIdentity<User>()
                .AddEntityFrameworkStores<IdentityContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<User, IdentityContext>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseMySql(connectionString, mySqlVersion);
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseMySql(connectionString, mySqlVersion);
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 5000;
                })
                .AddSigningCredentials()
                .AddDeveloperSigningCredential();

            // DependencyInjection registration.
            services.AddTransient<BaseSeeder>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ITokenService, TokenService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization(); // Should be included in below IdentityServer, but isn't.
            app.UseIdentityServer();
            
            app.UseEndpoints(cfg =>
            {
                // Simple routing for API controllers. 
                cfg.MapControllers();
            });
        }
    }
}
