using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitSubscriber
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IRabbitConfig>(s => new RabbitConfig
            {
                HostName = Configuration.GetValue<string>("HostName"),
                Port = Configuration.GetValue<string>("Port"),
                UserName = Configuration.GetValue<string>("UserName"),
                Password = Configuration.GetValue<string>("Password"),
                ExchangeName = Configuration.GetValue<string>("ExchangeName"),
                QueueName = Configuration.GetValue<string>("QueueName"),
            });

            services.AddSingleton<IRabbitManipulation>(x => new RabbitManipulation(x.GetRequiredService<IRabbitConfig>(), true));
            services.AddHostedService<ConsumeRabbitMQHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("RabbitMQ demo. Messages would be written in to console");
                });
            });
        }
    }
}
