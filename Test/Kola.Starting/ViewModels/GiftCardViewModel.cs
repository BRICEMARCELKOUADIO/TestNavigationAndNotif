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
using Prism.Services;
using reewire.core.services;
using reewire.core.services.utils;
using reewire.core.services.contracts.application;
using reewire.core.services.contracts.services;
using reewire.core.services.models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Kola.Infrastructure.Setting.Contract;
using System.IO;
using System;
using Kola.Infrastructure.Extensions;

namespace Kola.Starting.ViewModels
{
    public class GiftCardViewModel : ViewModelBase
    {
        private readonly IAppInfo _appInfo;
        private IApplications _applicationsService;
        private readonly IFeature _giftcardservice;
        // Create gift card
        private AccountModel _accountSelected = new AccountModel();
        public Field<string> Merchant { get; set; } = new Field<string>();
        public Field<string> Amount { get; set; } = new Field<string>();
        public Field<int> AmountMaxLength { get; set; } = new Field<int>(Constants.MaxLengthAmount);
        public Field<string> CurrentCurrency { get; set; } = new Field<string>();
        public Field<bool> HasAccountSelected { get; set; } = new Field<bool>(false);
        public Field<string> CurrentAccountName { get; set; } = new Field<string>();
        public Field<string> CurrentBalance { get; set; } = new Field<string>();
        public Field<string> ReceiverPhoneNumber { get; set; } = new Field<string>();
        public Field<int> PhoneNumberMaxLength { get; set; } = new Field<int>(Constants.MaxLengthPhoneNumber);

        // add gift card
        public Field<string> GiftCardNumber { get; set; } = new Field<string>();


        public Field<bool> FirstStep { get; set; } = new Field<bool>(true);
        public Field<bool> SecondStep { get; set; } = new Field<bool>(false);
        public Field<bool> ThirdStep { get; set; } = new Field<bool>(false);

        private ObservableCollection<GiftCardModel> _items = new ObservableCollection<GiftCardModel>();
        public ObservableCollection<GiftCardModel> Items
        {
            get { return _items; }
            set
            {
                SetProperty(ref _items, value);
            }
        }

        private IEventAggregator _ea;
        private IOrganizations _organizations;
        private IAuthentication _authentication;
        private IFeature _creategiftcardservice;
        private IUserInformation _userInformation;
        private IQRCode _qRCode;
        public ICommand DisplayBurger => new DelegateCommand(OnDisplayBurger);
        public ICommand GotoCreateGiftCardCommand => new DelegateCommand(GotoCreateGiftCard);
        public ICommand GotoAddGiftCardCommand => new DelegateCommand(GotoAddGiftCard);
        public ICommand GotoOpenAccountPopupCommand => new DelegateCommand(GotoOpenAccountPopup);
        public ICommand OnOpenMerchantPopupCommand => new DelegateCommand(OnOpenMerchantPopup);

        public ICommand AddGiftCardCommand => new DelegateCommand(AddGiftCard);
        public ICommand CreateGiftCardCommand => new DelegateCommand(CreateGiftCard);
        public ICommand DisplayQrCodeCommand => new DelegateCommand<GiftCardModel>(DisplayQrCode);

        public GiftCardViewModel(INavigationService navigationService,
                                 IEventAggregator ea,
                                 IAuthentication authentication,
                                 IFeature creategiftcardservice,
                                 IUserInformation userInformation,
                                 IOrganizations organizations,
                                 IQRCode qRCode,
                                 IAppInfo appInfo,
                                 IApplications applicationsService,
                                 IFeature giftcardservice) : base(navigationService)
        {
            _ea = ea;
            _authentication = authentication ?? throw new System.ArgumentNullException(nameof(authentication));
            _creategiftcardservice = creategiftcardservice ?? throw new System.ArgumentNullException(nameof(userInformation));
            _userInformation = userInformation ?? throw new System.ArgumentNullException(nameof(userInformation));
            _organizations = organizations ?? throw new System.ArgumentNullException(nameof(organizations));
            _qRCode = qRCode ?? throw new System.ArgumentNullException(nameof(qRCode));
            _appInfo = appInfo ?? throw new System.ArgumentNullException(nameof(appInfo));
            _applicationsService = applicationsService ?? throw new System.ArgumentNullException(nameof(applicationsService));
            _giftcardservice = giftcardservice ?? throw new System.ArgumentNullException(nameof(giftcardservice));
            InitializeInput();
            LoadGiftCard();
        }

