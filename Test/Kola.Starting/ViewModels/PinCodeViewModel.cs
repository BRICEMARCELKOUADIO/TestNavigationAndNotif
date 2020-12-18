
using Kola.Infrastructure;
using Kola.Infrastructure.Controls.ToastMessage;
using Kola.Infrastructure.Helper;
using Kola.Infrastructure.Models;
using Kola.Infrastructure.Navigation;
using Kola.Infrastructure.UserAuthentication.Contracts;
using Kola.Services.Infrastructure.LocalStorage.Contract;
using Prism.Commands;
using Prism.Navigation;
using reewire.core.services;
using reewire.core.services.contracts.services;
using reewire.core.services.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Plugin.FirebasePushNotification;

namespace Kola.Starting.ViewModels
{
    public class PinCodeViewModel : ViewModelBase
    {
        private readonly IUserInformationLocalStorage _userInformationLocalStorage;
        private IUserInformation _userInformation;
        private IFeature _checkpincode;
        private IAuthentication _authentication;

        private string _pincode = "";
        public Field<string> Message { get; set; } = new Field<string>();

        public Field<string> Number_1 { get; set; } = new Field<string>(" ");
        public Field<string> Number_2 { get; set; } = new Field<string>(" ");
        public Field<string> Number_3 { get; set; } = new Field<string>(" ");
        public Field<string> Number_4 { get; set; } = new Field<string>(" ");
        public Field<string> Number_5 { get; set; } = new Field<string>(" ");

        public Field<string> Position_0_0 { get; set; } = new Field<string>();
        public Field<string> Position_0_1 { get; set; } = new Field<string>();
        public Field<string> Position_0_2 { get; set; } = new Field<string>();
        public Field<string> Position_1_0 { get; set; } = new Field<string>();
        public Field<string> Position_1_1 { get; set; } = new Field<string>();
        public Field<string> Position_1_2 { get; set; } = new Field<string>();
        public Field<string> Position_2_0 { get; set; } = new Field<string>();
        public Field<string> Position_2_1 { get; set; } = new Field<string>();
        public Field<string> Position_2_2 { get; set; } = new Field<string>();
        public Field<string> Position_3_1 { get; set; } = new Field<string>();

        public ICommand OnTapAddNumberPositionCommand => new DelegateCommand<string>(OnTapAddNumberPosition);
        public ICommand OnTapRemoveNumberPositionCommand => new DelegateCommand(OnTapRemoveNumberPosition);

        //public ICommand GotoHome => new Command<string>(OnGotoHome);
        public PinCodeViewModel(INavigationService navigationService, IUserInformation userInformation,
            IFeature checkpincode, IAuthentication authentication, IUserInformationLocalStorage userInformationLocalStorage) : base(navigationService, false)
        {
            _userInformation = userInformation ?? throw new ArgumentNullException(nameof(userInformation));
            _checkpincode = checkpincode ?? throw new ArgumentNullException(nameof(checkpincode));
            _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
            _userInformationLocalStorage = userInformationLocalStorage ?? throw new ArgumentNullException(nameof(userInformationLocalStorage));

            Initialize();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            var value = parameters.GetValue<bool>(Constants.FingerLogin);
            switch (value)
            {
                case true:
                    {
                        var passwordarray = _userInformationLocalStorage.FPassword.ToCharArray();
                        IsLoading = false;
                        foreach (var item in passwordarray)
                        {
                            OnTapAddNumberPosition(item.ToString());
                        }
                    }
                    break;
                case false:
                    break;
                default:
                    break;
            }

            var notif = parameters.GetValue<string>(Constants.PopupId);
            switch(notif)
            {
                case "NOTIFICATION_ACTIVE_POPUP":
                    {
                        var ok = parameters.GetValue<bool>(Constants.PopupResponseData);
                        GotoHomeWithAciveNotification(ok);
                        break;
                    }
                default:
                    break;
            }
        }

        private void Initialize()
        {
            int[] numbers = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            Random rnd = new Random();
            int[] randomNumbers = numbers.OrderBy(x => rnd.Next()).ToArray();

            Position_0_0.Value = randomNumbers[0].ToString();
            Position_0_1.Value = randomNumbers[1].ToString();
            Position_0_2.Value = randomNumbers[2].ToString();

            Position_1_0.Value = randomNumbers[3].ToString();
            Position_1_1.Value = randomNumbers[4].ToString();
            Position_1_2.Value = randomNumbers[5].ToString();

            Position_2_0.Value = randomNumbers[6].ToString();
            Position_2_1.Value = randomNumbers[7].ToString();
            Position_2_2.Value = randomNumbers[8].ToString();

            Position_3_1.Value = randomNumbers[9].ToString();
        }

