namespace TradePositionSimulator.Models
{
    public class ScenarioDetails
    {
        public List<TradeRecord> Trades { get; set; } = new();
        public PositionSummary Summary { get; set; } = new();
        public List<string> Events { get; set; } = new();
    }

    public class TradeRecord
    {
        public string Type { get; set; } = string.Empty;
        public double Price { get; set; }
        public double Units { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; } = string.Empty;
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
}