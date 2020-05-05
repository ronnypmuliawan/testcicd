using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestCICDApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IAmazonS3 _client;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IAmazonS3 client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Content("test from test method");
        }

        [HttpGet("test2")]
        public IActionResult Test2()
        {
            return Content("test 2");
        }

        // test the IAM Role and C# SDK.
        [HttpGet("iamtest")]
        public async Task<IActionResult> IamTest([FromQuery] string bucketName, [FromQuery] string key)
        {
            try
            {
                var request = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = key
                };

                var response = await _client.GetObjectAsync(request);
                return File(response.ResponseStream, "image/png");
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError(e, e.Message);
                throw new ArgumentException($"An exception occured when retrieving object from AWS S3 with key name: {key}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw new ArgumentException($"An exception occured when retrieving object from AWS S3 with key name: {key}");
            }
        }
    }
}
