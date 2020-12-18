using Kola.Infrastructure.Models;
using Kola.Infrastructure.UserAuthentication.Contracts;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Kola.Infrastructure.UserAuthentication
{
    public class UserInformation : IUserInformation
    {
        public long UserId { get; private set; }
        public string UserName { get; private set; }

        public string Password { get; private set; }

        public string FirstCurrency { get; private set; }

        public string SecondCurrency { get; private set; }

        public string BalanceToFirstCurrency { get; private set; }

        public string BalanceToSecondCurrency { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string FullName { get; private set; }

        public List<AccountModel> Accounts { get; private set; }

        public UserInformation(string username)
        {
            UserName = username;
            Accounts = new List<AccountModel>();
        }

        public void SetPassword(string password)
        {
            Password = password;
        }

        public void SetUserId(long userId)
        {
            UserId = userId;
        }

        public void GetBalanceInquiry(string accounId)
        {
            throw new NotImplementedException();
        }

        public void GetBalanceInquiry(string currency, string type)
        {
            throw new NotImplementedException();
        }

        public void add(AccountModel account)
        {
            Accounts.Add(account);
        }

        public AccountModel getaccount(string stype, string currency)
        {
            return Accounts.Find(a => a.Name == stype && a.Currency == currency);
        }

        public AccountModel getaccount(string accnumber)
        {
            return Accounts.Find(a => a.AccountNumber == accnumber);
        }
        public void SetAccounts()
        {       }

        public void SetAccount(string accountNumer)
        {
            throw new NotImplementedException();
        }

        public void SetProfile()
        {

        }
    }
}
