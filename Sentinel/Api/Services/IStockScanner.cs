using System.Threading.Tasks;

namespace Api.Services
{
    public interface IScannerParams
    {
        int TransactionCount { get; set; }
        int TradingVolume { get; set; }
    }
    
    /*
     * Scans all tickers for given criteria
     */
    public interface IStockScanner
    {
        Task Set(IScannerParams p);
        Task Start();
        Task Stop();
    }
}