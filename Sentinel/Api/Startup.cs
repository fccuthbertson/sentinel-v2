using System;
using Api.Clients.Polygon;
using Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpClient("polygon.io", c =>
            {
                c.BaseAddress = new Uri("https://api.polygon.io");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddSingleton<IPolygonClient, PolygonClient>();
            services.AddTransient<IStochasticService, StochasticService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            lifetime.ApplicationStopping.Register(() =>
            {
                var p = app.ApplicationServices.GetService<IPolygonClient>();
                p.CleanUp();
            });
        }
    }
}
