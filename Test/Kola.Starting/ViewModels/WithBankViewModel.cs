using Kola.Infrastructure;
using Kola.Infrastructure.EventAggregator;
using Kola.Infrastructure.Helper;
using Kola.Infrastructure.Models;
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

namespace Kola.Starting.ViewModels
{
    public class WithBankViewModel : ViewModelBase
    {
        private AccountModel _accountSelected = new AccountModel();
        public Field<string> Amount { get; set; } = new Field<string>();
        public Field<string> Bank { get; set; } = new Field<string>();
        public Field<string> AccountType { get; set; } = new Field<string>();
        public Field<string> FirstName { get; set; } = new Field<string>();
        public Field<string> LastName { get; set; } = new Field<string>();
        public Field<string> AccountNumber { get; set; } = new Field<string>();
        public Field<int> AccountNumberMaxLength { get; set; } = new Field<int>(Constants.MaxLengthAccountNumber);
        public Field<int> AmountMaxLength { get; set; } = new Field<int>(Constants.MaxLengthAmount);
        public Field<bool> HasAccountSelected { get; set; } = new Field<bool>(false);
        public Field<string> CurrentCurrency { get; set; } = new Field<string>();
        public Field<string> CurrentBalance { get; set; } = new Field<string>();

        public Field<int> TotalStep { get; set; } = new Field<int>(2);
        public Field<int> CurrentStep { get; set; } = new Field<int>(1);
        public Field<bool> FirstStep { get; set; } = new Field<bool>(true);
        public Field<bool> SecondStep { get; set; } = new Field<bool>(false);

        public Field<string> PreviousText { get; set; } = new Field<string>();
        public Field<string> NextText { get; set; } = new Field<string>();

        private IEventAggregator _ea;
        public ICommand DisplayBurger => new DelegateCommand(OnDisplayBurger);
        public ICommand GotoOpenAccountPopupCommand => new DelegateCommand(GotoOpenAccountPopup);
        public ICommand OnOpenBankPopupCommand => new DelegateCommand(GotoOpenBankPopup);
        public ICommand OnOpenAccountTypePopupCommand => new DelegateCommand(GotoOpenAccountTypePPopup);
        public ICommand GotoPreviousCommand => new DelegateCommand(GotoPrevious);
        public ICommand GotoNextCommand => new DelegateCommand(GotoNext);
        public WithBankViewModel(INavigationService navigationService, IEventAggregator ea, bool hasReleveView = true) : base(navigationService, hasReleveView)
        {
            _ea = ea;
            Initialize();
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
                case "BANK_POPUP":
                    var selectedBank = parameters.GetValue<PikerItem>(Constants.PopupResponseData);
                    Bank.Id = selectedBank.Id;
                    Bank.Value = selectedBank.Name_1;
                    break;
                case "ACCOUNT_TYPE_POPUP":
                    var selectedAccountType = parameters.GetValue<PikerItem>(Constants.PopupResponseData);
                    AccountType.Id = selectedAccountType.Id;
                    AccountType.Value = selectedAccountType.Name_1;
                    break;
                case "PINCODE_POPUP":
                    {
                        var ok = parameters.GetValue<bool>(Constants.PopupResponseData);
                        // call service do with card
                        DoService(ok);
                    }
                    break;
                default:
                    break;
            }
        }

        private void Initialize()
        {
            _accountSelected = new AccountModel();
            PreviousText.Value = TranslateManagerHelper.Convert("cancel").ToUpperInvariant();
            NextText.Value = TranslateManagerHelper.Convert("next").ToUpperInvariant();
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

        private async void GotoOpenBankPopup()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            // appel service pour recuperer la liste des genders, attention, le service devra gerer le cache de sorte
            // a ne pas rappeller a cahque fois le service referential

            ObservableCollection<PikerItem> exampleGendersList = new ObservableCollection<PikerItem>();

            //var listg = await _genderservice.GetList("");
            //if (listg?.isuccess == true)
            //{
            //    exampleGendersList = listg.data.Select(x => new PikerItem { Id = x.id.ToString(), Name_1 = x.name }).ToObservableCollection();
            //}

            var parameters = new NavigationParameters
                {
                    { Constants.PopupId, "BANK_POPUP" },
                    { Constants.PopupTitle, TranslateManagerHelper.Convert("select_bank").ToUpperInvariant() },
                    //{Constants.PopupCurrentData, exampleGendersList.FirstOrDefault(x => x.Id == Gender.Id)},
                    { Constants.PopupResquestData, exampleGendersList }
                };

            await NavigationService.NavigateAsync(PopupName.PikerPopup, parameters).ConfigureAwait(false);

            IsLoading = false;

        }

