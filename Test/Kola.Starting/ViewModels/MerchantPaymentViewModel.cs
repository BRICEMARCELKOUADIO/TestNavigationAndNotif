using Kola.Infrastructure.EventAggregator;
using Kola.Infrastructure.Helper;
using Kola.Infrastructure.Models;
using Kola.Infrastructure.Navigation;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Kola.Starting.ViewModels
{
    public class MerchantPaymentViewModel : ViewModelBase
    {
        private IEventAggregator _ea;
        public ICommand DisplayBurger => new DelegateCommand(OnDisplayBurger);
        public ICommand GotoPageCommand => new DelegateCommand<string>(GotoPage);

        private ObservableCollection<SubMenuModel> _subMenus = new ObservableCollection<SubMenuModel>();
        public ObservableCollection<SubMenuModel> SubMenus
        {
            get { return _subMenus; }
            set
            {
                SetProperty(ref _subMenus, value);
            }
        }

        public MerchantPaymentViewModel(INavigationService navigationService, IEventAggregator ea) : base(navigationService)
        {
            _ea = ea;
            Initialize();
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

        private void OnDisplayBurger()
        {
            _ea.GetEvent<BurgerEvent>().Publish();
        }

        private void Initialize()
        {
            SubMenus = new ObservableCollection<SubMenuModel>()
            {
                new SubMenuModel()
                {
                    Title=TranslateManagerHelper.Convert("merchant_payment").ToUpperInvariant(),
                    SecondDescription = TranslateManagerHelper.Convert("scan_merchant_qr_code"),
                    Icon = "icon-qr_code",
                    Url ="MerchantPaymentQRCode"
                },
                new SubMenuModel()
                {
                    Title=TranslateManagerHelper.Convert("manual_payment").ToUpperInvariant(),
                    SecondDescription = TranslateManagerHelper.Convert("scan_merchant_qr_code"),
                    Icon = "icon-qr_code",
                    Url ="MerchantPaymentManual"
                }
            };
        }
    }
}
