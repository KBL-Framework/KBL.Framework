using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KBL.Framework.BAL.Base.Extensions;
using KBL.Framework.BAL.Base.Services;
using KBL.Framework.BAL.Interfaces.Mappers;
using KBL.Framework.DAL.Base.Repositories;
using KBL.Framework.DAL.Interfaces.Repositories;
using KBL.Framework.DAL.Interfaces.UnitOfWork;
using KBL.Framework.TestApi.DAL.Repos;
using KBL.Framework.TestApi.DAL.UoW;
using KBL.Framework.TestApi.Services;
using KBL.Framework.TestApi.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KBL.Framework.TestApi
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddTransient<ITestUoW, TestUoW>();
            services.AddTransient<IUnitOfWork, TestUoW>();
            services.AddTransient<IUserQueryRepository, UserQueryRepository>();
            services.AddTransient<IUserServices, UserServices>();
            services.AddTransient<EntityHistoryServices, EntityHistoryServices>();

            services.AddTransient(typeof(IQueryRepository<>), typeof(GenericQueryRepository<>));
            services.AddTransient(typeof(ICrudRepository<>), typeof(GenericCrudRepository<>));
            services.AddTransient(typeof(IGenericMapperFactory<,,>), typeof(GenericMapperFactory<,,>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuditEntityValues(Configuration);
            app.UseMvc();
        }
    }
}
