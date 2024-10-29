using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace TradePositionSimulator
{
    public class OptimalSettingsFinder
    {
        private double _originalCashBalance;
        private double _originalCurrentUnits;
        private double _originalMoneyInvested;
        private double _originalTotalMoneyInvested;
        private double _originalLiquidationPrice;
        private double _originalAveragePrice;

        private double _initialBuyPercentage = 1;
        private double _drawdown = 0.9;
        private double _buyPercentage = 1;
        private double _sellPercentage = 0;
        private double _leverage;

        private StringBuilder _allResults = new();

        public double FilteredFinalCashBalance { get; set; }
        public double FilteredHighestLiquidationPrice { get; set; }
        public int FilteredMinimumIterations { get; set; }
        public int FilteredMaximumIterations { get; set; }
        public double FilteredLowestActualPurchasePrice { get; set; }

        public OptimalSettingsFinder()
        {

        }
        
        public OptimalSettingsFinder(double finalCashBalance, double highestLiquidationPrice, int minimumIterations, int maximumIterations, double lowestActualPurchasePrice)
        {
            FilteredFinalCashBalance = finalCashBalance;
            FilteredHighestLiquidationPrice = highestLiquidationPrice;
            FilteredMinimumIterations = minimumIterations;
            FilteredMaximumIterations = maximumIterations;
            FilteredLowestActualPurchasePrice = lowestActualPurchasePrice;
        }

        public string FindOptimalCombinedSettings(
            Account account,
            Position position,
            BuyOperations buyer,
            SellOperations seller,
            CompoundOperations compound,
            (double start, double end, double step) initialBuyPercentageRange,
            (double start, double end, double step) leverageRange,
            (double start, double end, double step) buyPercentageRange,
            (double start, double end, double step) sellPercentageRange,
            (double start, double end, double step) drawdownPercentageRange,
            double initialPurchasePrice, double finalSellPrice)
        {
            _allResults.Clear();

            StoreOriginalValues(account, position);

            // Nested loops for each parameter
            for (_initialBuyPercentage = initialBuyPercentageRange.start;
                 _initialBuyPercentage <= initialBuyPercentageRange.end;
                 _initialBuyPercentage += initialBuyPercentageRange.step)
            {
                for (_leverage = leverageRange.start;
                     _leverage <= leverageRange.end;
                     _leverage += leverageRange.step)
                {
                    for (_buyPercentage = buyPercentageRange.start;
                         _buyPercentage <= buyPercentageRange.end;
                         _buyPercentage += buyPercentageRange.step)
                    {
                        for (_sellPercentage = sellPercentageRange.start;
                             _sellPercentage <= sellPercentageRange.end;
                             _sellPercentage += sellPercentageRange.step)
                        {
                            for (_drawdown = drawdownPercentageRange.start;
                                 _drawdown <= drawdownPercentageRange.end;
                                 _drawdown += drawdownPercentageRange.step)
                            {
                                // Round values to avoid floating point precision issues
                                _initialBuyPercentage = Math.Round(_initialBuyPercentage, 2);
                                _leverage = Math.Round(_leverage, 1);
                                _buyPercentage = Math.Round(_buyPercentage, 2);
                                _sellPercentage = Math.Round(_sellPercentage, 2);
                                _drawdown = Math.Round(_drawdown, 2);

                                // Execute trading simulation with current parameters
                                buyer.Buy(account, position, account.CashBalance * _initialBuyPercentage,
                                        initialPurchasePrice, _leverage);

                                compound.LoopThroughPreventLiquidation(position, account, seller, buyer,
                                    _drawdown, _sellPercentage, _buyPercentage, FilteredMaximumIterations, FilteredLowestActualPurchasePrice);

                                seller.Sell(account, position, account.CurrentUnits, finalSellPrice);

                                FilterResults(account, position, compound, buyer,
                                    FilteredFinalCashBalance, FilteredHighestLiquidationPrice, FilteredMinimumIterations, FilteredMaximumIterations);

                                ResetToOriginalValues(account, position);
                            }
                        }
                    }
                }
            }
            return _allResults.ToString();
        }

        private void FilterResults(Account account, Position position, CompoundOperations compound, BuyOperations buyer,
            double finalCashBalance, double maxLiquidationPrice, int minimumIterations, int maximumIterations)
        {
            if (account.CashBalance >= finalCashBalance
                && position.LiquidationPrice <= maxLiquidationPrice
                && compound.Iterations >= minimumIterations
                && compound.Iterations <= maximumIterations
                && compound.NextBuyMorePrice >= FilteredLowestActualPurchasePrice)
            {
                var resultText = DisplayFilteredResults(account, position, compound, buyer);
                _allResults.Append(resultText);
            }
        }
        
        private StringBuilder DisplayFilteredResults(Account account, Position position, CompoundOperations compound, BuyOperations buyer)
        {
            double profitAndLoss = account.TotalMoneyInvested * -1;
            double roi = profitAndLoss / account.InitialCashBalance * 100;

            var result = new System.Text.StringBuilder();

            result.AppendLine("===============================");
            result.AppendLine($"Initial buy percentage: {_initialBuyPercentage * 100:F0}%");
            result.AppendLine($"Leverage: {_leverage:F1}");
            result.AppendLine($"Iterations: {compound.Iterations}");
            result.AppendLine($"Drawdown to buy more: {_drawdown * 100:F0}%");
            result.AppendLine($"Buy percentage: {_buyPercentage * 100:F0}%");
            result.AppendLine($"Sell percentage: {_sellPercentage * 100:F0}%");
            result.AppendLine($"Lowest purchase price: ${buyer.PricePerPurchasedUnit:F2}");
            result.AppendLine($"Average price per unit: ${position.AveragePrice:F2}");
            result.AppendLine($"Liquidation price: ${position.LiquidationPrice:F2}");
            result.AppendLine($"P&L: ${profitAndLoss:F2}");
            result.AppendLine($"ROI: {roi:F0}%");
            result.AppendLine($"Final cash balance: ${account.CashBalance:F2}");
            result.AppendLine("===============================");
            result.AppendLine();

            return result;
        }
        
        private void StoreOriginalValues(Account account, Position position)
        {
            _originalCashBalance = account.CashBalance;
            _originalCurrentUnits = account.CurrentUnits;
            _originalMoneyInvested = account.MoneyInvested;
            _originalTotalMoneyInvested = account.TotalMoneyInvested;
            _originalLiquidationPrice = position.LiquidationPrice;
            _originalAveragePrice = position.AveragePrice;
        }

        private void ResetToOriginalValues(Account account, Position position)
        {
            account.CashBalance = _originalCashBalance;
            account.CurrentUnits = _originalCurrentUnits;
            account.MoneyInvested = _originalMoneyInvested;
            account.TotalMoneyInvested = _originalTotalMoneyInvested;
            position.LiquidationPrice = _originalLiquidationPrice;
            position.AveragePrice = _originalAveragePrice;
        }
    }
}
