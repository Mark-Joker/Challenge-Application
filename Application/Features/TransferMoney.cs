using Application.DataAccess;
using Application.Domain.Services;
using System;

namespace Application.Features
{
    public class TransferMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var from = this.accountRepository.GetAccountById(fromAccountId);
            var to = this.accountRepository.GetAccountById(toAccountId);

            // Withdraw money.
            from.Withdraw(amount);

            // Pay in money.
            to.PayIn(amount);

            // TODO: transaction is needed.
            this.accountRepository.Update(from);
            this.accountRepository.Update(to);

            if (from.AreFundsLow())
            {
                this.notificationService.NotifyFundsLow(from.User.Email);
            }

            if (to.IsApproachingPayInLimit())
            {
                this.notificationService.NotifyApproachingPayInLimit(to.User.Email);
            }
        }
    }
}
