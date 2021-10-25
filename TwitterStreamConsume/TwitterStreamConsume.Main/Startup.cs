using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TwitterStreamConsume.Main.BackgroundTasks;
using TwitterStreamConsume.Main.Hubs;
using TwitterStreamConsume.Main.RabbitMQServices;

namespace TwitterStreamConsume.Main
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
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                        .AddJsonFile("appsettings.json").Build();

            var loggingOptions = config.GetSection("Log4NetCore").Get<Log4NetProviderOptions>();
            services.AddLogging(configure => configure.AddLog4Net(loggingOptions));

            services.AddMemoryCache();
            services.AddSingleton<IConsumeService, ConsumeService>();

            services.AddScoped<IScopedProcessingService, ScopedProcessingService>();

            services.AddSingleton<MessageHub>();

            services.AddCors(o => o.AddPolicy("TwitterUIPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
            }));

            services.AddSignalR();

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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();
            //app.UseCors("TwitterUIPolicy");
            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });

            app.UseEndpoints(configure =>
            {
                configure.MapHub<MessageHub>("/messagehub");
            });
        }
    }
}
