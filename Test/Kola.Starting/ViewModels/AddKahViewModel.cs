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
    public class AddKahViewModel : ViewModelBase
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

        public AddKahViewModel(INavigationService navigationService, IEventAggregator ea) : base(navigationService)
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
                    Title= TranslateManagerHelper.Convert("with_card").ToUpperInvariant(),
                    SecondDescription = TranslateManagerHelper.Convert("use_your_card"),
                    Icon = "icon-kash_in_with_card_method_sub",
                    Url ="KashInWithCard"
                },
                new SubMenuModel()
                {
                    Title= TranslateManagerHelper.Convert("with_bank").ToUpperInvariant(),
                    SecondDescription =  TranslateManagerHelper.Convert("use_your_bank_account"),
                    Icon = "icon-kash_in_with_bank_method_sub",
                    Url ="KashInWithCard"
                }
            };
        }
    }
}
