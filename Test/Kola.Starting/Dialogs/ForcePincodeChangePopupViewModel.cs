using Kola.Infrastructure;
using Kola.Infrastructure.Helper;
using Kola.Infrastructure.Navigation;
using Kola.Infrastructure.UserAuthentication.Contracts;
using Kola.Services.Infrastructure.LocalStorage.Contract;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using reewire.core.services;
using reewire.core.services.contracts.services;
using reewire.core.services.models;
using reewire.core.services.services;
using System;
using System.Windows.Input;

namespace Kola.Starting.Dialogs
{
    public class ForcePincodeChangePopupViewModel : ViewModelBase
    {
        private readonly IAuthentication _authentication;
        private readonly IFeature _updatepinservice;
        private readonly IUserInformation _userInformation;
        private readonly IUserInformationLocalStorage _userInformationLocalStorage;

        private Field<string> _user { get; set; } = new Field<string>();
        public Field<string> OldPinCode { get; set; } = new Field<string>();
        public Field<string> NewPinCode { get; set; } = new Field<string>();
        public Field<string> ConfirmNewPinCode { get; set; } = new Field<string>();
        public Field<int> MaxLenthPinCode { get; set; } = new Field<int>(Constants.MaxLenthPinCode);

        public ICommand GotoValidateCommand => new DelegateCommand(ValidateChangePinCodeAsync);

        public ForcePincodeChangePopupViewModel(INavigationService navigationService,
                                      IAuthentication authentication,
                                      IFeature updateservice,
                                      IUserInformation userInformation,
                                      IUserInformationLocalStorage userInformationLocalStorage
                                     ) : base(navigationService)
        {
            _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
            _userInformation = userInformation ?? throw new System.ArgumentNullException(nameof(userInformation));
            _updatepinservice = updateservice ?? throw new ArgumentNullException(nameof(updateservice));
            _userInformationLocalStorage = userInformationLocalStorage ?? throw new ArgumentNullException(nameof(userInformationLocalStorage));
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        private bool CheckValidateData(string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        private bool CheckValidateLength(string value)
        {
            return value?.Length == MaxLenthPinCode.Value;
        }

        private async void ValidateChangePinCodeAsync()
        {
            OldPinCode.HasError = false;
            OldPinCode.ErrorText = "";

            NewPinCode.HasError = false;
            NewPinCode.ErrorText = "";

            ConfirmNewPinCode.HasError = false;
            ConfirmNewPinCode.ErrorText = "";

            if (!CheckValidateData(OldPinCode.Value))
            {
                OldPinCode.HasError = true;
                OldPinCode.ErrorText = TranslateManagerHelper.Convert("cannot_empty");
                return;
            }

            if (!CheckValidateLength(OldPinCode.Value))
            {
                OldPinCode.HasError = true;
                OldPinCode.ErrorText = TranslateManagerHelper.Convert("pincode_is_short");
                return;
            }

            if (!CheckValidateData(NewPinCode.Value))
            {
                NewPinCode.HasError = true;
                NewPinCode.ErrorText = TranslateManagerHelper.Convert("cannot_empty");
                return;
            }

            if (!CheckValidateLength(NewPinCode.Value))
            {
                NewPinCode.HasError = true;
                NewPinCode.ErrorText = TranslateManagerHelper.Convert("pincode_is_short");
                return;
            }

            if (NewPinCode.Value == OldPinCode.Value)
            {
                NewPinCode.HasError = true;
                NewPinCode.ErrorText = TranslateManagerHelper.Convert("old_pincode_newpincode_cannot_equal");
                return;
            }

            if (!CheckValidateData(ConfirmNewPinCode.Value))
            {
                ConfirmNewPinCode.HasError = true;
                ConfirmNewPinCode.ErrorText = TranslateManagerHelper.Convert("cannot_empty");
                return;
            }

            if (!CheckValidateLength(ConfirmNewPinCode.Value))
            {
                ConfirmNewPinCode.HasError = true;
                ConfirmNewPinCode.ErrorText = TranslateManagerHelper.Convert("pincode_is_short");
                return;
            }

            // compare newPIN to ConfirmNewPIN
            if (NewPinCode.Value != ConfirmNewPinCode.Value)
            {
                ConfirmNewPinCode.HasError = true;
                ConfirmNewPinCode.ErrorText = TranslateManagerHelper.Convert("new_pincode_no_match");
                return;
            }

            if (IsBusy || IsLoading)
            {
                // Message Toast
                return;
            }
            IsLoading = true;

            // appel service pour confirmation pin code

            UpdatePincodeRequest upr = new UpdatePincodeRequest();
            upr.NewPassword = NewPinCode.Value;

            featureDTO fb = new featureDTO
            {
                FeatureName = "UPDATE_PINCODE",
                SenderId = ApiConstants.CountryPrefix + _userInformation.UserName,
                SenderPassword = OldPinCode.Value,
                TargetId = "",
                Amount = 0,
                jsonObjString = JsonConvert.SerializeObject(upr)
            };

            // NOTE : the Initialize method is not async , so we wait for the result.
            //ObservableCollection<AccountModel> Items = new ObservableCollection<AccountModel>();
            NavigationParameters parameters = null;
            string message = "";
            bool isSuccess = false;

            var response = await _updatepinservice.Execute<featureDTO, string>(_authentication.GetToken(), fb, ApiConstants.SystemId);
            if (response != null)
            {
                if (response.isuccess)
                {
                    message = TranslateManagerHelper.Convert("pincode_changed_successfully");
                    await NavigationService.ClearPopupStackAsync("", "", true);
                    await NavigationService.NavigateAsync($"/{FunctionName.PinCode}");
                    isSuccess = true;
                    IsLoading = false;
                    return;
                }
                else
                {
                    switch (response.errcode)
                    {
                        case "EXCEPTION":
                            message = TranslateManagerHelper.Convert("error_contact_support");
                            break;
                        default:
                            message = !string.IsNullOrEmpty(response.msg) ? response.msg : TranslateManagerHelper.Convert("failed_pincode_change");
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
                        { Constants.PopupMessage, message },
                        {Constants.PopupNextPage, FunctionName.Login }
                    };


            await NavigationService.NavigateAsync(PopupName.SuccessfullPopup, parameters);


            IsLoading = false;
        }
    }
}
