using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace RestApi_Scoring.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScoringController : Controller
    {
        private readonly ILogger<ScoringController> _logger;

        // Miliseconds
        private const int responseDelay = 1000;

        public ScoringController(ILogger<ScoringController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Mock сервис скоринга, который возвращает решение по заявке
        /// </summary>
        /// <param name="obj">Json-файл с информацией по кредитной заявке</param> 
        [HttpPost("evaluate")]
        public JsonResult Evaluate(Object obj)
        {
            JObject json = JObject.Parse(obj.ToString());
            var status = new Random().Next(2) == 1;

            _logger.LogInformation(">>>> ApplicationNum : " + json["ApplicationNum"] + "; status : " + status);

            return Json(new
            {
                ScoringStatus = status
            });
        }
    }
}