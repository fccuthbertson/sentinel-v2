using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace Api.Models.Polygon
{
    public class AggregatesRequest
    {
        public string Ticker { get; set; }
        public int Multiplier { get; set; }
        // minute OR hour
        public string Timespan { get; set; }
        // DateTime in format "YYYY-MM-DD"
        public DateTime From { get; set; }
        // DateTime in format "YYYY-MM-DD"
        public DateTime To { get; set; }
        // limited to asc OR desc
        public string Sort { get; set; }
        // Limits the number of base aggregates queried to
        // create the aggregate result
        // cf. https://polygon.io/blog/aggs-api-updates/
        public int Limit { get; set; }
    }

    public class AggregateResult
    {
        [JsonPropertyName("c")]
        public decimal Close { get; set; }
        [JsonPropertyName("h")]
        public decimal High { get; set; }
        [JsonPropertyName("l")]
        public decimal Low { get; set; }
        [JsonPropertyName("n")]
        public int Transactions { get; set; }
        [JsonPropertyName("o")]
        public decimal Open { get; set; }
        // The Unix Msec timestamp for the start of the aggregate window
        [JsonPropertyName("t")]
        public long Timestamp { get; set; }
        // The trading volume of the symbol in the given time period.
        [JsonPropertyName("v")]
        public float Volume { get; set; }
        // The volume weighted average price
        [JsonPropertyName("vw")]
        public decimal Vwap { get; set; }
    }
    
    public class AggregatesResponse
    {
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; }
        [JsonPropertyName("queryCount")]
        public int QueryCount { get; set; }
        [JsonPropertyName("resultsCount")]
        public int ResultsCount { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("results")]
        public IList<AggregateResult> Results { get; set; }
    }

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