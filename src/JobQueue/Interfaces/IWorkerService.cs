using JobQueue.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobQueue.Interfaces
{
    public interface IWorkerService
    {
        Task AddJob(JobItem item);
        Task<JobItem> GetJob(Guid id);
        Task<IEnumerable<JobItem>> GetJobs();
    }
}
