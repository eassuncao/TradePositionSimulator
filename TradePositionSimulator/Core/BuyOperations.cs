using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TradePositionSimulator.Services;

namespace TradePositionSimulator
{
    public class BuyOperations
    {
        private readonly SimulationLogger? _logger;

        public double PricePerPurchasedUnit { get; set; }

        public BuyOperations()
        {

        }
        
        public BuyOperations(SimulationLogger logger)
        {
            _logger = logger;
        }

        public bool Buy(Account account, Position position, 
            double numberOfPurchasedUnits, double pricePerPurchasedUnit, double newLeverage)
        {
            double borrowedAmount = ComputeCostAndInvestment(account, numberOfPurchasedUnits, pricePerPurchasedUnit, newLeverage);

            if (account.CashBalance < account.MoneyInvested)
            {
                if (_logger != null)
                {
                    _logger.Log("Unable to buy: Insufficient funds\n");
                }
                return false;
            }
            else
            {
                DisplayPurchaseInfo(numberOfPurchasedUnits, pricePerPurchasedUnit, newLeverage, account.MoneyInvested);

                UpdateAccountAndPosition(
                    account,
                    position,
                    numberOfPurchasedUnits,
                    pricePerPurchasedUnit,
                    newLeverage,
                    borrowedAmount);

                return true;
            }
        }

        private double ComputeCostAndInvestment(Account account, 
            double numberOfPurchasedUnits, double pricePerPurchasedUnit, double newLeverage)
        {
            PricePerPurchasedUnit = pricePerPurchasedUnit;
            double costOfPurchase = numberOfPurchasedUnits * pricePerPurchasedUnit;
            account.MoneyInvested = costOfPurchase / newLeverage;
            double borrowedAmount = costOfPurchase - account.MoneyInvested;

            return borrowedAmount;
        }
        
        private void UpdateAccountAndPosition(
            Account account,
            Position position,
            double numberOfPurchasedUnits,
            double pricePerPurchasedUnit,
            double newLeverage,
            double borrowedAmount)
        {
            position.Leverage = newLeverage;

            position.AveragePrice = ((position.AveragePrice * account.CurrentUnits) + (pricePerPurchasedUnit * numberOfPurchasedUnits))
                                    / (account.CurrentUnits + numberOfPurchasedUnits);

            account.CurrentUnits += numberOfPurchasedUnits;
            account.CashBalance -= account.MoneyInvested;
            account.TotalMoneyInvested += account.MoneyInvested;
            account.TotalBorrowedAmount += borrowedAmount;

            position.LiquidationPrice = position.AveragePrice * (position.Leverage - 1) / position.Leverage;
        }

        private void DisplayPurchaseInfo(double numberOfPurchasedUnits, double pricePerPurchasedUnit, double leverage, double moneyInvested)
        {
            if (_logger != null)
            {
            _logger.Log("Buying:");
            _logger.Log($"{numberOfPurchasedUnits:F3} units purchased");
            _logger.Log($"Price per unit: ${pricePerPurchasedUnit:F2}");
            _logger.Log($"Leverage: {leverage}x");
            _logger.Log($"Transaction cost: ${moneyInvested:F2}");
            _logger.Log("");
            }
        }
    }
}
