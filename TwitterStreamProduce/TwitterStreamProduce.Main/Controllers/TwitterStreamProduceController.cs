using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterStreamProduce.SharedLibrary;
using TwitterStreamProduce.SharedLibrary.Models;

namespace TwitterStreamProduce.Main.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TwitterStreamProduceController : ControllerBase
    {
        private readonly ILogger<TwitterStreamProduceController> _logger;
        private readonly ILoggerFactory _loggery;

        public TwitterStreamProduceController(ILoggerFactory loggery)
        {
            _logger = loggery.CreateLogger<TwitterStreamProduceController>();
            _loggery = loggery;
        }

        [HttpGet]
        public ActionResult<List<OutputEntity>> Get()
        {
            _logger.LogInformation("Start : Getting stream summary in method RandomStreamController::Get()");

            List<OutputEntity> output = null;

            try
            {
                TwitterHistoryStreamProcess twitterHistoryStreamProcess = new TwitterHistoryStreamProcess(_loggery);
                var task = Task.Run(() => twitterHistoryStreamProcess.GetTwitterHistorySummary());
                output = task.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("RandomStreamController :: GetHistory() -- {0}", ex.Message));
            }

            _logger.LogInformation("Completed in method RandomStreamController::Get()");

            return output;
        }
    }
}

