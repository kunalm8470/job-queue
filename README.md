## Job Queue using C# System.Threading.Channels

This project is about a thread-safe concurrent Job Queue using [`System.Threading.Channels`](https://devblogs.microsoft.com/dotnet/an-introduction-to-system-threading-channels/) and [`Background Services`](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-5.0&tabs=visual-studio). 

This includes implementing the following three controller actions in the `JobsController`:

* An endpoint to which clients can post a list of numbers to be sorted. The endpoint returns immediately, without requiring clients to wait for the jobs to complete.
* Returns the current state of a specific job.
* Return the current state of all jobs (both pending and completed).

## Example requests

```shell
> curl --request GET "http://localhost:5000/api/Jobs"
[]

> curl --request POST --header "Content-Type: application/json" --data "[2, 3, 1, 5, 3, 1, -20, 2]" "http://localhost:5000/api/Jobs"
{
  "id":"fcdffff4-1017-410c-9aa9-44c04e6aac6f",
  "status":"Pending",
  "duration":null,
  "input":[2,3,1,5,3,1,-20,2],
  "output":null
}

> curl --request GET "http://localhost:5000/api/Jobs/fcdffff4-1017-410c-9aa9-44c04e6aac6f"
{
  "id":"fcdffff4-1017-410c-9aa9-44c04e6aac6f",
  "status":"Completed",
  "duration":"00:00:05.0134826",
  "input":[2,3,1,5,3,1,-20,2],
  "output":[-20,1,1,2,2,3,3,5]
}

> curl --request GET "http://localhost:5000/api/Jobs"
[{
  "id":"fcdffff4-1017-410c-9aa9-44c04e6aac6f",
  "status":"Completed",
  "duration":"00:00:05.0134826",
  "input":[2,3,1,5,3,1,-20,2],
  "output":[-20,1,1,2,2,3,3,5]
}]
```

Postman collection can be imported [`here`](https://www.getpostman.com/collections/9cc68430761013b656cb).