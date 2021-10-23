using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TwitterStreamProduce.BackgroundWorker.BackgroundTasks;
using TwitterStreamProduce.BackgroundWorker.RabbitMQServices;
using TwitterStreamProduce.BackgroundWorker.TwitterStreamServices;
using TwitterStreamProduce.SharedLibrary.Models;

namespace TwitterStreamProduce.Main
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
            var tokenConfig = Configuration.GetSection("SecretKeyConfiguration").Get<SecretKeyConfiguration>();
            var streamConfig = Configuration.GetSection("TwitterStreamDataSettings").Get<TwitterStreamDataConifiguration>();

            services.AddControllers();

            services.AddSingleton<SecretKeyConfiguration>(tokenConfig);
            services.AddSingleton<TwitterStreamDataConifiguration>(streamConfig);

            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                    .AddJsonFile("appsettings.json").Build();

            var loggingOptions = config.GetSection("Log4NetCore").Get<Log4NetProviderOptions>();
            services.AddLogging(configure => configure.AddLog4Net(loggingOptions));

            services.AddMemoryCache();
            services.AddSingleton<IPublishService, PublishService>();

            services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
            services.AddScoped<ITwitterStreamService, TwitterStreamService>();

            services.AddCors(o => o.AddPolicy("TwitterSampleStreamPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
            }));

            services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);
            services.AddHostedService<ConsumeScopedServiceHostedService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(o => o.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("Default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
