using Kola.Infrastructure.Models;
using System.Collections.Generic;

namespace Kola.Infrastructure.UserAuthentication.Contracts
{
    public interface IUserInformation
    {
        long UserId { get; }
        string UserName { get; }
        string Password { get; }

        string FirstCurrency { get; }
        string SecondCurrency { get; }
        string BalanceToFirstCurrency { get; }
        string BalanceToSecondCurrency { get; }

        string FirstName { get; }
        string LastName { get; }
        string FullName { get; }

        List<AccountModel> Accounts { get; }

        void SetProfile();
        void SetAccounts();
        void SetAccount(string accountNumer);
        void SetPassword(string password);
        void GetBalanceInquiry(string accounId);
        void GetBalanceInquiry(string currency, string type);
        void SetUserId(long userId);
    }
}
