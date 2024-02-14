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

            var accountToWithdraw = _accountRepository.GetAccountById(fromAccountId);

            if (accountToWithdraw == null)
            {
                throw new NullReferenceException("User account was not found");
            }

            accountToWithdraw.Withdraw(amount);

            _accountRepository.Update(accountToWithdraw);

            if (accountToWithdraw.AreFundsLow())
            {
                _notificationService.NotifyFundsLow(accountToWithdraw.User.Email);
            }
        }
    }
}