        private async void GotoOpenAccountTypePPopup()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }
            IsLoading = true;

            // appel service pour recuperer la liste des genders, attention, le service devra gerer le cache de sorte
            // a ne pas rappeller a cahque fois le service referential

            ObservableCollection<PikerItem> exampleGendersList = new ObservableCollection<PikerItem>();

            //var listg = await _genderservice.GetList("");
            //if (listg?.isuccess == true)
            //{
            //    exampleGendersList = listg.data.Select(x => new PikerItem { Id = x.id.ToString(), Name_1 = x.name }).ToObservableCollection();
            //}

            var parameters = new NavigationParameters
                {
                    { Constants.PopupId, "ACCOUNT_TYPE_POPUP" },
                    { Constants.PopupTitle, TranslateManagerHelper.Convert("account_type").ToUpperInvariant() },
                    //{Constants.PopupCurrentData, exampleGendersList.FirstOrDefault(x => x.Id == Gender.Id)},
                    { Constants.PopupResquestData, exampleGendersList }
                };

            await NavigationService.NavigateAsync(PopupName.PikerPopup, parameters).ConfigureAwait(false);

            IsLoading = false;

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
                        FirstStep.Value = false;
                        SecondStep.Value = true;
                        PreviousText.Value = TranslateManagerHelper.Convert("previous").ToUpperInvariant();
                        NextText.Value = TranslateManagerHelper.Convert("validate").ToUpperInvariant();
                        CurrentStep.Value++;
                        break;
                    }
                case 2:
                    {
                        var parameters = new NavigationParameters
                        {
                            { Constants.PopupId, "PINCODE_POPUP" },
                        };
                        await NavigationService.NavigateAsync(PopupName.PinCodePopup, parameters).ConfigureAwait(false);
                        IsLoading = false;
                        break;
                    }
                default:
                    break;
            }
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

            // call service 

           

            //var result = await _service.Execute<featureDTO, >(_authentication.GetToken(), f, ApiConstants.SystemId);
            //NavigationParameters parameters = null;
            //string message = "";
            //bool isSuccess = false;
            //if (result != null)
            //{
            //    if (result.isuccess)
            //    {
                    
            //        //UpdateUserAccount(new AccountModel
            //        //{
            //        //    AccountNumber = ,
            //        //    Balance = ,
            //        //    Currency = ,
            //        //    Type = 
            //        //});
            //    }
            //    else
            //    {
            //        switch (result.errcode)
            //        {
            //            case "EXCEPTION":
            //                message = TranslateManagerHelper.Convert("operation_failed");
            //                break;
            //            default:
            //                message = !string.IsNullOrEmpty(result.msg) ? result.msg : TranslateManagerHelper.Convert("cashin_failed");
            //                break;
            //        }

            //        isSuccess = false;

            //    }

            //}
            //else
            //{
            //    // TODO : what message do we send ?
            //    isSuccess = false;

            //    switch (result.errcode)
            //    {
            //        case "EXCEPTION":
            //            message = TranslateManagerHelper.Convert("operation_failed");
            //            break;
            //        default:
            //            message = TranslateManagerHelper.Convert("error_contact_support");
            //            break;
            //    }

            //}
            //parameters = new NavigationParameters
            //        {
            //            { Constants.PopupIsSucces, isSuccess },
            //            { Constants.PopupIsBeforeHome, false },
            //            { Constants.PopupMessage, message },
            //            {Constants.PopupNextPage, FunctionName.Home }
            //        };


            //await NavigationService.NavigateAsync(PopupName.SuccessfullPopup, parameters).ConfigureAwait(false);
            //IsLoading = false;
        }

        private void OnDisplayBurger()
        {
            _ea.GetEvent<BurgerEvent>().Publish();
        }
    }
}
