using Plugin.FirebasePushNotification;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Test.Event;
using Test.ViewModels.Base;

namespace Test.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private IEventAggregator _ea;
        public DelegateCommand DisplayBurger => new DelegateCommand(OnDisplayBurger);
        public HomeViewModel(INavigationService navigationService, IEventAggregator ea) : base(navigationService)
        {
            _ea = ea;
        }
        private void OnDisplayBurger()
        {
            _ea.GetEvent<BurgerEvent>().Publish();
        }
    }
}
