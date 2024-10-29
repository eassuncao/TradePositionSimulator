public class CalculatorInputModel
{
    public double InitialBalance { get; set; }
    public double FinalBalance { get; set; }
    public double HighestLiquidationPrice { get; set; }
    public int MinimumIterations { get; set; }
    public int MaximumIterations { get; set; }
    public double LowestActualPurchasePrice { get; set; }
    public double InitialPrice { get; set; }
    public double FinalPrice { get; set; }

    // Initial Buy Percentage Range
    public double InitialBuyStart { get; set; }
    public double InitialBuyEnd { get; set; }
    public double InitialBuyStep { get; set; }

    // Leverage Range
    public double LeverageStart { get; set; }
    public double LeverageEnd { get; set; }
    public double LeverageStep { get; set; }

    // Buy Percentage Range
    public double BuyPercentageStart { get; set; }
    public double BuyPercentageEnd { get; set; }
    public double BuyPercentageStep { get; set; }

    // Sell Percentage Range
    public double SellPercentageStart { get; set; }
    public double SellPercentageEnd { get; set; }
    public double SellPercentageStep { get; set; }

    // Drawdown Percentage Range
    public double DrawdownStart { get; set; }
    public double DrawdownEnd { get; set; }
    public double DrawdownStep { get; set; }
}