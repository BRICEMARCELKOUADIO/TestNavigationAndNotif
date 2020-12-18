using Kola.Infrastructure.EventAggregator;
using Kola.Infrastructure.Helper;

using Kola.Infrastructure.Navigation;
using Kola.Starting.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Kola.Infrastructure;
using Kola.Infrastructure.Models;
using Kola.Infrastructure.UserAuthentication.Contracts;
using reewire.core.services.contracts.services;
using reewire.core.services.models;
using reewire.core.services;
using Newtonsoft.Json;

namespace Kola.Starting.ViewModels
{
    public class ManualPaymentViewModel : ViewModelBase
    {
        private readonly IAuthentication _authentication;
        private readonly IUserInformation _userinformation;
        private readonly IFeature _makepayment;

        private AccountModel _accountSelected;

        public Field<string> PreviousText { get; set; } = new Field<string>();
        public Field<string> NextText { get; set; } = new Field<string>();

        public Field<string> MerchantId { get; set; } = new Field<string>();
        public Field<string> MerchantName { get; set; } = new Field<string>();
        public Field<string> Amount { get; set; } = new Field<string>();
        public Field<int> AmountMaxLength { get; set; } = new Field<int>(Constants.MaxLengthAmount);
        public Field<string> CurrentCurrency { get; set; } = new Field<string>();
        public Field<bool> HasAccountSelected { get; set; } = new Field<bool>(false);
        public Field<string> CurrentAccountName { get; set; } = new Field<string>();
        public Field<string> CurrentBalance { get; set; } = new Field<string>();
        public Field<int> MerchantIdMaxLength { get; set; } = new Field<int>(Constants.MaxLengthMerchaniTd);


        public Field<int> TotalStep { get; set; } = new Field<int>(2);
        public Field<int> CurrentStep { get; set; } = new Field<int>(1);
        public Field<bool> FirstStep { get; set; } = new Field<bool>(true);
        public Field<bool> SecondStep { get; set; } = new Field<bool>(false);

        public ICommand GotoNextCommand => new DelegateCommand(GotoNext);
        public new ICommand GotoPreviousCommand => new DelegateCommand(GotoPrevious);

        public ICommand GotoOpenAccountPopupCommand => new DelegateCommand(GotoOpenAccountPopup);

        private IEventAggregator _ea;
        public ICommand DisplayBurger => new DelegateCommand(OnDisplayBurger);
        public ICommand GotoPageCommand => new DelegateCommand<string>(GotoPage);
        public new ICommand GotoCancelCommand => new DelegateCommand(GotoCancel);

        public ManualPaymentViewModel(INavigationService navigationService,
                                      IEventAggregator ea,
                                      IAuthentication authentication,
                                      IUserInformation userinformation,
                                      IFeature makepayment) : base(navigationService)
        {
            _authentication = authentication;
            _userinformation = userinformation;
            _makepayment = makepayment;

            _ea = ea;
            PreviousText.Value = TranslateManagerHelper.Convert("cancel").ToUpperInvariant();
            NextText.Value = TranslateManagerHelper.Convert("next").ToUpperInvariant();
            Initialize();
        }

        private void Initialize()
        {
            _accountSelected = new AccountModel();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
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
                case "MANUAL_PAYMENT":
                    {
                        var ok = parameters.GetValue<bool>(Constants.PopupResponseData);
                        // call Qr Code service
                        DoPaymentService(ok);
                    }
                    break;
                default:
                    break;
            }
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

