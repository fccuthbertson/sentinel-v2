using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Clients.Polygon;
using Api.Models.Polygon;
using Api.Models.TechnicalIndicators;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SymbolsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IPolygonClient _polygon;
        private readonly IStochasticService _stochastic;

        public SymbolsController(ILoggerProvider l, IPolygonClient polygon, IStochasticService stochastic)
        {
            _logger = l.CreateLogger("symbolsController");
            _polygon = polygon;
            _stochastic = stochastic;
        }

        // GET: api/Symbols
        [HttpGet]
        public async Task<StochasticResult> Get()
        {
            var target = new DateTime(2022, 07, 15, 11, 5, 0);
            var result = await _stochastic.Get("AAPL", Window.OneMinute, target);
            return result;
        }

        // GET: api/Symbols/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Symbols
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Symbols/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


    }
}