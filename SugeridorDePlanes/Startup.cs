using System;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telefonica.SugeridorDePlanes.Code;
using Telefonica.SugeridorDePlanes.Models.ApiModels;
using Telefonica.SugeridorDePlanes.Models.Users;

namespace Telefonica.SugeridorDePlanes
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
                configuration.CreateMap<SugeridorClientes, SugeridorClientesModel>().ReverseMap();
                configuration.CreateMap<PlanesOferta, PlanOfertaActualModel>().ReverseMap();
                configuration.CreateMap<PlanDefinitivolModel, PlanesOferta>().ReverseMap();                
                configuration.CreateMap<EquipoPymes, EquipoPymesModel>().ReverseMap();
            }, typeof(Startup));

            services.AddScoped<IUserManager, UserManager>();       
            services.AddSingleton<ITelefonicaService, TelefonicaService>();
            services.AddSingleton<IClient>(_ => new Client(Configuration["ClientId"]));

            services.AddSession();


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
            
        }
    }
}
