using GameBattleShip.Data;
using GameBattleShip.Helpers;
using GameBattleShip.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;

namespace GameBattleShip
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "BattleShip API", Version = "v1" });
            });
            services.AddMemoryCache();
            services.AddSingleton(Log.Logger);
            services.AddSingleton<IData, SqlData>();
            services.AddSingleton<IConnectionStringFactory, ConnectionStringFactory>();
            services.AddScoped<IBattleshipService, BattleshipService>();
            services.AddScoped<IRandomGenerator, RandomGenerator>();
            services.AddScoped<ICacheProvider, InMemoryCacheProvider>();
            services.AddScoped<IPositionService, PositionService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BattleShip API V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMvc();
            DatabaseRunner.RunAsync(Configuration).Wait();
        }
    }
}
