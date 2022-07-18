using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Clients.Polygon;
using Api.Models.Polygon;
using Api.Models.TechnicalIndicators;

namespace Api.Services
{
    /*
     * Uses Aggregate data from Polygon to calculate
     * the Stochastic Oscillator value for a given
     * window.
     * Windows supported are:
     * 1min
     * 2min
     * 5min
     * 15min
     * 30min
     * 60min
     */
    public interface IStochasticService
    {
        /*
          %K = (C - L14 / H14 - L14) x 100 
          C = The most recent closing price
          L14 = Lowest price traded for the 14 prev sessions
          H14 = Highest Price traded for the same 14 prev sessions
         */
        Task<StochasticResult> Get(string ticker, Window window, DateTime target);
    }

    public class StochasticService : IStochasticService
    {
        private readonly IPolygonClient _polygon;

        public StochasticService(IPolygonClient polygon)
        {
            _polygon = polygon;
        }

        /*
         * we can just get the data for the past hour and
         * calc the lower windows out of that.
         */
        public async Task<StochasticResult> Get(string ticker, Window window, DateTime target)
        {
            var s = await GetFor(ticker, Window.OneMinute, target);
            return new StochasticResult
            {
                Values = new List<StochasticOscillator>{s}
            };
        }
        
        // %K = (C - L14 / H14 - L14) x 100 
        private async Task<StochasticOscillator> GetFor(string ticker, Window w, DateTime target)
        {
            var window = (int) w;
            // calculate start since target is our end date
            var start = target.AddMinutes(-(window * 14));
            
            // fetch data
            var data = (await _polygon.GetAggregates(new AggregatesRequest
            {
                Ticker = ticker,
                Multiplier = (int)w,
                // minutes in a day; ensure we have enough data
                Limit = 1440,
                // we'll support days later
                Timespan = "minutes",
                From = start,
                To = target,
                Sort = "desc"
                
            })).Results.ToList();
            
            // get most recent closing price.
            var C = data.Take(1).First().Close;
            // gets us only the PREVIOUS sessions. If we include the current
            // session then the calc returns an incorrect value.
            var thisWindow = data.Skip(1).Take(window * 14).ToList();
            // aggregate data into 'w' periods
            var windowAgg = new List<List<AggregateResult>>();
            var counter = 0;
            while (counter < 14)
            {
                var local = thisWindow.Skip(counter * window).Take(window);
                windowAgg.Add(local.ToList());
                counter++;
            }

            var periodRes = new List<AggregateResult>();
            if (w != Window.OneMinute)
            {
                windowAgg.ForEach(list =>
                {
                    if (list.Count == 0)
                    {
                        return;
                    }
                    var agg = new AggregateResult();
                    agg.Close = list.Min(c => c.Close);
                    agg.High = list.Max(c => c.High);
                    periodRes.Add(agg);
                });
            }
            else
            {
                periodRes = thisWindow;
            }
            return CalcStoch(C, periodRes, w);
        }

        private StochasticOscillator CalcStoch(decimal C, List<AggregateResult> data, Window w)
        {
            // get lowest 
            var L14 = data.Min(c => c.Close);
            // get Highest 
            var H14 = data.Max(c => c.High);
            var K = (C - L14) / (H14 - L14) * 100;
            if (K < 0) K = 0;
            return new StochasticOscillator
            {
                Window = w,
                KValue = (float)K
            };
        }
    }
}