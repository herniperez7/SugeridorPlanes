using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SugeridorDePlanes.Models.Usuarios;
using Telefonica.SugeridorDePlanes;
using Telefonica.SugeridorDePlanes.Code;
using Telefonica.SugeridorDePlanes.Models.ApiModels;

namespace SugeridorDePlanes
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
            services.AddMvc(options => options.EnableEndpointRouting = false).AddRazorRuntimeCompilation();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(3600);
            });
            services.AddAutoMapper(configuration =>
            {
                configuration.CreateMap<RecomendadorB2b, RecomendadorB2bModel>().ReverseMap();
            }, typeof(Startup));

            services.AddScoped<IManejoUsuario, ManejoUsuario>();
            services.AddScoped<ITelefonicaService, TelefonicaService>();
            services.AddSingleton<IClient>(_ => new Client("https://localhost:44310/"));



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                DeveloperExceptionPageOptions d = new DeveloperExceptionPageOptions
                {
                    SourceCodeLineCount = 2
                };
                app.UseDeveloperExceptionPage(d);
            }

            
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvcWithDefaultRoute();
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Entorno" + env.EnvironmentName);
            });

            
        }
    }
}
