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
using System.Text;
using System.Windows.Input;

namespace Kola.Starting.ViewModels
{
    public class KolaPayViewModel : ViewModelBase
    {
        private IEventAggregator _ea;

        public ObservableCollection<SlideItem> Items { get; set; } = new ObservableCollection<SlideItem>();

        public ICommand DisplayBurger => new DelegateCommand(OnDisplayBurger);
        public ICommand GotoPageCommand => new DelegateCommand<string>(GotoPage);
        public KolaPayViewModel(INavigationService navigationService, IEventAggregator ea) : base(navigationService)
        {
            _ea = ea;
            Initialize();
        }

        protected void Initialize()
        {
            IsLoading = true;
            Items = new ObservableCollection<SlideItem>()
            {
                new SlideItem() { Icon ="icon-pay_bills_internet",  PrimayText =TranslateManagerHelper.Convert("airtime").ToUpperInvariant(), Url= FunctionName.Airtime },
                new SlideItem() { Icon ="icon-remittance_inter_payout_sub",  PrimayText =TranslateManagerHelper.Convert("utilities").ToUpperInvariant(), Url=FunctionName.Utilities },
                new SlideItem() { Icon ="icon-pay_bills_tv",  PrimayText =TranslateManagerHelper.Convert("tv").ToUpperInvariant(), Url="" },
                new SlideItem() { Icon ="icon-kash_in_with_bank_method_sub",  PrimayText =TranslateManagerHelper.Convert("gol").ToUpperInvariant(), Url=FunctionName.Gol },
                new SlideItem() { Icon ="icon-other",  PrimayText =TranslateManagerHelper.Convert("others").ToUpperInvariant(), Url=FunctionName.Others }
            };
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

        private void OnDisplayBurger()
        {
            _ea.GetEvent<BurgerEvent>().Publish();
        }
    }
}
