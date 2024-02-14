using Application.DataAccess;
using Application.Domain.Services;
using System;

namespace Application.Features
{
    public class WithdrawMoney
    {
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationService _notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            _accountRepository = accountRepository;
            _notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            if (amount == 0m)
            {
                return;
            }

            if (amount < 0m)
            {
                throw new InvalidOperationException("Negative amount can't be processed");
            }

            var accountToWithdraw = _accountRepository.GetAccountById(fromAccountId);

            if (accountToWithdraw == null)
            {
                throw new NullReferenceException("User account was not found");
            }

            // Check if money is enough to withdraw.
            var balanceAfterWithdraw = accountToWithdraw.Balance - amount;
            if (balanceAfterWithdraw < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to make withdraw");
            }

            // Withdraw money.   
            accountToWithdraw.Balance = accountToWithdraw.Balance - amount;
            accountToWithdraw.Withdrawn = accountToWithdraw.Withdrawn - amount;

            _accountRepository.Update(accountToWithdraw);

            if (accountToWithdraw.Balance < 500m)
            {
                _notificationService.NotifyFundsLow(accountToWithdraw.User.Email);
            }
        }
    }
}