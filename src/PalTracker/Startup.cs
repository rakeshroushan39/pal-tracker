using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
namespace PalTracker
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
            
            var message = Configuration.GetValue<string>("WELCOME_MESSAGE");
             if (string.IsNullOrEmpty(message))    
               {
               throw new ApplicationException("WELCOME_MESSAGE not configured.");
               }
             services.AddSingleton(sp => new WelcomeMessage(message));

             var Port = Configuration.GetValue<string>("PORT");
              var MemoryLimit = Configuration.GetValue<string>("MEMORY_LIMIT");
             var CfInstanceIndex = Configuration.GetValue<string>("CF_INSTANCE_INDEX");
             var CfInstanceAddr = Configuration.GetValue<string>("CF_INSTANCE_ADDR");

            //   if (string.IsNullOrEmpty(Port))    
            //    {
            //    throw new ApplicationException("CloudNative  not configured.");
            //    }
          services.AddSingleton(sp => new CloudFoundryInfo(Port,MemoryLimit,CfInstanceIndex,CfInstanceAddr));
            //services.AddSingleton<ITimeEntryRepository, InMemoryTimeEntryRepository>();
            services.AddScoped<ITimeEntryRepository, MySqlTimeEntryRepository>();
            services.AddDbContext<TimeEntryContext>(options => options.UseMySql(Configuration));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
        }
    }
}
