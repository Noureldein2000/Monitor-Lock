using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MonitorDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly object _object = 1;

        //private static readonly Object obj = new Object();
        private ConcurrentDictionary<string, object> dictionary = new ConcurrentDictionary<string, object>();

        private readonly LockManager _lockManager = new LockManager();

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> GetWeatherForecast()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet("GetById")]
        public bool GetById(int id)
        {
            return ProcessNumber(id);

            //Boolean _lockTaken = false;
            //var obj = dictionary.GetOrAdd(id.ToString(), new object());
            //Monitor.Enter(obj, ref _lockTaken);

            //lock (obj)
            //{

            //    for (int i = 0; i < 5; i++)
            //    {
            //        Thread.Sleep(1000);
            //        Console.Write(i + ",");
            //    }
            //    _lockTaken = true;
            //}
            ////finally
            ////{
            ////    if (_lockTaken)
            ////    {
            ////        Monitor.Exit(_object);
            ////    }
            ////}
            //Console.WriteLine(_lockTaken.ToString());
            //return _lockTaken;
        }

        [HttpGet("GetById2")]
        public void GetById2(int id)
        {
            var obj = dictionary.GetOrAdd(id.ToString(), new object());
            lock (obj)
            {
                //enter your locking code..
            }
        }

        private bool ProcessNumber(int number)
        {
            if (_lockManager.IsNumberLocked(number))
            {
                // Handle the case when the number is locked
                Console.WriteLine($"Number {number} is currently locked.");
                return false;
            }

            // Try to lock the number
            if (_lockManager.TryLockNumber(number))
            {
                try
                {
                    // Perform your processing here
                    Console.WriteLine($"Processing number {number}...");
                    Thread.Sleep(2000); // Simulating work
                    return true;
                }
                finally
                {
                    // Always release the lock
                    _lockManager.ReleaseLock(number);
                }
            }
            else
            {
                // Could not acquire the lock
                Console.WriteLine($"Could not lock number {number} as it's already in use.");
                return false;
            }
        }

    }
}
