using JobQueue.Interfaces;
using JobQueue.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace JobQueue.Services
{
    public class WorkerService : BackgroundService, IWorkerService
    {
        private readonly ILogger<WorkerService> _logger;
        private readonly ChannelWriter<JobItem> _channelWriter;
        private readonly ChannelReader<JobItem> _channelReader;
        private readonly IJobRepository<JobItem> _jobService;
        private readonly Random _rnd = new();

        private const int _lowerBound = 10;
        private const int _upperBound = 21;
        
        public WorkerService(ILogger<WorkerService> logger,
            ChannelWriter<JobItem> channelWriter,
            ChannelReader<JobItem> channelReader,
            IJobRepository<JobItem> jobService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _channelWriter = channelWriter ?? throw new ArgumentNullException(nameof(channelWriter));
            _channelReader = channelReader ?? throw new ArgumentNullException(nameof(channelReader));
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
        }

        public async Task AddJob(JobItem item)
        {
            if (item == default(JobItem)) throw new ArgumentNullException(nameof(item));

            await _jobService.SaveAsync(item);
            await _channelWriter.WriteAsync(item);
        }

        public Task<JobItem> GetJob(Guid id)
        {
            return _jobService.FindByIdAsync(id);
        }

        public Task<IEnumerable<JobItem>> GetJobs()
        {
            return _jobService.ListAsync();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await foreach (JobItem item in _channelReader.ReadAllAsync(stoppingToken))
                {
                    await DoLongRunningWork(item, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Log an swallow as the while loop will end gracefully when cancellation has been requested
                _logger.Log(LogLevel.Information, new EventId(002, "OperationCancelledExceptionCaught"), "A task/operation cancelled exception was caught.");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, new EventId(001, "ExceptionCaught"), ex, "An exception occurred and was caught.");
            }
        }

        private async Task DoLongRunningWork(JobItem item, CancellationToken stoppingToken)
        {
            item.Output = new int[item.Input.Length];
            item.Input.CopyTo(item.Output, 0);

            int duration = _rnd.Next(_lowerBound, _upperBound);

            _logger.LogInformation("Start processing job with ID '{JobId}'.", item.Id);

            Stopwatch sw = new();
            sw.Start();
            await Task.Delay(TimeSpan.FromSeconds(duration), stoppingToken);
            Array.Sort(item.Output);
            sw.Stop();

            _logger.LogInformation("Completed processing job with ID '{JobId}'. Duration: '{Duration}'.", item.Id, sw.Elapsed);

            item.Status = JobStatus.Completed;
            item.Duration = sw.Elapsed;

            await _jobService.SaveAsync(item);
        }
    }
}
