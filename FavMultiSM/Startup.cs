using FavMultiSM.Api.Instagram;
using FavMultiSM.Api.Socials;
using FavMultiSM.Api.Telegram;
using FavMultiSM.Api.Users;
using FavMultiSM.Api.Users.Security;
using FavMultiSM.Models;
using FavMultiSM.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;

namespace FavMultiSM
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"passes.json", optional: true, reloadOnChange: true)
                .AddJsonFile("lang.json", optional: true, reloadOnChange: true)
                .Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddControllers();
            services.AddDbContext<AppDbContext>();
            services.AddSingleton<IVkApi>(sp =>
            {
                var api = new VkApi(services);
                api.Authorize(new ApiAuthParams { AccessToken = Configuration["vk:token"] });
                Console.WriteLine(api.Status);
                return api;
            });
            services.AddControllers().AddNewtonsoftJson();
            services.AddCustomServices<ISingletoneService>();
            services.AddCustomServices<ITrancientService>();
            services.AddHostedService<InstagramChecker>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FavMultiSM", Version = "v1" });
            });          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FavMultiSM v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
