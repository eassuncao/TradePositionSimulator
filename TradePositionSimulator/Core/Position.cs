using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TradePositionSimulator.Services;

namespace TradePositionSimulator
{
    public class Position
    {
        public double Leverage { get; set; }
        public double AveragePrice { get; set; }
        public double LiquidationPrice { get; set; }

        private readonly SimulationLogger? _logger;

        public Position()
        {

        }
        
        public Position(SimulationLogger logger)
        {
            _logger = logger;
        }

        public void DisplayPositionSummary(Account account)
        {
            if (_logger != null)
            {
                _logger.Log($"Asset balance: {account.CurrentUnits:F3} units");
                _logger.Log($"Average price per unit: ${AveragePrice:F2}");
                _logger.Log($"Liquidation price: ${LiquidationPrice:F2}");
                _logger.Log($"Total spent: ${account.TotalMoneyInvested:F2}");
                _logger.Log($"Borrowed amount: ${account.TotalBorrowedAmount:F2}");
                _logger.Log($"Remaining cash balance: ${account.CashBalance:F2}");
                _logger.Log("");
            }
        }
    }
}
