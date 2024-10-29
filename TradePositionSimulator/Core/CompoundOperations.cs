using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TradePositionSimulator.Services;

namespace TradePositionSimulator
{
    public class CompoundOperations
    {
        public double NextBuyMorePrice { get; set; }
        public int Iterations { get; set; }

        private readonly SimulationLogger? _logger;

        public CompoundOperations()
        {

        }

        public CompoundOperations(SimulationLogger logger)
        {
            _logger = logger;
        }

        OptimalSettingsFinder optimizer = new();
        
        public bool PreventLiquidation(Position position, Account account, SellOperations seller, BuyOperations buyer, 
            double drawdownPercentage, double sellPercentage, double buyPercentage)
        {
            if (_logger != null)
            {
                _logger.Log($"Modifying position: Drawdown: {drawdownPercentage:F2}");
            }
            double buyMorePrice = position.AveragePrice * (position.Leverage - drawdownPercentage) / position.Leverage;

            bool sellSuccessful = seller.Sell(account, position, account.CurrentUnits * sellPercentage, buyMorePrice); // There is no point in multiplying by 0.5 or more as the ROI will always be equal to spot buying.

            if (sellSuccessful)
            {
                buyer.Buy(account, position, account.CurrentUnits * buyPercentage, buyMorePrice, position.Leverage);
                if (_logger != null)
                {
                    position.DisplayPositionSummary(account);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void LoopThroughPreventLiquidation(Position position, Account account, SellOperations seller, BuyOperations buyer,
            double drawdownPercentage, double sellPercentage, double buyPercentage, int maxIterations, double lowestPrice)
        {
            Iterations = 0;
            bool preventLiquidationSuccessful = true;
            NextBuyMorePrice = position.AveragePrice * (position.Leverage - drawdownPercentage) / position.Leverage;
            double nextPurchaseCost = account.CurrentUnits * buyPercentage * NextBuyMorePrice / position.Leverage;
            while (account.CashBalance >= nextPurchaseCost && preventLiquidationSuccessful && Iterations <= maxIterations && NextBuyMorePrice >= lowestPrice)
            {
                Iterations++;
                if (_logger != null)
                {
                    _logger.Log($"Iteration {Iterations}");
                }
                preventLiquidationSuccessful = PreventLiquidation(position, account, seller, buyer,
                    drawdownPercentage, sellPercentage, buyPercentage);
                NextBuyMorePrice = position.AveragePrice * (position.Leverage - drawdownPercentage) / position.Leverage;
                nextPurchaseCost = account.CurrentUnits * buyPercentage * NextBuyMorePrice / position.Leverage;
            }
        }
    }
}
