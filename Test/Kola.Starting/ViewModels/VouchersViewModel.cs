using Kola.Infrastructure;
using Kola.Infrastructure.EventAggregator;
using Kola.Infrastructure.Helper;
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
using System.IO;
using System.Text;
using System.Windows.Input;

namespace Kola.Starting.ViewModels
{
    public class VouchersViewModel : ViewModelBase
    {
        private readonly IApplications _applicationsService;
        private readonly IFeature _feature;
        private readonly IUserInformation _userInformation;
        private readonly IAppInfo _appInfo;
        private readonly IAuthentication _authentication;
        private readonly IQRCode _qrcode;

        public Field<bool> FirstStep { get; set; } = new Field<bool>(true);

        public Field<bool> SecondStep { get; set; } = new Field<bool>(false);

        public Field<bool> IsDisplayQrCode { get; set; } = new Field<bool>(false);
        public Field<string> VoucherNumber { get; set; } = new Field<string>();

        private ObservableCollection<VoucherModel> _items = new ObservableCollection<VoucherModel>();
        public ObservableCollection<VoucherModel> Items
        {
            get { return _items; }
            set
            {
                SetProperty(ref _items, value);
            }
        }

        private IEventAggregator _ea;
        public ICommand DisplayBurger => new DelegateCommand(OnDisplayBurger);
        public ICommand RegisterVoucherCommand => new DelegateCommand(RegisterVoucher);
        public ICommand AddVoucherCommand => new DelegateCommand(AddVoucher);
        public ICommand DisplayQrCodeCommand => new DelegateCommand<VoucherModel>(DisplayQrCode);
        public VouchersViewModel(INavigationService navigationService,
                                 IEventAggregator ea,
                                 IApplications applicationsService,
                                 IFeature feature,
                                 IAppInfo appInfo,
                                 IQRCode qrcode,
                                 IAuthentication authentication,
                                 IUserInformation userInformation) : base(navigationService)
        {
            _ea = ea;
            _applicationsService = applicationsService ?? throw new ArgumentNullException(nameof(applicationsService));
            _feature = feature ?? throw new ArgumentNullException(nameof(feature));
            _userInformation = userInformation ?? throw new ArgumentNullException(nameof(userInformation));
            _appInfo = appInfo ?? throw new ArgumentNullException(nameof(appInfo));
            _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
            _qrcode = qrcode ?? throw new ArgumentNullException(nameof(qrcode));

            Initialize();
        }

