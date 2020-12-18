using Kola.Infrastructure.Controls.ToastMessage;
using Kola.Infrastructure.Models;
using Plugin.LocalNotifications;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Kola.Infrastructure.Navigation
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
    {

        protected static AccountModel _accountUSD { get; private set; }
        protected  static AccountModel _accountLRD { get; private set; }
        public static string _userFullName;

        public Field<string> FirstCurrency { get; set; } = new Field<string>();
        public Field<string> SecondCurrency { get; set; } = new Field<string>();
        public Field<string> BalanceToFirstCurrency { get; set; } = new Field<string>();
        public Field<string> BalanceToSecondCurrency { get; set; } = new Field<string>();


        public Field<string> UserFullName { get; set; } = new Field<string>();
        public Field<string> Version { get; set; } = new Field<string>($"beta v{DateTime.Now.ToString("MM-dd-yyyy-HH", CultureInfo.InvariantCulture)}");

        protected INavigationService NavigationService { get; private set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _subTitle;
        public string SubTitle
        {
            get { return _subTitle; }
            set { SetProperty(ref _subTitle, value); }
        }

        private string _icon;
        public string Icon
        {
            get { return _icon; }
            set { SetProperty(ref _icon, value); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                SetProperty(ref _isBusy, value);
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private bool _isEmpty;
        public bool IsEmpty
        {
            get { return _isEmpty; }
            set { SetProperty(ref _isEmpty, value); }
        }

        private bool _hasMissingData;
        public bool HasMissingData
        {
            get { return _hasMissingData; }
            set { SetProperty(ref _hasMissingData, value); }
        }

        public ICommand GotoBackCommand => new Command<string>(GotoBack);
        public ICommand GotoCancelCommand => new DelegateCommand(GotoCancel);
        public ICommand GotoAccountCommand => new DelegateCommand(GotoAccount);

        public ViewModelBase(INavigationService navigationService, bool hasReleveView = true)
        {
            NavigationService = navigationService;

            if (hasReleveView)
            {
                SetReleveView();
            }
        }

        private void SetReleveView()
        {
            UserFullName.Value = _userFullName;
            FirstCurrency.Value = _accountUSD?.Currency;
            BalanceToFirstCurrency.Value = _accountUSD?.Balance;

            SecondCurrency.Value = _accountLRD?.Currency;
            BalanceToSecondCurrency.Value = _accountLRD?.Balance;
        }

        public void UpdateCurrentUserData(IList<AccountModel> accounts, string fullName)
        {
            var accUSD = accounts.FirstOrDefault(x => x.Currency == "USD");
            if (accUSD != null)
            {
                _accountUSD = accUSD;
            }

            var accLRD = accounts.FirstOrDefault(x => x.Currency == "LRD");
            if (accLRD != null)
            {
                _accountLRD = accLRD;
            }

            if (!string.IsNullOrEmpty(fullName))
                _userFullName = fullName;

        }        

        public void UpdateUserAccount(AccountModel account)
        {
            if (_accountUSD == null && account.Currency == "USD")
            {
                _accountUSD = new AccountModel
                {
                    AccountNumber = account.AccountNumber,
                    Balance = account.Balance,
                    Currency = account.Currency,
                    Type = account.Type,
                    Name = account.Name
                };

            }
            else
            {
                if (account.Currency == "USD")
                {
                    _accountUSD.Balance = account.Balance;
                }
            }
            if (_accountLRD == null && account.Currency == "LRD")
            {
                _accountLRD = new AccountModel
                {
                    AccountNumber = account.AccountNumber,
                    Balance = account.Balance,
                    Currency = account.Currency,
                    Type = account.Type,
                    Name = account.Name
                };
            }
            else
            {
                if (account.Currency == "LRD")
                {
                    _accountLRD.Balance = account.Balance;
                }
            }




            SetReleveView();
        }

        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }

        /// <summary>
        /// Show Notification With Scheduler
        /// </summary>
        /// <param name="ptitle"></param>
        /// <param name="pdescription"></param>
        /// <param name="pid"></param>
        /// <param name="pdelay">pdelay before to show (secondes)</param>
        public void ShowNotificationScheduler(string ptitle, string pdescription, int pid, int pdelay = 10)
        {
            CrossLocalNotifications.Current.Show(ptitle, pdescription, pid, DateTime.Now.AddSeconds(pdelay));
        }

        /// <summary>
        /// Show Notification Immediatly
        /// </summary>
        /// <param name="ptitle"></param>
        /// <param name="pdescription"></param>
        public void ShowNotificationImmediatly(string ptitle, string pdescription)
        {
            CrossLocalNotifications.Current.Show(ptitle, pdescription);
        }

        /// <summary>
        /// Cancel Notification Scheduler
        /// </summary>
        /// <param name="pid"></param>
        public void CancelNotificationScheduler(int pid)
        {
            CrossLocalNotifications.Current.Cancel(pid);
        }

        /// <summary>
        /// Show Toast
        /// </summary>
        /// <param name="pmessage"></param>
        public void ShowToast(string pmessage)
        {
            DependencyService.Get<IMessage>().ShowToast(pmessage);
        }

        /// <summary>
        /// Show Snack Bar Toast
        /// </summary>
        /// <param name="pmessage"></param>
        public void ShowSnackBar(string pmessage)
        {
            DependencyService.Get<IMessage>().ShowSnackBar(pmessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pmessage"></param>
        /// <param name="paction"></param>
        /// <param name="pactionText">Methode action</param>
        /// <param name="pactionTextColor"></param>
        public void ShowSnackBarWithAction(string pmessage, Action<object> paction = null, string pactionText = "Ok", string pactionTextColor = "#F48120")
        {
            if (paction == null)
            {
                paction = (e) => { };
            }
            DependencyService.Get<IMessage>().ShowSnackBarWithAction(pmessage, paction, pactionText, pactionTextColor);
        }

        private async void GotoBack(string p)
        {
            if (IsBusy || IsLoading)
            {
                // message toast
                return;
            }
            IsLoading = true;
            await NavigationService.GoBackAsync();
            IsLoading = false;
        }

        private async void GotoCancel()
        {
            if (IsBusy || IsLoading)
            {
                // message toast
                return;
            }
            IsLoading = true;
            await NavigationService.GoBackAsync();
            IsLoading = false;
        }

        private async void GotoAccount()
        {
            if (IsBusy || IsLoading)
            {
                // message toast
                return;
            }
            IsLoading = true;
            await NavigationService.NavigateAsync(FunctionName.Account).ConfigureAwait(false);
            IsLoading = false;
        }
    }
}
