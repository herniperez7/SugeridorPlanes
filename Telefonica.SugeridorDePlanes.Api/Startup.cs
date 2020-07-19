using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telefonica.SugeridorDePlanes.BusinessLogic;
using Telefonica.SugeridorDePlanes.DataAccess;
using Telefonica.SugeridorDePlanes.DataAccess.Context;
using AutoMapper;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.Dto.Dto;
using Microsoft.OpenApi.Models;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;
using Telefonica.SugeridorDePlanes.BusinessLogic.Services;
using Telefonica.SugeridorDePlanes.DataAccess.Interfaces;
using Telefonica.SugeridorDePlanes.DataAccess.Services;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.Users;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;

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
            services.AddCors();
            services.AddControllers();

            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings").GetSection("Secret").Value);
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


            services.AddAutoMapper(configuration =>
            {
                configuration.CreateMap<SuggestorClient, SuggestorClientDTO>().ReverseMap();
                configuration.CreateMap<SuggestorB2b, SuggestorB2bDTO>().ReverseMap();
                configuration.CreateMap<OfertPlan, OfertActualPlanDTO>().ReverseMap();
                configuration.CreateMap<DevicePymes, DevicePymesDTO>().ReverseMap();
                configuration.CreateMap<UserDTO, User>().ForMember(dest => dest.RolString, opt => opt.MapFrom(org => org.Rol)).ForMember(dest => dest.Rol, opt => opt.Ignore()).ReverseMap();
                configuration.CreateMap<Proposal, ProposalDTO>().ForMember(dest => dest.Documento, opt => opt.MapFrom(org => org.RutCliente))
                    .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(org => org.IdUsuario)).ForMember(dest => dest.Id, opt => opt.MapFrom(org => org.Id)).ReverseMap();
            }, typeof(Startup));

            services.AddDbContext<TelefonicaSugeridorDePlanesContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("TelefonicaConnectionString")));

            //Services
            services.AddScoped<IClientLogic, ClientLogic>();
            services.AddScoped<ISuggestorLogic, SuggestorLogic>();
            services.AddScoped<IEmailSenderLogic, EmailSenderLogic>();
            services.AddScoped<IPdfLogic, PdfLogic>();
            services.AddScoped<IProposalLogic, ProposalLogic>();
            services.AddScoped<IUserLogic, UserLogic>();
            services.AddScoped<ILogLogic, LogLogic>();

            //Repositories
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISuggestorRepository, SuggestorRepository>();
            services.AddScoped<IProposalRepository, ProposalRepository>();
            services.AddScoped<ILogRepository, LogRepository>();

            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddSwaggerGen(config =>
            {
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

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
