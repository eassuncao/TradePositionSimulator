using TradePositionSimulator.Services;

namespace TradePositionSimulator.Services
{
    public class CalculatorService : ICalculatorService
    {
        // Keep your existing method as is
        public string CalculateOptimalSettings(CalculatorInputModel input)
        {
            var position = new Position();
            var buyer = new BuyOperations();
            var seller = new SellOperations();
            var compound = new CompoundOperations();
            var account = new Account(input.InitialBalance);
            var filter = new OptimalSettingsFinder(
                input.FinalBalance,
                input.HighestLiquidationPrice,
                input.MinimumIterations,
                input.MaximumIterations,
                input.LowestActualPurchasePrice
            );
            var results = filter.FindOptimalCombinedSettings(
                account, position, buyer, seller, compound,
                initialBuyPercentageRange: (input.InitialBuyStart, input.InitialBuyEnd, input.InitialBuyStep),
                leverageRange: (input.LeverageStart, input.LeverageEnd, input.LeverageStep),
                buyPercentageRange: (input.BuyPercentageStart, input.BuyPercentageEnd, input.BuyPercentageStep),
                sellPercentageRange: (input.SellPercentageStart, input.SellPercentageEnd, input.SellPercentageStep),
                drawdownPercentageRange: (input.DrawdownStart, input.DrawdownEnd, input.DrawdownStep),
                initialPurchasePrice: input.InitialPrice,
                finalSellPrice: input.FinalPrice
            );
            return results;
        }

        public string RunSpecificConfiguration(double initialBuyPercentage, double leverage,
    double drawdown, double buyPercentage, double sellPercentage,
    CalculatorInputModel input)
        {
            var logger = new SimulationLogger();

            var position = new Position(logger);  // Pass logger to Position
            var buyer = new BuyOperations(logger);
            var seller = new SellOperations(logger);
            var compound = new CompoundOperations(logger);
            var account = new Account(input.InitialBalance);

            // Calculate initial units to buy based on percentage
            var initialUnits = (input.InitialBalance * initialBuyPercentage);

            // Initial buy
            buyer.Buy(account, position, initialUnits, input.InitialPrice, leverage);
            position.DisplayPositionSummary(account);  // Display after initial buy

            // Modify CompoundOperations.PreventLiquidation to display position summary after each iteration
            compound.LoopThroughPreventLiquidation(
                position, account, seller, buyer,
                drawdown, sellPercentage, buyPercentage,
                input.MaximumIterations, input.LowestActualPurchasePrice);

            // Final sell
            seller.Sell(account, position, account.CurrentUnits, input.FinalPrice);
            position.DisplayPositionSummary(account);  // Display after final sell

            return logger.GetLogs();
        }
    }
}