        private async void LoadGiftCard()
        {
            if (IsBusy || IsLoading)
            {
                // Message toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            var listr = await _applicationsService.GetApplicationUserGiftcards(_authentication.GetToken(), _appInfo.App.Id, _userInformation.UserId);
            if (listr != null && listr?.isuccess == true)
            {
                Items = listr.data.Select(x => new GiftCardModel { GiftCardNumber = x.Number, Currency = x.CurrencyCode, Amount = x.Value.ToString(), QRCodeData = x.QrCode }).ToObservableCollection();
            }
            else
            {
                string message;
                switch (listr?.errcode)
                {
                    case "EXCEPTION":
                        message = TranslateManagerHelper.Convert("operation_failed");
                        break;
                    default:
                        message = !string.IsNullOrEmpty(listr.msg) ? listr?.msg : TranslateManagerHelper.Convert("error_contact_support");
                        break;
                }
                var parameters = new NavigationParameters
                    {
                        { Constants.PopupIsSucces, false },
                        { Constants.PopupIsBeforeHome, false },
                        { Constants.PopupMessage, message },
                        {Constants.PopupNextPage, FunctionName.GiftCard }
                    };
                await NavigationService.NavigateAsync(PopupName.SuccessfullPopup, parameters).ConfigureAwait(false);

            }
            IsEmpty = Items.Count == 0;
            IsLoading = false;
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
                case "MERCHANT_POPUP":
                    var merch = parameters.GetValue<PikerItem>(Constants.PopupResponseData);
                    Merchant.Id = merch?.Id;
                    Merchant.Value = merch?.Name_1;
                    break;
                case "GIFTCARD_PINCODE_POPUP":
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

        private async void OnOpenMerchantPopup()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            ObservableCollection<PikerItem> list = new ObservableCollection<PikerItem>();
            var listr = await _applicationsService.GetApplicationMerchants(_authentication.GetToken(), _appInfo.App.Id);
            if (listr?.isuccess == true)
            {
                list = listr.data.Select(x => new PikerItem { Id = x.UserId, Name_1 = x.Fname + " " + x.Lname }).ToObservableCollection();
            }


            var parameters = new NavigationParameters
                {
                    { Constants.PopupId, "MERCHANT_POPUP" },
                    { Constants.PopupTitle, TranslateManagerHelper.Convert("select_merchant").ToUpperInvariant() },
                   // {Constants.PopupIcon, "icon-add-user"},
                    {Constants.PopupCurrentData, list.FirstOrDefault(x => x.Id == Merchant.Id)},
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

        private void GotoCreateGiftCard()
        {
            FirstStep.Value = false;
            SecondStep.Value = false;
            ThirdStep.Value = true;
        }

        private void GotoAddGiftCard()
        {
            FirstStep.Value = false;
            ThirdStep.Value = false;
            SecondStep.Value = true;
        }


        private void AddGiftCard()
        {
            if (IsBusy || IsLoading)
            {
                // Message toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            GiftCardNumber.HasError = false;

            if (string.IsNullOrEmpty(GiftCardNumber.Value))
            {
                GiftCardNumber.ErrorText = TranslateManagerHelper.Convert("gitf_card_number_error");
                GiftCardNumber.HasError = true;
                IsLoading = false;
                return;
            }

            // call service to add gift card

            Items.Add(new GiftCardModel { GiftCardNumber = GiftCardNumber.Value, Amount = "57", Currency = "USD" });

            InitializeInput();
            GoBacckToStpe1();
            IsLoading = false;
        }

        private void GoBacckToStpe1()
        {
            ThirdStep.Value = false;
            SecondStep.Value = false;
            FirstStep.Value = true;
        }

        private async void CreateGiftCard()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            ReceiverPhoneNumber.HasError = Amount.HasError = CurrentCurrency.HasError = Merchant.HasError = false;

            if (string.IsNullOrEmpty(ReceiverPhoneNumber.Value))
            {
                ReceiverPhoneNumber.ErrorText = TranslateManagerHelper.Convert("phone_number_error");
                ReceiverPhoneNumber.HasError = true;
                IsLoading = false;
                return;
            }

            if (ReceiverPhoneNumber.Value.Length != PhoneNumberMaxLength.Value)
            {
                ReceiverPhoneNumber.ErrorText = TranslateManagerHelper.Convert("phone_number_is_short");
                ReceiverPhoneNumber.HasError = true;
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

            //if (string.IsNullOrEmpty(Merchant.Value))
            //{
            //    Merchant.ErrorText = TranslateManagerHelper.Convert("merchant_error");
            //    Merchant.HasError = true;
            //    IsLoading = false;
            //    return;
            //}



            var parameters = new NavigationParameters
                {
                    { Constants.PopupId, "GIFTCARD_PINCODE_POPUP" },
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
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;


            var data = new QRCodeGiftKardData
            {
                SystemId = ApiConstants.SystemId,
                Amount = Amount.Value,
                PhoneNumber = ApiConstants.CountryPrefix + ReceiverPhoneNumber.Value,
                Currency = _accountSelected.Currency,
                CompanyId = !string.IsNullOrEmpty(Merchant.Id) ? Merchant.Id : "",
                CompanyName = !string.IsNullOrEmpty(Merchant.Value) ? Merchant.Value : ""
            };

            GiftCardCreationRequest cash = new GiftCardCreationRequest
            {
                SourceAccountNumber = _accountSelected.AccountNumber,
                QrCode = JsonConvert.SerializeObject(data)
            };

            featureDTO f = new featureDTO
            {
                FeatureName = "GIFT_CARD_CREATE",
                Amount = decimal.Parse(Amount.Value),
                SenderId = ApiConstants.CountryPrefix + _userInformation.UserName,
                SenderPassword = _userInformation.Password,
                TargetId = ApiConstants.CountryPrefix + ReceiverPhoneNumber.Value,
                jsonObjString = JsonConvert.SerializeObject(cash)
            };

            var result = await _giftcardservice.Execute<featureDTO, GiftCardCreationResponse>(_authentication.GetToken(), f, ApiConstants.SystemId);
            NavigationParameters parameters = null;
            string message = "";
            bool isSuccess = false;
            if (result != null)
            {
                if (result.isuccess)
                {
                    isSuccess = true;
                    message = TranslateManagerHelper.Convert("operation_successfully");
                    // init value 
                    var ai = result.data.AccountUpdate;
                    UpdateUserAccount(new AccountModel
                    {
                        AccountNumber = ai.AccountNumber,
                        Balance = ai.Balance.ToString(),
                        Currency = ai.CurrencyISO,
                        Type = ai.AccountType,
                    });
                    InitializeInput();
                    GoBacckToStpe1();
                }
                else
                {
                    switch (result.errcode)
                    {
                        case "EXCEPTION":
                            message = TranslateManagerHelper.Convert("operation_failed");
                            break;
                        default:
                            message = !string.IsNullOrEmpty(result.msg) ? result.msg : TranslateManagerHelper.Convert("cashin_failed");
                            break;
                    }

                    isSuccess = false;
                }
            }
            else
            {
                // TODO : what message do we send ?
                isSuccess = false;

                switch (result.errcode)
                {
                    case "EXCEPTION":
                        message = TranslateManagerHelper.Convert("operation_failed");
                        break;
                    default:
                        message = TranslateManagerHelper.Convert("error_contact_support");
                        break;
                }
            }

            parameters = new NavigationParameters
                    {
                        { Constants.PopupIsSucces, isSuccess },
                        { Constants.PopupIsBeforeHome, false },
                        { Constants.PopupMessage, message },
                        {Constants.PopupNextPage, FunctionName.GiftCard }
                    };

            //InitializeInput();
            //GoBacckToStpe1();
            await NavigationService.NavigateAsync(PopupName.SuccessfullPopup, parameters).ConfigureAwait(false);
            IsLoading = false;
        }

        private async void DisplayQrCode(GiftCardModel GiftCardModel)
        {
            if (IsBusy || IsLoading)
            {
                // message toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            QRCodeGiftKardData obj = JsonConvert.DeserializeObject<QRCodeGiftKardData>(GiftCardModel.QRCodeData);
            obj.Number = GiftCardModel.GiftCardNumber;
            obj.Amount = GiftCardModel.Amount;

            // call genreate qrcode service
            var qrcodedata = new QRcodeRequest();
            qrcodedata.Type = "Text";
            qrcodedata.Separator = string.Empty;
            qrcodedata.Parameters = new List<KeyValuePair<string, string>>
                    {
                          new KeyValuePair<string, string>("QRCodeData" ,JsonConvert.SerializeObject(obj))
                    };
            /* 
             * Call Simple API here
             */
            var qc = await _qRCode.Generate<QRcodeRequest>(_authentication.GetToken(), qrcodedata);
            if (qc == null || qc?.isuccess == false)
            {
                string message = "";
                switch (qc?.errcode)
                {
                    case "EXCEPTION":
                        TranslateManagerHelper.Convert("error_occured");
                        break;
                    default:
                        message = !string.IsNullOrEmpty(qc?.msg) ? qc.msg : TranslateManagerHelper.Convert("qrcode_error_occured");
                        break;
                }

                var parameters = new NavigationParameters
                                    {
                                        { Constants.PopupIsSucces, false },
                                        { Constants.PopupMessage, message },
                                        { Constants.PopupIsBeforeHome, false },
                                        {Constants.PopupNextPage, "" }
                                    };
                await NavigationService.NavigateAsync(PopupName.SuccessfullPopup, parameters).ConfigureAwait(false);
                IsLoading = false;
                return;
            }

            var qrImage = Xamarin.Forms.ImageSource.FromStream(
               () => new MemoryStream(Convert.FromBase64String(qc.data)));

            var data = new DisplayQrCodeModel()
            {
                Code1 = GiftCardModel.GiftCardNumber,
                //Code2 = GiftCardModel.Amount,
                //Currency = obj.Currency,
                IsDisplayIcon = false,
                QrCodeImage = qrImage,
                Title = TranslateManagerHelper.Convert("scan_me"),
                HigthTitle = TranslateManagerHelper.Convert("gift_card"),
                CompanyId = obj.CompanyId,
                CompanyName = obj.CompanyName
            };

            var Parameters = new NavigationParameters()
            {
                {Constants.QrCodeId, "DISPLAY_QRCODE"},
                {Constants.QrCodeResponseData, data}
            };

            await NavigationService.NavigateAsync(FunctionName.GenerateQrCode, Parameters);
            IsLoading = false;
        }

        private void InitializeInput()
        {
            _accountSelected = new AccountModel();
            Merchant = new Field<string>("");
            Amount = new Field<string>("");
            ReceiverPhoneNumber = new Field<string>("");
            CurrentAccountName = new Field<string>("");
            CurrentBalance = new Field<string>("");
            CurrentCurrency = new Field<string>("");
            GiftCardNumber = new Field<string>("");
        }
    }
}
