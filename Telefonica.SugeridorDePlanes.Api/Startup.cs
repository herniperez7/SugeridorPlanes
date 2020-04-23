using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telefonica.SugeridorDePlanes.BusinessLogic;
using Telefonica.SugeridorDePlanes.DataAccess;
using Telefonica.SugeridorDePlanes.DataAccess.Context;
using AutoMapper;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.Dto.Dto;
using Microsoft.OpenApi.Models;

namespace Telefonica.SugeridorDePlanes.Api
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


            services.AddAutoMapper(configuration =>
            {
                configuration.CreateMap<SugeridorClientes, SugeridorClientesDTO>().ReverseMap();
                configuration.CreateMap<RecomendadorB2b, RecomendadorB2bDTO>().ReverseMap();
                configuration.CreateMap<PlanesOfertaActual, PlanesOfertaActualDTO>().ReverseMap();
            }, typeof(Startup));

            services.AddDbContext<TelefonicaSugeridorDePlanesContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("TelefonicaConnectionString")));

            //Services
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<ISuggestorService, SuggestorService>();

            //Repositories
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<ISuggestorRepository, SuggestorRepository>();

             services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddSwaggerGen(config => {
                config.SwaggerDoc("V1", new OpenApiInfo { Title = "Telefonica Web Api", Version = "V1" });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/V1/swagger.json", "Telefonica Web Api");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
