using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GateEntryAPIService.Services.Master.Country;
using GateEntryAPIService.Services.Master.Employee;
using GateEntryAPIService.Services.Master.NumberingSchemas;
using GateEntryAPIService.Services.Master.RoleMasterService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Project.ContextHelper;
using Project.Models;
using Project.Services.Master.State;

namespace Project
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
            // services.AddSingleton<DapperContext>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Project", Version = "v1" });
            });
            #region  Sql Connection Set Up
            var connectionString = Configuration.GetConnectionString("VisitorManagementCon");
            services.Configure<DbConnectionInfo>(settings => Configuration.GetSection("ConnectionStrings").Bind(settings));
            services.AddScoped<DbContextHelper>();
           /*
                services.Configure<DbConnectionInfo>(settings => Configuration.GetSection("ConnectionStrings").Bind(settings));

                services.AddDbContext<DbContextHelper>(opt =>
               {
                   opt.UseSqlServer(connectionString);
                   //    opt.UseLazyLoadingProxies(true);
                   opt.EnableSensitiveDataLogging();
                   opt.ConfigureWarnings(warnings =>
                       warnings.Ignore(CoreEventId.DetachedLazyLoadingWarning)
                       .Ignore(CoreEventId.LazyLoadOnDisposedContextWarning)
                       );
               });
            */
            services.AddControllers()
                              .AddNewtonsoftJson(options =>
                                  options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                              );
            services.AddScoped<IDapperContext, DapperContext>();
            // services.AddScoped<DbContextHelper>();
            services.AddScoped<IStateService, StateService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<INumberingSchemaService, NumberingSchemaService>();
            services.AddScoped<IRoleMasterService, RoleMasterService>();
            
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project v1"));
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
