
using Kola.Infrastructure;
using Kola.Infrastructure.EventAggregator;
using Kola.Infrastructure.Helper;
using Kola.Infrastructure.Models;
using Kola.Infrastructure.Navigation;
using Kola.Infrastructure.UserAuthentication.Contracts;
using Kola.Starting.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using reewire.core.services;
using reewire.core.services.contracts.services;
using reewire.core.services.models;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Kola.Starting.ViewModels
{
    public class MerchantPaymentScanQrCodeViewModel : ViewModelBase
    {
        private readonly IAuthentication _authentication;
        private readonly IUserInformation _userInformation;
        private readonly IFeature _makepayment;

        private ZXing.Result _result;
        public ZXing.Result Result
        {
            get { return _result; }
            set
            {
                SetProperty(ref _result, value);
            }
        }
        private bool _isAnalyzing = true;
        public bool IsAnalyzing
        {
            get { return _isAnalyzing; }
            set
            {
                SetProperty(ref _isAnalyzing, value);
            }
        }

        private bool _isScanning = true;
        public bool IsScanning
        {
            get { return _isScanning; }
            set
            {
                SetProperty(ref _isScanning, value);
            }
        }

        public Field<string> MerchantId { get; set; } = new Field<string>();
        public Field<string> MerchantName { get; set; } = new Field<string>();
        public Field<string> Amount { get; set; } = new Field<string>();
        public Field<string> Currency { get; set; } = new Field<string>();

        private IEventAggregator _ea;
        public ICommand DisplayBurger => new DelegateCommand(OnDisplayBurger);
        public ICommand QRScanResultCommand => new DelegateCommand(QRScanResult);
        public ICommand GotoAcceptCommand => new DelegateCommand(GotoAccept);
        public ICommand GotoRejectCommand => new DelegateCommand(GotoReject);
        public MerchantPaymentScanQrCodeViewModel(INavigationService navigationService,
            IEventAggregator ea,
            IUserInformation userInformation,
            IAuthentication authentication,
            IFeature makepayment) : base(navigationService)
        {
            _ea = ea;
            _userInformation = userInformation ?? throw new ArgumentNullException(nameof(userInformation));
            _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
            _makepayment = makepayment ?? throw new ArgumentNullException(nameof(makepayment));
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            IsLoading = false;
            base.OnNavigatedTo(parameters);
            var popupId = parameters.GetValue<string>(Constants.PopupId);

            switch (popupId)
            {
                case "QR_CODE_PAYMENT":
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

        private void QRScanResult()
        {

            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    bool issucces = false;
                    string message = "";
                    if (!string.IsNullOrEmpty(Result.Text) && Result.Text.Contains("QRCodeData"))
                    {
                        string json = Result.Text.Replace("QRCodeData : ", "");
                        var obj = JsonConvert.DeserializeObject<QRCodeData>(json);

                        if (obj.PhoneNumber == _userInformation.UserName && obj.SystemId == ApiConstants.SystemId) // data match from user
                        {
                            issucces = true;
                            MerchantId.Value = obj.MerchantId;
                            MerchantName.Value = obj.MerchantName.ToUpperInvariant();
                            Amount.Value = obj.Amount.ToString();
                            Currency.Value = obj.Currency;
                            IsAnalyzing = false;
                            IsScanning = false;
                        }
                        else // data no match
                        {
                            message = TranslateManagerHelper.Convert("qrcode_is_not_yours");
                        }
                    }
                    else
                    {
                        message = TranslateManagerHelper.Convert("qrcode_not_reconize");
                    }

                    if (!issucces)
                    {
                        var parameters = new NavigationParameters
                        {
                            { Constants.PopupIsSucces, false },
                            { Constants.PopupMessage, message },
                            {Constants.PopupNextPage, FunctionName.MerchantPayment }
                        };

                        await NavigationService.NavigateAsync(PopupName.SuccessfullPopup, parameters).ConfigureAwait(false);
                        IsLoading = false;
                    }
                }
                catch (Exception ex)
                {
                    var parameters = new NavigationParameters
                        {
                            { Constants.PopupIsSucces, false },
                            { Constants.PopupMessage, TranslateManagerHelper.Convert("qrcode_error_occured") },
                            {Constants.PopupNextPage, FunctionName.MerchantPayment }
                        };

                    await NavigationService.NavigateAsync(PopupName.SuccessfullPopup, parameters).ConfigureAwait(false);
                    IsLoading = false;
                }
            });
        }

        private async void GotoAccept()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            string message = null;
            if (string.IsNullOrEmpty(MerchantId.Value))
            {
                message = TranslateManagerHelper.Convert("merchant_error");
            }
            else if (string.IsNullOrEmpty(Amount.Value) || decimal.Parse(Amount.Value) <= 0)
            {
                message = TranslateManagerHelper.Convert("amount_error");
            }
            else if (string.IsNullOrEmpty(Currency.Value))
            {
                message = TranslateManagerHelper.Convert("source_account_error");
            }

            if (!string.IsNullOrEmpty(message))
            {
                var parameters1 = new NavigationParameters
                        {
                            { Constants.PopupIsSucces, false },
                            { Constants.PopupMessage, message },
                            {Constants.PopupNextPage, FunctionName.MerchantPayment }
                        };
                await NavigationService.NavigateAsync(PopupName.SuccessfullPopup, parameters1).ConfigureAwait(false);
                IsLoading = false;
                return;
            }

            var parameters = new NavigationParameters
                {
                    { Constants.PopupId, "QR_CODE_PAYMENT" },
                };
            await NavigationService.NavigateAsync(PopupName.PinCodePopup, parameters).ConfigureAwait(false);
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

            AccountModel accountSelected;
            if (Currency.Value == _accountLRD.Currency)
            {
                accountSelected = _accountLRD;
            }
            else if (Currency.Value == _accountUSD.Currency)
            {
                accountSelected = _accountUSD;
            }
            else
            {
                var parameters1 = new NavigationParameters
                        {
                            { Constants.PopupIsSucces, false },
                            { Constants.PopupMessage, TranslateManagerHelper.Convert("operation_failed") },
                            {Constants.PopupNextPage, FunctionName.MerchantPayment }
                        };
                await NavigationService.NavigateAsync(PopupName.SuccessfullPopup, parameters1).ConfigureAwait(false);
                IsLoading = false;
                return;
            }

            // call service feature
            MerchantPaymentRequest mpr = new MerchantPaymentRequest()
            {
                TargetAccountNumber = accountSelected.AccountNumber
            };

            featureDTO f = new featureDTO()
            {
                FeatureName = "MERCHANT_PAYMENT",
                SenderId = ApiConstants.CountryPrefix + _userInformation.UserName,
                SenderPassword = _userInformation.Password,
                Amount = decimal.Parse(Amount.Value),
                TargetId = MerchantId.Value,
                jsonObjString = JsonConvert.SerializeObject(mpr)
            };

            bool issuccess = false;
            string message = "";

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

            var parameters = new NavigationParameters
                        {
                            { Constants.PopupIsSucces, issuccess },
                            { Constants.PopupMessage, message },
                            {Constants.PopupNextPage, FunctionName.MerchantPayment }
                        };
            await NavigationService.NavigateAsync(PopupName.SuccessfullPopup, parameters).ConfigureAwait(false);
            IsLoading = false;
        }

        private async void GotoReject()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
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
    }
}
