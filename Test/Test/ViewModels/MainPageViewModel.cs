using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Test.ViewModels.Base;

namespace Test.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        INavigationService _navigationService;
       public DelegateCommand<string> NavigateCommand { get; set; }
        public MainPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
           NavigateCommand = new DelegateCommand<string>(OnNavigateCommand);
        }

        private async void OnNavigateCommand(string page)
        {

            await _navigationService.NavigateAsync($"/MasterDetailPageView/NavigationPage/{page}").ConfigureAwait(false);
        }
    }
}
