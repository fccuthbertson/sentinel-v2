using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
}