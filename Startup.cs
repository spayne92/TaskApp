using BaseCoreAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

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
            services.AddDbContext<BaseContext>(cfg =>
            {
                cfg.UseMySql(_config.GetConnectionString("BaseConnectionString"), new MySqlServerVersion(new Version(8, 0, 25)));
            });

            // DependencyInjection registration.
            services.AddTransient<BaseSeeder>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(cfg =>
            {
                // Simple routing for API controllers. 
                cfg.MapControllers();
            });
        }
    }
}
