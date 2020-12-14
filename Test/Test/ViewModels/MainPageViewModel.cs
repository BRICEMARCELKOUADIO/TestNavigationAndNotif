using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Test.ViewModels.Base;

namespace Test.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        public MainPageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}
