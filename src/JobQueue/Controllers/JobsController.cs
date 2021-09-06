using JobQueue.Interfaces;
using JobQueue.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobQueue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IWorkerService _workerService;

        public JobsController(IWorkerService workerService)
        {
            _workerService = workerService ?? throw new ArgumentNullException(nameof(workerService));
        }

        [HttpGet]
        public async Task<ActionResult<JobItem[]>> ListJobs()
        {
            return Ok(await _workerService.GetJobs());
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<JobItem>> GetJob(Guid id)
        {
            try
            {
                return Ok(await _workerService.GetJob(id));
            }
            catch (Exception ex) when (ex is KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob([FromBody] int[] values)
        {
            JobItem item = new()
            {
                Id = Guid.NewGuid(),
                Status = JobStatus.Pending,
                Input = values
            };

            await _workerService.AddJob(item);
            return AcceptedAtAction("GetJob", new { id = item.Id }, item);
        }
    }
}
