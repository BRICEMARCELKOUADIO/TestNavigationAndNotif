using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Test.ViewModels.Base
{
    public class BaseViewModel : BindableBase, INavigationAware
    {
        INavigationService _navigationService;
        public DelegateCommand<string> GoToView =>  new DelegateCommand<string>(OnGoToView);
        public DelegateCommand GoBack => new DelegateCommand(OnGoBack);
        public BaseViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }


        private async void OnGoToView(string page)
        {
            await _navigationService.NavigateAsync($"{page}");
        }

        private async void OnGoBack()
        {
            await _navigationService.GoBackAsync();
        }
        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
        }
    }
}
