using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Test.ViewModels.Base
{
    public class BaseViewModel : BindableBase, INavigationAware, IDestructible
    {
        INavigationService _navigationService;
        public DelegateCommand<string> GoToView { get; set; }
        public DelegateCommand GoBack { get; set; }
        public BaseViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            GoToView = new DelegateCommand<string>(OnGoToView);
            GoBack = new DelegateCommand(OnGoBack);
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

        public void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}