        private async void GotoHomeWithAciveNotification(bool ok)
        {
            if (IsBusy || IsLoading)
            {
                // Message toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            if(ok)
            {
                _userInformationLocalStorage.UnSubscribeNotification();
                _userInformationLocalStorage.SubscribeNotification(_userInformation.UserName);
            }
           
            await NavigationService.NavigateAsync($"/CustomMasterDetailPageView/NavigationPage/{FunctionName.Home}").ConfigureAwait(false);
            IsLoading = false;
        }
        
        private async void OnTapAddNumberPosition(string value)
        {
            Message.HasError = false;
            Message.ErrorText = "";

            if (_pincode.Length >= Constants.MaxLenthPinCode)
                return;

            _pincode += value.ToString();
            switch (_pincode.Length)
            {
                case 1:
                    Number_1.Value = "*";
                    break;
                case 2:
                    Number_2.Value = "*";
                    break;
                case 3:
                    Number_3.Value = "*";
                    break;
                case 4:
                    Number_4.Value = "*";
                    break;
                case 5:
                    Number_5.Value = "*";
                    break;
                default:
                    break;
            }
            if (_pincode.Length >= Constants.MaxLenthPinCode)
            {
                if (IsBusy || IsLoading)
                {
                    // Message toast
                    ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                    return;
                }
                IsLoading = true;
                // call service check_pincode

                featureDTO fb = new featureDTO
                {
                    FeatureName = "CHECK_PINCODE",
                    SenderId = ApiConstants.CountryPrefix + _userInformation.UserName,
                    SenderPassword = _pincode,
                    TargetId = "",
                    Amount = 0,
                    jsonObjString = ""
                };
                  
                var response = await _checkpincode.Execute<featureDTO, CheckPincodeResponse>(_authentication.GetToken(), fb, ApiConstants.SystemId);
                if (response != null && response.isuccess)
                {
                    _userInformation.SetPassword(_pincode);
                    _userInformation.SetUserId(response.data.Id);
                    _authentication.Connect();
                    CheckPincodeResponse cp = response.data;
                    _userFullName = $"{cp.Fname} {cp.Lname}";
                    foreach (AccountInfo ai in cp.AccountUpdate)
                    {
                        UpdateUserAccount(new AccountModel
                        {
                            AccountNumber = ai.AccountNumber,
                            Balance = ai.Balance.ToString(),
                            Currency = ai.CurrencyISO,
                            Type = ai.AccountType
                        });
                    }

                    if (_userInformationLocalStorage.HasSubscribeNotification)
                    {
                        if (_userInformationLocalStorage.UserForNotification != _userInformation.UserName)
                        {
                            // Call popup message action
                            var parameters = new NavigationParameters
                                {
                                    { Constants.PopupId, "NOTIFICATION_ACTIVE_POPUP" },
                                    {Constants.PopupMessage, TranslateManagerHelper.Convert("do_you_want").ToUpper() },
                                    {Constants.PopupMessage2, TranslateManagerHelper.Convert("to_receive_your_notifications_on_this_device").ToUpper() },
                                };
                            await NavigationService.NavigateAsync(PopupName.MessageWithActionPopup, parameters).ConfigureAwait(false);

                            IsLoading = false;
                            return;                            
                        }
                    }
                    else
                    {
                        _userInformationLocalStorage.SubscribeNotification(_userInformation.UserName);
                    }

                    await NavigationService.NavigateAsync($"/CustomMasterDetailPageView/NavigationPage/{FunctionName.Home}").ConfigureAwait(false);
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                        OnTapRemoveNumberPosition();
                    
                    switch (response?.errcode)
                    {
                        case "EXCEPTION":
                            Message.ErrorText = TranslateManagerHelper.Convert("operation_failed");
                            break;
                        default:
                            Message.ErrorText = !string.IsNullOrEmpty(response?.msg) ? response?.msg : TranslateManagerHelper.Convert("error_occured");
                            break;
                    }
                    Message.HasError = true;
                }

                IsLoading = false;
            }
        }

        private void OnTapRemoveNumberPosition()
        {
            int index = _pincode.Length;
            if (index == 0)
                return;

            _pincode = _pincode.Remove(index - 1);
            switch (_pincode.Length)
            {
                case 0:
                    Number_1.Value = " ";
                    Number_2.Value = " ";
                    Number_3.Value = " ";
                    Number_4.Value = " ";
                    Number_5.Value = " ";
                    break;
                case 1:
                    Number_1.Value = "*";
                    Number_2.Value = " ";
                    Number_3.Value = " ";
                    Number_4.Value = " ";
                    Number_5.Value = " ";
                    break;
                case 2:
                    Number_1.Value = "*";
                    Number_2.Value = "*";
                    Number_3.Value = " ";
                    Number_4.Value = " ";
                    Number_5.Value = " ";
                    break;
                case 3:
                    Number_1.Value = "*";
                    Number_2.Value = "*";
                    Number_3.Value = "*";
                    Number_4.Value = " ";
                    Number_5.Value = " ";
                    break;
                case 4:
                    Number_1.Value = "*";
                    Number_2.Value = "*";
                    Number_3.Value = "*";
                    Number_4.Value = "*";
                    Number_5.Value = " ";
                    break;
                case 5:
                    Number_1.Value = "*";
                    Number_2.Value = "*";
                    Number_3.Value = "*";
                    Number_4.Value = "*";
                    Number_5.Value = "*";
                    break;
                default:
                    break;

            }

        }

        //private async void OnGotoHome(string p)
        //{
        //    if (IsBusy || IsLoading)
        //    {
        //        ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
        //        return;
        //    }
        //    await NavigationService.NavigateAsync($"/CustomMasterDetailPageView/NavigationPage/{p}");
        //    IsBusy = false;
        //}
    }
}
