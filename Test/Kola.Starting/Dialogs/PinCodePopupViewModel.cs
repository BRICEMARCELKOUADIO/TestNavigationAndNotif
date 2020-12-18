using Kola.Infrastructure;
using Kola.Infrastructure.Helper;
using Kola.Infrastructure.Navigation;
using Kola.Infrastructure.UserAuthentication.Contracts;
using Kola.Services.Infrastructure.LocalStorage.Contract;
using Plugin.Fingerprint;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Windows.Input;

namespace Kola.Starting.Dialogs
{
    public class PinCodePopupViewModel : ViewModelBase
    {
        private string _id;
        private readonly IUserInformation _userInformation;
        private readonly IUserInformationLocalStorage _userInformationLocalStorage;

        public Field<int> PinCodeMaxLength { get; set; } = new Field<int>(Constants.MaxLenthPinCode);
        public Field<int> PinCodeMinLength { get; set; } = new Field<int>(Constants.MinLenthPinCode);
        public Field<string> PinCode { get; set; } = new Field<string>();
        public Field<bool> FingerPrintAvailable { get; set; } = new Field<bool>(false);

        public Field<string> CancelText { get; set; } = new Field<string>("cancel");
        public Field<string> SubmitText { get; set; } = new Field<string>("submit");


        public new ICommand GotoCancelCommand => new DelegateCommand(GotoCancel);
        public new ICommand GotoValidateCommand => new DelegateCommand(GotoValidate);
        public ICommand GotoFingerPrintCommand => new DelegateCommand(GotoFingerPrint);
        public PinCodePopupViewModel(INavigationService navigationService, IUserInformation userInformation, IUserInformationLocalStorage userInformationLocalStorage) : base(navigationService)
        {
            _userInformation = userInformation ?? throw new System.ArgumentNullException(nameof(userInformation));
            _userInformationLocalStorage = userInformationLocalStorage ?? throw new ArgumentNullException(nameof(userInformationLocalStorage));
            Initialize();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            _id = parameters.GetValue<string>(Constants.PopupId);

        }

        private async void GotoCancel()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            await NavigationService.GoBackAsync().ConfigureAwait(false);
            IsLoading = false;
        }

        protected void Initialize()
        {
            IsLoading = true;
            GetFingerPrintAvailability();
            IsLoading = false;
        }

        private void GotoValidate()
        {
            PinCode.HasError = false;
            PinCode.ErrorText = "";

            if (string.IsNullOrEmpty(PinCode.Value))
            {
                PinCode.HasError = true;
                PinCode.ErrorText = TranslateManagerHelper.Convert("pincode_cannot_empty");
                return;
            }

            if (PinCode.Value.Length <= PinCodeMinLength.Value)
            {
                PinCode.HasError = true;
                PinCode.ErrorText = TranslateManagerHelper.Convert("pincode_is_short");
                return;
            }

            if (PinCode.Value != _userInformation.Password) 
            {
                PinCode.HasError = true;
                PinCode.ErrorText = TranslateManagerHelper.Convert("pincode_is_incorrect");
                return;
            }

            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            var parameters = new NavigationParameters 
            {
                { Constants.PopupId, _id},
                {Constants.PopupResponseData, true }
            };

            NavigationService.GoBackAsync(parameters);
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
                PinCode.Value = _userInformationLocalStorage.FPassword;
                IsLoading = false;
                GotoValidate();
            }
            else
            {
                IsLoading = false;
            }
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
    }
}
