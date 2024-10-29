namespace TradePositionSimulator.Services
{
    public interface ICalculatorService
    {
        string CalculateOptimalSettings(CalculatorInputModel input);
        string RunSpecificConfiguration(double initialBuyPercentage, double leverage,
            double drawdown, double buyPercentage, double sellPercentage,
            CalculatorInputModel input);
    }

    public class ScenarioParameters
    {
        public double InitialBuyPercentage { get; set; }
        public double Leverage { get; set; }
        public double DrawdownPercentage { get; set; }
        public double BuyPercentage { get; set; }
        public double SellPercentage { get; set; }
    }
}

public class TradeRecord
    {
        public string Type { get; set; } = string.Empty;  // Initialize with empty string
        public double Price { get; set; }
        public double Units { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class ScenarioDetails
    {
        public List<TradeRecord> Trades { get; set; } = new();
        public PositionSummary Summary { get; set; } = new();  // Initialize with new instance
        public List<string> Events { get; set; } = new();
    }

    public class PositionSummary
    {
        public int TotalBuys { get; set; }
        public int TotalSells { get; set; }
        public double AverageEntryPrice { get; set; }
        public double FinalLeverage { get; set; }
        public double TotalInvestment { get; set; }
        public double FinalBalance { get; set; }
        public double ROI { get; set; }
    }