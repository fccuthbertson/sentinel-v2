using System.Collections.Generic;

namespace Api.Models.TechnicalIndicators
{
    public enum Window
    {
        OneMinute = 1,
        TwoMinutes = 2,
        FiveMinutes = 5,
        FifteenMinutes = 15,
        ThirtyMinutes = 30,
        OneHour = 60
    }

    public class StochasticOscillator
    {
        public Window Window { get; set; }
        public float KValue { get; set; }
    }

    public class StochasticResult
    {
        public List<StochasticOscillator> Values { get; set; }
    }
}