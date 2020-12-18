using DryIoc;
using Kola.Infrastructure;
using Kola.Infrastructure.Helper;
using Kola.Infrastructure.Navigation;
using Kola.Infrastructure.UserAuthentication;
using Kola.Infrastructure.UserAuthentication.Contracts;
using Kola.Services.Infrastructure.LocalStorage.Contract;
using Newtonsoft.Json;
using Plugin.Fingerprint;
using Prism.Commands;
using Prism.Navigation;
using reewire.core.services;
using reewire.core.services.contracts.services;
using reewire.core.services.models;
using reewire.core.services.services;
using System;
using System.Windows.Input;

namespace Kola.Starting.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IUserInformationLocalStorage _userInformationLocalStorage;
        private readonly IAuthentication _authentication;
        private readonly IFeature _checkmsisdn;
        private readonly IContainer _container;

        public Field<string> NextText { get; set; } = new Field<string>("next");

        public Field<bool> IsRemember { get; set; } = new Field<bool>();
        private Field<bool> IsFingerLogin { get; set; } = new Field<bool>(false);
        public Field<bool> FingerPrintAvailable { get; set; } = new Field<bool>(false);
        public Field<string> PhoneNumber { get; set; } = new Field<string>();
        public Field<string> Message { get; set; } = new Field<string>();
        public Field<int> PhoneNumberLength { get; set; } = new Field<int>(Constants.MaxLengthPhoneNumber);

        public ICommand GotoFingerPrintCommand => new DelegateCommand(GotoFingerPrint);
        public ICommand GotoPageCommand => new DelegateCommand<string>(OnGotoPageCommand);
        public ICommand GotoNextCommand => new DelegateCommand(GotoNext);

        public LoginViewModel(INavigationService navigationService,
                               IContainer container,
                               IUserInformationLocalStorage userInformationLocalStorage,
                               IAuthentication authentication,
                               IFeature checkmsisdn) : base(navigationService, false)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _userInformationLocalStorage = userInformationLocalStorage ?? throw new ArgumentNullException(nameof(userInformationLocalStorage));
            _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
            _checkmsisdn = checkmsisdn ?? throw new ArgumentNullException(nameof(Feature));

            Initialize();
        }

        private async void GotoNext()
        {
            Message.HasError = false;
            PhoneNumber.HasError = false;
            if (string.IsNullOrEmpty(PhoneNumber.Value))
            {
                PhoneNumber.ErrorText = TranslateManagerHelper.Convert("username_error");
                PhoneNumber.HasError = true;
                return;
            }

            if (PhoneNumber.Value.Length != PhoneNumberLength.Value)
            {
                PhoneNumber.ErrorText = TranslateManagerHelper.Convert("username_is_short");
                PhoneNumber.HasError = true;
                return;
            }

            if (IsBusy || IsLoading)
            {
                // Message toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;


            // callservice to check phonenumber if ok, go to otp view          
            MsisdnRequest mr = new MsisdnRequest();
            mr.Msisdn = ApiConstants.CountryPrefix + PhoneNumber.Value;          

            featureDTO fb = new featureDTO
            {
                FeatureName = "CHECK_MSISDN",
                SenderId = "",
                SenderPassword = "",
                TargetId = "",
                Amount = 0,
                jsonObjString = JsonConvert.SerializeObject(mr)
            };

            var response = await _checkmsisdn.Execute<featureDTO, MsisdnResponse>(_authentication.GetToken(), fb, Infrastructure.ApiConstants.SystemId);
            if (response != null)
            {
                if (response.isuccess)
                {
                    MsisdnResponse r = response.data;
                    if (r.Exists)
                    {
                        if (IsRemember.Value)
                        {
                            _userInformationLocalStorage.SetRememberMe(IsRemember.Value);
                            _userInformationLocalStorage.SetUserName(PhoneNumber.Value);
                        }
                        else
                        {
                            _userInformationLocalStorage.Remove("RememberMe");
                            _userInformationLocalStorage.Remove("UserName");
                        }

                        IUserInformation user = new UserInformation(PhoneNumber.Value);
                        _container.RegisterInstance(user);

                        if (_userInformationLocalStorage.FingerPrint && _userInformationLocalStorage.FUserName != user.UserName)
                        {
                            _userInformationLocalStorage.SetFingerPrintForThisInstance(false);
                        }                       

                        var param = new NavigationParameters()
                                    {
                                        {"FingerLogin", IsFingerLogin.Value }
                                    };

                        await NavigationService.NavigateAsync(FunctionName.PinCode, param);
                    }
                    else
                    {
                        switch (response.errcode)
                        {                           
                            case "EXCEPTION":
                                Message.ErrorText = TranslateManagerHelper.Convert("operation_failed");
                                break;
                            default:
                                Message.ErrorText = !string.IsNullOrEmpty(response.msg) ? response.msg : TranslateManagerHelper.Convert("user_not_found");
                                break;
                        }
                        
                        Message.HasError = true;
                        IsFingerLogin.Value = false;
                    }
                }
                else
                {
                    switch (response.errcode)
                    {
                        case "ForcePincodeChange":

                            if (IsRemember.Value)
                            {
                                _userInformationLocalStorage.SetRememberMe(IsRemember.Value);
                                _userInformationLocalStorage.SetUserName(PhoneNumber.Value);
                            }
                            else
                            {
                                _userInformationLocalStorage.Remove("RememberMe");
                                _userInformationLocalStorage.Remove("UserName");
                            }

                            IUserInformation user = new UserInformation(PhoneNumber.Value);
                            _container.RegisterInstance(user);
                            Message.ErrorText = "";

                            if (_userInformationLocalStorage.FingerPrint && _userInformationLocalStorage.FUserName != user.UserName)
                            {
                                _userInformationLocalStorage.SetFingerPrintForThisInstance(false);
                            }

                            await NavigationService.NavigateAsync(FunctionName.ForcePincodeChange);
                            break;
                        case "EXCEPTION":
                            Message.ErrorText = TranslateManagerHelper.Convert("operation_failed");
                            break;
                        default:
                            Message.ErrorText = !string.IsNullOrEmpty(response.msg) ? response.msg : TranslateManagerHelper.Convert("user_not_found");
                            break;
                    }
                    Message.HasError = true;
                    IsFingerLogin.Value = false;
                }
            }
            else
            {
                //pour afficher message error, utiliser :
                Message.ErrorText = !string.IsNullOrEmpty(response?.msg) ? response.msg : TranslateManagerHelper.Convert("user_not_found");
                Message.HasError = true;
                IsFingerLogin.Value = false;
            }

            IsLoading = false;
        }

        private async void GotoFingerPrint()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                return;
            }

            IsLoading = true;
            var result = await CrossFingerprint.Current.AuthenticateAsync(TranslateManagerHelper.Convert("proove_you_have_finger"));
            if (result.Authenticated)
            {
                PhoneNumber.Value = _userInformationLocalStorage.FUserName;
                IsFingerLogin.Value = true;
                IsLoading = false;
                GotoNext();
            }
            else
            {
                IsLoading = false;
            }
        }

        protected void Initialize()
        {
            IsLoading = true;
            if (_userInformationLocalStorage.RememberMe)
            {
                IsRemember.Value = _userInformationLocalStorage.RememberMe;
                PhoneNumber.Value = _userInformationLocalStorage.UserName;
            }
            GetFingerPrintAvailability();
            IsLoading = false;
        }

        private async void GetFingerPrintAvailability()
        {
            var result = await CrossFingerprint.Current.IsAvailableAsync(false);
            if (result)
            {
                if (_userInformationLocalStorage.FingerPrint) FingerPrintAvailable.Value = true;
                else FingerPrintAvailable.Value = false;
            }
            else FingerPrintAvailable.Value = false;
        }

        private async void OnGotoPageCommand(string p)
        {
            if (IsBusy || IsLoading)
            {
                return;
            }

            IsLoading = true;
            await NavigationService.NavigateAsync($"{p}");
            IsLoading = false;
        }
    }
}
