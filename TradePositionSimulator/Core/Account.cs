using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradePositionSimulator
{
    public class Account
    {
        public double InitialCashBalance { get; set; }
        public double CashBalance { get; set; }
        public double CurrentUnits { get; set; }
        public double TotalBorrowedAmount { get; set; }
        public double MoneyInvested { get; set; }
        public double TotalMoneyInvested { get; set; }

        public Account()
        {

        }

        public Account(double initialCashBalance)
        {
            CashBalance = initialCashBalance;
            InitialCashBalance = initialCashBalance;
        }

    }
}
