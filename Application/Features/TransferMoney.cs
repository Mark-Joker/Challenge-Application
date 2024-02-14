using Application.DataAccess;
using Application.Domain.Services;
using System;

namespace Application.Features
{
    public class TransferMoney
    {
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationService _notificationService;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this._accountRepository = accountRepository;
            this._notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var from = _accountRepository.GetAccountById(fromAccountId);
            var to = _accountRepository.GetAccountById(toAccountId);

            // Withdraw money.
            from.Withdraw(amount);

            // Pay in money.
            to.PayIn(amount);

            // TODO: transaction is needed.
            _accountRepository.Update(from);
            _accountRepository.Update(to);

            if (from.AreFundsLow())
            {
                this._notificationService.NotifyFundsLow(from.User.Email);
            }

            if (to.IsApproachingPayInLimit())
            {
                this._notificationService.NotifyApproachingPayInLimit(to.User.Email);
            }
        }
    }
}
