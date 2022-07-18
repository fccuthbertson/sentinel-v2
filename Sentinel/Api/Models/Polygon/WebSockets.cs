using System.Collections.Generic;
using System.Text.Json.Serialization;
using WebSocket4Net;

namespace Api.Models.Polygon
{
    public class WebSockets
    {
        public class Request
        {
            [JsonPropertyName("action")]
            public string Action { get; set; }
            [JsonPropertyName("params")]
            public dynamic Params { get; set; }
        }
        
        public interface IStreamConfig
        {
            // a null or empty list will subscribe to ALL ticker
            // aggregate events
            List<string> Tickers { get; set; }
            void OnData(object sender, MessageReceivedEventArgs e);
        }

        public class Result
        {
            [JsonPropertyName("ev")]
            public string EventType { get; set; }
            [JsonPropertyName("sym")]
            public string Ticker { get; set; }
            [JsonPropertyName("v")]
            public long Volume { get; set; }
            [JsonPropertyName("av")]
            public long TodaysAccVolume { get; set; }
            [JsonPropertyName("op")]
            public decimal TodaysOpen { get; set; }
            [JsonPropertyName("vw")]
            public decimal Vwap { get; set; }
            [JsonPropertyName("o")]
            public decimal Open { get; set; }
            [JsonPropertyName("c")]
            public decimal Close { get; set; }
            [JsonPropertyName("h")]
            public decimal High { get; set; }
            [JsonPropertyName("l")]
            public decimal Low { get; set; }
            [JsonPropertyName("a")]
            public decimal TodaysVwap { get; set; }
            [JsonPropertyName("z")]
            public long AvgTradeSize { get; set; }
            [JsonPropertyName("s")]
            public long Start { get; set; }
            [JsonPropertyName("e")]
            public long End { get; set; }
        }
    }
}