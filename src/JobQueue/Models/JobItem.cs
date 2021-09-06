using System;

namespace JobQueue.Models
{
    public class JobItem: BaseEntity
    {
        public JobStatus Status { get; set; }
        public TimeSpan Duration { get; set; }
        public int[] Input { get; set; }
        public int[] Output { get; set; }
    }

    [Flags]
    public enum JobStatus
    {
        Pending = 1,
        Completed = 2
    }
}
