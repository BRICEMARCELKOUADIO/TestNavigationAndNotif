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
    public class UtilitiesViewModel : ViewModelBase
    {
        private IEventAggregator _ea;

        public ObservableCollection<SlideItem> Items { get; set; } = new ObservableCollection<SlideItem>();

        public ICommand DisplayBurger => new DelegateCommand(OnDisplayBurger);
        public ICommand GotoPageCommand => new DelegateCommand<string>(GotoPage);
        public UtilitiesViewModel(INavigationService navigationService, IEventAggregator ea) : base(navigationService)
        {
            _ea = ea;
            Initialize();
        }

        protected void Initialize()
        {
            IsLoading = true;
            Items = new ObservableCollection<SlideItem>()
            {
                new SlideItem() { Icon ="icon-pay_bills_electricity",  PrimayText =TranslateManagerHelper.Convert("electricity").ToUpperInvariant(), Url="" },
                new SlideItem() { Icon ="icon-pay_bills_water",  PrimayText =TranslateManagerHelper.Convert("water").ToUpperInvariant(), Url="" },               
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
