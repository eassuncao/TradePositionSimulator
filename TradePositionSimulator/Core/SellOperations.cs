using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TradePositionSimulator.Services;

namespace TradePositionSimulator
{
    public class SellOperations
    {
        private readonly SimulationLogger? _logger;

        public SellOperations()
        {

        }
        
        public SellOperations(SimulationLogger logger)
        {
            _logger = logger;
        }

        public bool Sell(Account account, Position position, double numberOfUnitsSold, double pricePerUnitSold)
        {
            var (borrowedAmountToRepay, proceeds, netProceeds) = ComputeProceedsAndRepayment(account, numberOfUnitsSold, pricePerUnitSold);

            if (numberOfUnitsSold > account.CurrentUnits)
            {
                if (_logger != null)
                {
                    _logger.Log("Unable to sell: Attempted to sell more than the available units.\n");
                }
                return false;
            }

            else if (numberOfUnitsSold == 0)
            {
                if (_logger != null)
                {
                    _logger.Log("No sale.");
                }
                
                return true;
            }
            else if (proceeds < 0.01)
            {
                if (_logger != null)
                {
                    _logger.Log("Unable to sell: Proceeds less than $0.01.\n");
                }
                    
                return false;
            }
            else
            {
                DisplaySaleInfo(numberOfUnitsSold, pricePerUnitSold, netProceeds);
                
                UpdateAccountAndPosition(account, netProceeds, numberOfUnitsSold, borrowedAmountToRepay);

                return true;
            }
        }

        private (double borrowedAmountToRepay, double proceeds, double netProceeds)
        ComputeProceedsAndRepayment(Account account, double numberOfUnitsSold, double pricePerUnitSold)
        {
            double borrowedAmountToRepay = (numberOfUnitsSold / account.CurrentUnits) * account.TotalBorrowedAmount;
            double proceeds = numberOfUnitsSold * pricePerUnitSold;
            double netProceeds = proceeds - borrowedAmountToRepay;

            return (borrowedAmountToRepay, proceeds, netProceeds);
        }

        private void DisplaySaleInfo(double numberOfUnitsSold, double pricePerUnitSold, double netProceeds)
        {
            if (_logger != null)
            {
                _logger.Log("Selling:");
                _logger.Log($"{numberOfUnitsSold:F3} units sold");
                _logger.Log($"Price per unit: ${pricePerUnitSold:F2}");
                _logger.Log($"Net proceeds: ${netProceeds:F2}");
                _logger.Log("");
            }
        }

        private void UpdateAccountAndPosition(Account account, double netProceeds, double numberOfUnitsSold, double borrowedAmountToRepay)
        {
            account.CashBalance += netProceeds;
            account.CurrentUnits -= numberOfUnitsSold;
            account.TotalMoneyInvested -= netProceeds;
            account.TotalBorrowedAmount -= borrowedAmountToRepay;
        }
    }
}
