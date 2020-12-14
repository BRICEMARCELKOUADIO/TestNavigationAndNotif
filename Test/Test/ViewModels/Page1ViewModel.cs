using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Test.ViewModels.Base;

namespace Test.ViewModels
{
    public class Page1ViewModel : BaseViewModel
    {
        public Page1ViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}
