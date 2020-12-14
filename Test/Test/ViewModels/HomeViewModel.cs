using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Test.ViewModels.Base;

namespace Test.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}
