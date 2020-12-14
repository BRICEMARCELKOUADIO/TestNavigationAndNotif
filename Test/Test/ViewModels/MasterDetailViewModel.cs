using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Test.ViewModels.Base;

namespace Test.ViewModels
{
    public class MasterDetailViewModel : BaseViewModel
    {
        public DelegateCommand<string> NavigateCommand { get; set; }
        INavigationService _navigationService;
        public MasterDetailViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            NavigateCommand = new DelegateCommand<string>(OnNavigateCommand);
        }

        private async void OnNavigateCommand(string page)
        {
            await _navigationService.NavigateAsync($"/MasterDetailView/NavigationPage/Home/{page}").ConfigureAwait(false);
        }
    }
}
