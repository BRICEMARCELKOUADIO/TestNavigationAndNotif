using Kola.Infrastructure;
using Kola.Infrastructure.Controls.ToastMessage;
using Kola.Infrastructure.EventAggregator;
using Kola.Infrastructure.Extensions;
using Kola.Infrastructure.Helper;
using Kola.Infrastructure.Models;
using Kola.Infrastructure.Navigation;
using Kola.Infrastructure.Setting.Contract;
using Kola.Infrastructure.UserAuthentication.Contracts;
using Kola.Starting.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using reewire.core.services;
using reewire.core.services.contracts.application;
using reewire.core.services.contracts.services;
using reewire.core.services.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Kola.Starting.ViewModels
{
    public class CharityViewModel : ViewModelBase
    {
        private readonly IOrganizations _organizations;
        private readonly IAuthentication _authentication;
        private readonly IFeature _donationservice;
        private readonly IUserInformation _userInformation;
        private readonly IAppInfo _appInfo;
        private readonly IFeature _statementsservice;

        private AccountModel _accountSelected = new AccountModel();
        public Field<string> Organization { get; set; } = new Field<string>();

        public Field<string> Amount { get; set; } = new Field<string>();
        public Field<int> AmountMaxLength { get; set; } = new Field<int>(Constants.MaxLengthAmount);
        public Field<string> CurrentCurrency { get; set; } = new Field<string>();
        public Field<bool> HasAccountSelected { get; set; } = new Field<bool>(false);
        public Field<string> CurrentAccountName { get; set; } = new Field<string>();
        public Field<string> CurrentBalance { get; set; } = new Field<string>();

        public Field<bool> FirstStep { get; set; } = new Field<bool>(true);

        public Field<bool> SecondStep { get; set; } = new Field<bool>(false);

        private ObservableCollection<StatetementGroup> _items = new ObservableCollection<StatetementGroup>();
        public ObservableCollection<StatetementGroup> Items
        {
            get { return _items; }
            set
            {
                SetProperty(ref _items, value);
            }
        }

        private IEventAggregator _ea;
        public ICommand DisplayBurger => new DelegateCommand(OnDisplayBurger);
        public ICommand RegisterCharityCommand => new DelegateCommand(RegisterCharity);
        public ICommand GotoOpenAccountPopupCommand => new DelegateCommand(GotoOpenAccountPopup);
        public ICommand OnOpenOrganizationPopupCommand => new DelegateCommand(OnOpenOrganizationPopup);
        public ICommand GotoValidateCommand => new DelegateCommand(GotoValidate);
        public CharityViewModel(INavigationService navigationService,
                                IEventAggregator ea,
                                IAuthentication authentication,
                                IUserInformation userInformation,
                                IFeature donationservice,
                                IOrganizations organizations,
                                IAppInfo appInfo,
                                IFeature statementsservice) : base(navigationService)
        {
            _organizations = organizations ?? throw new System.ArgumentNullException(nameof(organizations));
            _authentication = authentication ?? throw new System.ArgumentNullException(nameof(authentication));
            _donationservice = donationservice ?? throw new System.ArgumentNullException(nameof(donationservice));
            _userInformation = userInformation ?? throw new System.ArgumentNullException(nameof(userInformation));
            _appInfo = appInfo ?? throw new System.ArgumentNullException(nameof(appInfo));
            _statementsservice = statementsservice ?? throw new System.ArgumentNullException(nameof(statementsservice));

            _ea = ea;
            Initialize();
        }

        private  async void Initialize()
        {
            Items = new ObservableCollection<StatetementGroup>();
            await Load();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            IsLoading = false;
            base.OnNavigatedTo(parameters);
            var popupId = parameters.GetValue<string>(Constants.PopupId);

            switch (popupId)
            {
                case "ACCOUNT_POPUP":
                    var selectedData = parameters.GetValue<PikerItem>(Constants.PopupResponseData);
                    _accountSelected.AccountNumber = selectedData.Id;
                    _accountSelected.Currency = CurrentCurrency.Value = selectedData.Name_4;
                    HasAccountSelected.Value = !string.IsNullOrEmpty(CurrentCurrency.Value);

                    _accountSelected.Type = selectedData.Name_2;
                    _accountSelected.Balance = CurrentBalance.Value = selectedData.Name_3;
                    break;
                case "ORGANIZATION_POPUP":
                    var org = parameters.GetValue<PikerItem>(Constants.PopupResponseData);
                    Organization.Id = org.Id;
                    Organization.Value = org.Name_1;
                    break;
                case "CHARITY_PINCODE_POPUP":
                    {
                        var ok = parameters.GetValue<bool>(Constants.PopupResponseData);
                        // call service do charity
                        DoService(ok);
                    }
                    break;
                default:
                    break;
            }
        }

        private async void OnOpenOrganizationPopup()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            ObservableCollection<PikerItem> list = new ObservableCollection<PikerItem>();

            var listorgs = await _organizations.GetList(_authentication.GetToken(), (int)_appInfo.GetApp()?.Id);
            if (listorgs?.isuccess == true)
            {
                list = listorgs.data.Select(x => new PikerItem
                {
                    Id = x.UserId,
                    Name_1 = $"{x.Fname} {x.Lname}"
                }).ToObservableCollection();
            }

            var parameters = new NavigationParameters
                {
                    { Constants.PopupId, "ORGANIZATION_POPUP" },
                    { Constants.PopupTitle, TranslateManagerHelper.Convert("select_organization").ToUpperInvariant() },
                   // {Constants.PopupIcon, "icon-add-user"},
                    {Constants.PopupCurrentData, list.FirstOrDefault(x => x.Id == Organization.Id)},
                    { Constants.PopupResquestData, list }
                };

            await NavigationService.NavigateAsync(PopupName.PikerPopup, parameters).ConfigureAwait(false);

            IsLoading = false;
        }

        private async void GotoOpenAccountPopup()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            var parameters = new NavigationParameters
                {
                    { Constants.PopupId, "ACCOUNT_POPUP" },
                    { Constants.PopupTitle, TranslateManagerHelper.Convert("select_account").ToUpperInvariant() },
                   // {Constants.PopupIcon, "icon-add-user"},
                    {Constants.PopupCurrentData, _accountSelected},

                };

            await NavigationService.NavigateAsync(PopupName.PikerWalletPopup, parameters).ConfigureAwait(false);

            IsLoading = false;

        }

        private void OnDisplayBurger()
        {
            _ea.GetEvent<BurgerEvent>().Publish();
        }

        private void RegisterCharity()
        {
            FirstStep.Value = false;
            SecondStep.Value = true;

        }

        private async void GotoValidate()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            Organization.HasError = Amount.HasError = CurrentCurrency.HasError = false;

            if (string.IsNullOrEmpty(Organization.Value))
            {
                Organization.ErrorText = TranslateManagerHelper.Convert("organisation_error");
                Organization.HasError = true;
                IsLoading = false;
                return;
            }

            if (string.IsNullOrEmpty(Amount.Value))
            {
                Amount.ErrorText = TranslateManagerHelper.Convert("amount_error");
                Amount.HasError = true;
                IsLoading = false;
                return;
            }

            if (double.Parse(Amount.Value) <= 0)
            {
                Amount.ErrorText = TranslateManagerHelper.Convert("amount_must_be_greater_than_0");
                Amount.HasError = true;
                IsLoading = false;
                return;
            }

            if (string.IsNullOrEmpty(CurrentCurrency.Value))
            {
                CurrentCurrency.ErrorText = TranslateManagerHelper.Convert("source_account_error");
                CurrentCurrency.HasError = true;
                IsLoading = false;
                return;
            }

            var parameters = new NavigationParameters
                {
                    { Constants.PopupId, "CHARITY_PINCODE_POPUP" },
                };
            await NavigationService.NavigateAsync(PopupName.PinCodePopup, parameters).ConfigureAwait(false);
            IsLoading = false;
        }

        private async void DoService(bool ok)
        {
            if (!ok)
            {
                return;
            }

            if (IsBusy || IsLoading)
            {
                // message toast
                return;
            }
            IsLoading = true;


            // call the donation service
            DonationRequest d = new DonationRequest();
            d.SourceAccountNumber = _accountSelected.AccountNumber;

            featureDTO f = new featureDTO
            {
                FeatureName = "DONATION",
                Amount = decimal.Parse(Amount.Value),
                SenderId = ApiConstants.CountryPrefix + _userInformation.UserName,
                SenderPassword = _userInformation.Password,
                TargetId = Organization.Id,
                jsonObjString = JsonConvert.SerializeObject(d)
            };

            var result = await _donationservice.Execute<featureDTO, DonationResponse>(_authentication.GetToken(), f, ApiConstants.SystemId);

            NavigationParameters parameters = null;
            string message = "";
            bool isSuccess = false;
            if (result != null)
            {
                if (result.isuccess)
                {
                    var acc = result.data;
                    UpdateUserAccount(new AccountModel
                    {
                        AccountNumber = acc.AccountUpdate.AccountNumber,
                        Balance = acc.AccountUpdate.Balance.ToString(),
                        Currency = acc.AccountUpdate.CurrencyISO,
                        Type = acc.AccountUpdate.AccountType
                    });

                    message = !string.IsNullOrEmpty(result.msg) ? result.msg : TranslateManagerHelper.Convert("operation_successfully");
                    isSuccess = true;
                }
                else
                {
                    switch (result.errcode)
                    {
                        case "EXCEPTION":
                            message = TranslateManagerHelper.Convert("operation_failed");
                            break;
                        default:
                            message = !string.IsNullOrEmpty(result.msg) ? result.msg : TranslateManagerHelper.Convert("error_occured");
                            break;
                    }
                    isSuccess = false;
                }
            }
            else
            {
                // TODO : what message do we send ?
                isSuccess = false;
                message = TranslateManagerHelper.Convert("error_contact_support");
            }

            parameters = new NavigationParameters
                    {
                        { Constants.PopupIsSucces, isSuccess },
                        { Constants.PopupIsBeforeHome, false },
                        { Constants.PopupMessage, message },
                        {Constants.PopupNextPage, FunctionName.Charity }
                    };
            await NavigationService.NavigateAsync(PopupName.SuccessfullPopup, parameters);

            SecondStep.Value = false;
            FirstStep.Value = true;
            IsLoading = false;
        }

        private async Task Load()
        {
            if (IsBusy || IsLoading)
            {
                // message toast
                return;
            }
            IsLoading = true;

            AccountStatementRequest asr = new AccountStatementRequest();
            asr.SourceAccountNumber = "";

            featureDTO fb = new featureDTO
            {
                FeatureName = "MY_DONATION",
                SenderId = ApiConstants.CountryPrefix + _userInformation.UserName,
                SenderPassword = _userInformation.Password,
                TargetId = "",
                Amount = 0,
                jsonObjString = JsonConvert.SerializeObject(asr)
            };

            List<StatementDetails> list = new List<StatementDetails>();
            var raccst = await _statementsservice.Execute<featureDTO, List<AccountStatementResponse>>(_authentication.GetToken(), fb, Infrastructure.ApiConstants.SystemId);

            if (raccst != null && raccst.isuccess)
            {
                var accresp = raccst?.data;
                list = accresp?.Select(x => new StatementDetails
                {
                    OperationNumber = x.TransactionId,
                    Amount = x.Amount.ToString(),
                    Currency = "Ex.LRD",
                    Description = x.Stakeholder, // x.Type,
                    OperationSign = x.Sign,
                    IsPositive = x.Sign == "+",
                    OperationTime = x.Date.ToShortTimeString(),
                    OpDate = x.Date.ToString("yyyy-MM-dd"),
                    OperationType = x.Opname.ToUpperInvariant()
                }).ToList();

                foreach (var s in list)
                {
                    try
                    {
                        var group = Items.FirstOrDefault(x => x.OperationDate.Equals(s.OpDate));
                        if (group == null)
                        {
                            group = new StatetementGroup(s.OpDate);
                            Items.Add(group);
                        }
                        group.Add(s);
                    }
                    catch (Exception ex)
                    {
                        DependencyService.Get<IMessage>().ShowSnackBarNetwork(TranslateManagerHelper.Convert("error_occured"));
                    }
                }
            }
            else
            {
                string message;
                switch (raccst?.errcode)
                {
                    case "EXCEPTION":
                        message = TranslateManagerHelper.Convert("operation_failed");
                        break;
                    default:
                        message = !string.IsNullOrEmpty(raccst?.msg) ? raccst?.msg : TranslateManagerHelper.Convert("error_occured");
                        break;
                }

                var parameters = new NavigationParameters
                    {
                        { Constants.PopupIsSucces, false },
                        { Constants.PopupMessage, message },
                        {Constants.PopupNextPage, FunctionName.Home }
                    };

                await NavigationService.NavigateAsync(PopupName.SuccessfullPopup, parameters);
                IsLoading = false;
            }
            IsEmpty = Items.Count == 0;
            IsLoading = false;
        }
    }
}
