using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Api.Models.Polygon;
using Microsoft.Extensions.Logging;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace Api.Clients.Polygon
{
    public interface IPolygonClient
    {
        Task<AggregatesResponse> GetAggregates(AggregatesRequest request);
        void StreamAggregatesMinute(List<string> tickers);
        void StreamAggregatesSecond(List<string> tickers);
        void StreamTrades(List<string> tickers);
        void StreamQuotes(List<string> tickers);
        Task CleanUp();
    }

    public class PolygonClient : IPolygonClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private WebSocket _ws;
        private readonly ILogger _logger;
        private readonly string _apiKey;

        public PolygonClient(IHttpClientFactory clientFactory, ILoggerProvider lp)
        {
            _logger = lp.CreateLogger("polygonClient");
            _clientFactory = clientFactory;
            _apiKey = Environment.GetEnvironmentVariable("POLYGON_IO_API_KEY");
            // InitWs();
        }

        public async Task<AggregatesResponse> GetAggregates(AggregatesRequest r)
        {
            var reqUri =
                $"v2/aggs/ticker/{r.Ticker}/range/{r.Multiplier}/{r.Timespan}/{ToUnix(r.From)}/{ToUnix(r.To)}?adjusted=true&sort={r.Sort}&limit={r.Limit}&apiKey={_apiKey}";
            var request = new HttpRequestMessage(HttpMethod.Get, reqUri);
            var client = _clientFactory.CreateClient("polygon.io");
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("request failed");
                return new AggregatesResponse
                {
                    Status = "failed"
                };
            }

            await using var resStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<AggregatesResponse>(resStream);
            return result;
        }

        private void InitWs()
        {
            Task.Run(() =>
            {
                using var websocket = new WebSocket("wss://socket.polygon.io/stocks",
                    sslProtocols: SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls);
                websocket.Opened += OnOpen;
                websocket.Error += OnError;
                websocket.Closed += OnClosed;
                websocket.MessageReceived += OnMessage;
                _ws = websocket;
                _ws.Open();
                Console.ReadKey();
            });
        }

        public void StreamAggregatesMinute(List<string> tickers)
        {
            Subscribe("AM", tickers);
        }

        public void StreamAggregatesSecond(List<string> tickers)
        {
            Subscribe("A", tickers);
        }

        public void StreamTrades(List<string> tickers)
        {
            Subscribe("T", tickers);
        }

        public void StreamQuotes(List<string> tickers)
        {
            Subscribe("Q", tickers);
        }

        private void AuthenticateWs()
        {
            var auth = new WebSockets.Request
            {
                Action = "auth",
                Params = _apiKey
            };
            var authJson = JsonSerializer.Serialize(auth);
            Console.WriteLine(authJson);
            _ws.Send(authJson);
        }

        private void Subscribe(string type, List<string> tickers)
        {
            var tks = new StringBuilder();
            if (tickers.Count > 0)
            {
                tickers.ForEach(t =>
                {
                    tks.Append($"{t},");
                });
            }

            var tksVal = tks.ToString();
            if (tksVal == string.Empty)
            {
                tksVal = "*";
            }
            var subscribe = new WebSockets.Request
            {
                Action = "subscribe",
                Params = $"{type}.{tksVal}"
            };
            var subJson = JsonSerializer.Serialize(subscribe);
            Console.WriteLine(subJson); 
            _ws.Send(subJson);
        }

        private void OnMessage(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Message);
            // need to dispatch events by type
        }

        private void OnOpen(object sender, EventArgs e)
        {
                Console.WriteLine("Connected!");
                AuthenticateWs();
                StreamAggregatesSecond(new List<string>());
                StreamAggregatesMinute(new List<string>());
                StreamQuotes(new List<string>());
                StreamTrades(new List<string>());
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("WebSocket Error");
            Console.WriteLine(e.Exception.Message);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Console.WriteLine("Connection Closed...");
            _logger.LogWarning("trying to reconnect websocket");
            _ws.Open();
        }

        public Task CleanUp()
        {
            _ws.Close("app cleanup");
            return Task.CompletedTask;
        }
        private static long ToUnix(DateTime d)
        {
            return ((DateTimeOffset) d).ToUnixTimeMilliseconds();
        }
    }
}