        private async void Initialize()
        {
            if (IsBusy || IsLoading)
            {
                // Message toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            Items = new ObservableCollection<VoucherModel>();

            var response = await _applicationsService.GetApplicationUserVouchers(_authentication.GetToken(), (long)_appInfo?.App?.Id, _userInformation.UserId);
            if (response != null && response?.isuccess == true)
            {
                foreach (var item in response.data)
                {
                    Items.Add(new VoucherModel
                    {
                        Amount = item.Value.ToString(),
                        Id = item.Id,
                        CurrencyId = item.CurrencyId,
                        QrCode = item.QrCode,
                        Currency = item.CurrencyCode,
                        VoucherNumber = item.Number,
                        Icon = ""
                    });
                }
            }
            else
            {
                string message;
                switch (response?.errcode)
                {
                    case "EXCEPTION":
                        message = TranslateManagerHelper.Convert("operation_failed");
                        break;
                    default:
                        message = !string.IsNullOrEmpty(response?.msg) ? response?.msg : TranslateManagerHelper.Convert("error_occured");
                        break;
                }

                var parameters = new NavigationParameters
                    {
                        { Constants.PopupIsSucces, false },
                        { Constants.PopupIsBeforeHome, false },
                        { Constants.PopupMessage, message },
                        {Constants.PopupNextPage, "" }
                    };
                await NavigationService.NavigateAsync(PopupName.SuccessfullPopup, parameters);
            }

            IsEmpty = Items.Count == 0;
            IsLoading = false;

        }

        private void OnDisplayBurger()
        {
            _ea.GetEvent<BurgerEvent>().Publish();
        }

        private void RegisterVoucher()
        {
            FirstStep.Value = false;
            SecondStep.Value = true;

        }

        private async void AddVoucher()
        {
            if (IsBusy || IsLoading)
            {
                // Message toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            VoucherNumber.HasError = false;

            if (string.IsNullOrEmpty(VoucherNumber.Value))
            {
                VoucherNumber.ErrorText = TranslateManagerHelper.Convert("voucher_number_error");
                VoucherNumber.HasError = true;
                IsLoading = false;
                return;
            }

            if (VoucherNumber.Value.Length != Constants.MaxLengthVourcherNumber)
            {
                VoucherNumber.ErrorText = TranslateManagerHelper.Convert("voucher_number_is_short");
                VoucherNumber.HasError = true;
                IsLoading = false;
                return;
            }

            // Call service
            VoucherRegisterRequest voucher = new VoucherRegisterRequest
            {
                vNumber = VoucherNumber.Value,
                vPhone = ApiConstants.CountryPrefix + _userInformation.UserName
            };

            featureDTO featureDTO = new featureDTO
            {
                FeatureName = "VOUCHER_REGISTER",
                Amount = 0,
                SenderId = ApiConstants.CountryPrefix + _userInformation.UserName,
                SenderPassword = _userInformation.Password,
                TargetId = "",
                jsonObjString = JsonConvert.SerializeObject(voucher)
            };

            bool isSuccess = false;
            string message;
            var response = await _feature.Execute<featureDTO, voucherdto>(_authentication.GetToken(), featureDTO, ApiConstants.SystemId);
            if (response != null && response?.isuccess == true)
            {
                isSuccess = true;
                message = !string.IsNullOrEmpty(response?.msg) ? response?.msg : TranslateManagerHelper.Convert("operation_successfully");
            }
            else
            {
                switch (response?.errcode)
                {
                    case "EXCEPTION":
                        message = TranslateManagerHelper.Convert("operation_failed");
                        break;
                    default:
                        message = !string.IsNullOrEmpty(response?.msg) ? response?.msg : TranslateManagerHelper.Convert("error_occured");
                        break;
                }
            }

            var parameters = new NavigationParameters
                    {
                        { Constants.PopupIsSucces, isSuccess },
                        { Constants.PopupIsBeforeHome, false },
                        { Constants.PopupMessage, message },
                        {Constants.PopupNextPage, FunctionName.Vouchers }
                    };
            await NavigationService.NavigateAsync(PopupName.SuccessfullPopup, parameters);

            if (isSuccess)
            {
                VoucherNumber.Value = "";
                SecondStep.Value = false;
                FirstStep.Value = true;
            }

            IsLoading = false;
        }

        private async void DisplayQrCode(VoucherModel voucherModel)
        {
            if (IsBusy || IsLoading)
            {
                // message toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            VoucherCreateRequestData obj = JsonConvert.DeserializeObject<VoucherCreateRequestData>(voucherModel.QrCode);
            
            var datagenerate = new QRCodeVourcherData
            {
                Amount = voucherModel.Amount,
                Currency = voucherModel.Currency,
                Number = voucherModel.VoucherNumber,
                UserName = ApiConstants.CountryPrefix + _userInformation.UserName,
                SystemId = ApiConstants.SystemId,
                Owner = $"{obj.Fname} {obj.Lname}",
                CompanyId = obj.Company,
                CompanyName = obj.CompanyName,
                IdentificationTypeId = obj.IdentificationTypeId,
                IdNumber = obj.IdNUmber,
                PhoneNumber = obj.vPhone
            };

            var qrcodedata = new QRcodeRequest();
            qrcodedata.Type = "Text";
            qrcodedata.Separator = string.Empty;
            qrcodedata.Parameters = new List<KeyValuePair<string, string>>
                    {
                          new KeyValuePair<string, string>("QRCodeData" ,JsonConvert.SerializeObject(datagenerate))
                    };

            var qc = await _qrcode.Generate<QRcodeRequest>(_authentication.GetToken(), qrcodedata);
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
                Code1 = voucherModel.VoucherNumber,
                //Code2 = voucherModel.Amount,
                //Currency = voucherModel.Currency,
                QrCodeImage = qrImage,
                IsDisplayIcon = true,
                Title = TranslateManagerHelper.Convert("scan_me").ToUpperInvariant(),
                HigthTitle = "vourchers"
            };

            var Parameters = new NavigationParameters()
            {
                {Constants.QrCodeId, "DISPLAY_QRCODE"},
                {Constants.QrCodeResponseData, data}
            };

            await NavigationService.NavigateAsync(FunctionName.GenerateQrCode, Parameters);
            IsLoading = false;
        }

    }
}
