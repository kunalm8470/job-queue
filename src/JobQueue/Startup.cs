using JobQueue.Interfaces;
using JobQueue.Models;
using JobQueue.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using System.Threading.Channels;

namespace JobQueue
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
            services.AddSingleton(Channel.CreateUnbounded<JobItem>(new UnboundedChannelOptions() { SingleReader = true }));
            services.AddSingleton(svc => svc.GetRequiredService<Channel<JobItem>>().Reader);
            services.AddSingleton(svc => svc.GetRequiredService<Channel<JobItem>>().Writer);
            services.AddSingleton(provider =>
            {
                return new ConcurrentDictionary<Guid, JobItem>();
            });

            services.AddTransient(typeof(IJobRepository<>), typeof(InMemoryRepository<>));
            services.AddTransient<IWorkerService, WorkerService>();
            services.AddHostedService<WorkerService>();

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/Error");

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
