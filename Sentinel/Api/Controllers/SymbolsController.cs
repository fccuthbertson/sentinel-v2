using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using Api.Models.Polygon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SymbolsController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILoggerProvider _l;
        private readonly string _apiKey = "yYLRCkyPkr1GQT8QUYDtezInj1hljzJI";

        private long toUnix(DateTime d)
        {
            return ((DateTimeOffset) d).ToUnixTimeMilliseconds();
        }
        
        public SymbolsController(ILoggerProvider l, IHttpClientFactory clientFactory)
        {
            _l = l;
            _clientFactory = clientFactory;
            var h = new Hello();
            Task.Run(() => h.Start());
        }
        
        // GET: api/Symbols
        [HttpGet]
        public async Task<AggregatesResponse> Get()
        {
            var r = new AggregatesRequest
            {
                Ticker = "AAPL",
                Multiplier = 1,
                From = new DateTime(2022, 07, 12),
                To = new DateTime(2022, 07, 13),
                Sort = "desc",
                Limit = 5000,
                Timespan = "hour"
            };
            var reqUri =
                $"v2/aggs/ticker/{r.Ticker}/range/{r.Multiplier}/{r.Timespan}/{toUnix(r.From)}/{toUnix(r.To)}?adjusted=true&sort={r.Sort}&limit={r.Limit}&apiKey={_apiKey}";
            var request = new HttpRequestMessage(HttpMethod.Get, reqUri);
            var client = _clientFactory.CreateClient("polygon.io");
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _l.CreateLogger("symbols").LogError("request failed");
                return new AggregatesResponse
                {
                    Status = "failed"
                };
            }

            await using var resStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<AggregatesResponse>(resStream);
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
    
    class Hello
    {
        WebSocket _websocket;
        public void Start(){
            _websocket = new WebSocket("wss://socket.polygon.io/stocks", sslProtocols: SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls);
            _websocket.Opened += websocket_Opened;
            _websocket.Error += websocket_Error;
            _websocket.Closed += websocket_Closed;
            _websocket.MessageReceived += websocket_MessageReceived;
            _websocket.Open();
            Console.ReadKey();
        }
        private void websocket_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Connected!");
            _websocket.Send("{\"action\":\"auth\",\"params\":\"yYLRCkyPkr1GQT8QUYDtezInj1hljzJI\"}");
            _websocket.Send("{\"action\":\"subscribe\",\"params\":\"AM.AAPL\"}");
        }
        private void websocket_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("WebSocket Error");
            Console.WriteLine(e.Exception.Message);
        }
        private void websocket_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Connection Closed...");
            // Add Reconnect logic... this.Start()
        }
        private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }}