        protected async void GotoPage(string p)
        {
            if (IsBusy || IsLoading)
            {
                // Message Tosat
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;
            await NavigationService.NavigateAsync($"{p}");
            IsLoading = false;
        }

        private async void GotoCancel()
        {
            if (IsBusy || IsLoading)
            {
                // Message Tosat
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;
            await NavigationService.GoBackAsync();
            IsLoading = false;
        }

        private void OnDisplayBurger()
        {
            _ea.GetEvent<BurgerEvent>().Publish();
        }

        private async void GotoNext()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            switch (CurrentStep.Value)
            {
                case 1:
                    {
                        MerchantId.HasError = Amount.HasError = CurrentCurrency.HasError = false;

                        if (string.IsNullOrEmpty(MerchantId.Value))
                        {
                            MerchantId.ErrorText = TranslateManagerHelper.Convert("merchant_error");
                            MerchantId.HasError = true;
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

                        FirstStep.Value = false;
                        SecondStep.Value = true;
                        PreviousText.Value = TranslateManagerHelper.Convert("previous").ToUpperInvariant();
                        NextText.Value = TranslateManagerHelper.Convert("make_payment").ToUpperInvariant();
                        CurrentStep.Value++;
                        break;
                    }
                default:

                    var parameters = new NavigationParameters
                        {
                            { Constants.PopupId, "MANUAL_PAYMENT" },
                        };
                    await NavigationService.NavigateAsync(PopupName.PinCodePopup, parameters).ConfigureAwait(false);
                    break;
            }

            IsLoading = false;
        }

        private async void DoPaymentService(bool ok)
        {
            if (!ok)
                return;

            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            MerchantPaymentRequest mpr = new MerchantPaymentRequest()
            {
                TargetAccountNumber = _accountSelected.AccountNumber
            };

            featureDTO f = new featureDTO()
            {
                FeatureName = "MERCHANT_PAYMENT",
                SenderId = ApiConstants.CountryPrefix + _userinformation.UserName,
                SenderPassword = _userinformation.Password,
                Amount = decimal.Parse(Amount.Value),
                TargetId = MerchantId.Value,
                jsonObjString = JsonConvert.SerializeObject(mpr)
            };

            bool issuccess = false;
            string message = "";
            NavigationParameters parameters = null;


            var mkp = await _makepayment.Execute<featureDTO, MerchantPaymentResponse>(_authentication.GetToken(), f, ApiConstants.SystemId);
            if (mkp != null)
            {
                if (mkp.isuccess)
                {
                    issuccess = true;

                    var mk = mkp.data;
                    UpdateUserAccount(new AccountModel
                    {
                        AccountNumber = mk.AccountUpdate.AccountNumber,
                        Balance = mk.AccountUpdate.Balance.ToString(),
                        Currency = mk.AccountUpdate.CurrencyISO,
                        Type = mk.AccountUpdate.AccountType
                    });
                    message = TranslateManagerHelper.Convert("operation_successfully");
                }
                else
                {
                    issuccess = false;

                    switch (mkp.errcode)
                    {
                        case "EXCEPTION":
                            message = TranslateManagerHelper.Convert("operation_failed");
                            break;
                        case "WrongPincode":
                            message = mkp.msg;
                            break;
                        default:
                            message = !string.IsNullOrEmpty(mkp?.msg) ? mkp.msg : TranslateManagerHelper.Convert("error_occured");
                            break;
                    }

                }
            }
            else
            {
                issuccess = false;
                message = TranslateManagerHelper.Convert("error_occured");
            }


            parameters = new NavigationParameters
                        {
                            { Constants.PopupIsSucces, issuccess },
                            { Constants.PopupMessage, message }, // operation_failed
                            {Constants.PopupNextPage, FunctionName.MerchantPayment }
                        };
            await NavigationService.NavigateAsync(PopupName.SuccessfullPopup, parameters).ConfigureAwait(false);
            IsLoading = false;
        }

        private async void GotoPrevious()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            switch (CurrentStep.Value)
            {
                case 2:
                    {
                        SecondStep.Value = false;
                        FirstStep.Value = true;
                        PreviousText.Value = TranslateManagerHelper.Convert("cancel").ToUpperInvariant();
                        NextText.Value = TranslateManagerHelper.Convert("next").ToUpperInvariant();
                        CurrentStep.Value--;
                        break;
                    }
                default:
                    await NavigationService.GoBackAsync().ConfigureAwait(false);
                    break;
            }
            IsLoading = false;
        }

    }
}
