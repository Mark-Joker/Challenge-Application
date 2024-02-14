using System;

namespace Application.Domain
{
    public class Account
    {
        public const decimal PayInLimit = 4000m;
        public const decimal WarningMoneyThreshold = 500m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; private set; }

        public decimal Withdrawn { get; private set; }

        public decimal PaidIn { get; private set; }

        public void Withdraw(decimal amount)
        {
            if (amount < 0m)
            {
                throw new InvalidOperationException("Negative amount can't be processed");
            }

            // Check if money is enough to withdraw.
            if (amount > Balance)
            {
                throw new InvalidOperationException("Insufficient funds to make withdraw");
            }

            // Withdraw money.
            Balance = Balance - amount;
            Withdrawn = Withdrawn - amount;
        }

        public void PayIn(decimal amount)
        {
            if (amount < 0m)
            {
                throw new InvalidOperationException("Negative amount can't be processed");
            }

            // Check pay in limit.
            var paidInAfterPayIn = PaidIn + amount;
            if (paidInAfterPayIn > PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            // Pay in money
            Balance = Balance + amount;
            PaidIn = PaidIn + amount;
        }

        public bool AreFundsLow()
        {
            bool result = Balance < WarningMoneyThreshold;

            return result;
        }

        public bool IsApproachingPayInLimit()
        {
            bool result = PayInLimit - PaidIn < WarningMoneyThreshold;

            return result;
        }
    }
}