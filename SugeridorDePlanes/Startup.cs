using System;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
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
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        private IWebHostEnvironment _env { get; }

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
                configuration.CreateMap<SuggestorB2b, SuggestorB2bModel>().ReverseMap();
                configuration.CreateMap<SuggestorClient, SuggestorClientModel>().ReverseMap();
                configuration.CreateMap<OfertPlan, OfertActualPlanModel>().ReverseMap();
                configuration.CreateMap<DefinitivePlanModel, OfertPlan>().ReverseMap();
                configuration.CreateMap<DevicePymes, DevicePymesModel>().ReverseMap();
            }, typeof(Startup));

            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<ITelefonicaService, TelefonicaService>();
            services.AddSingleton<IClient>(_ => new Client(Configuration["ClientId"]));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
             .AddCookie(options =>
               {
                  options.Cookie.HttpOnly = true;
                  //options.ExpireTimeSpan = TimeSpan.FromHours(12);  este valor permite que se eliminen las cookies de login despues de 12 horas
                  options.Cookie.SecurePolicy = _env.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
                  options.Cookie.SameSite = SameSiteMode.Lax;
                  options.Cookie.Name = "SugeridorCookies";
                  options.LoginPath = new PathString("/Login");
                  options.LogoutPath = new PathString("/Login");
                  options.AccessDeniedPath = new PathString("/Login");  
               });


            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.HttpOnly = HttpOnlyPolicy.None;
                options.Secure = _env.IsDevelopment()
                  ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
            });

            services.AddMvc(options => options.Filters.Add(new AuthorizeFilter()));

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

            app.UseCookiePolicy();
            app.UseAuthentication();


            app.UseStaticFiles();
            app.UseSession();
            app.UseMvcWithDefaultRoute();

        }
    }
}
