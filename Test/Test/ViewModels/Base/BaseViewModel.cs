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
        public readonly ICommand GoToView;
        public readonly ICommand GoBack;
        public BaseViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            GoToView = new DelegateCommand<string>(OnGoToView);
            GoBack = new DelegateCommand(OnGoBack);
        }


        public void OnGoToView(string page)
        {
            _navigationService.NavigateAsync($"{page}");
        }

        public void OnGoBack()
        {
            _navigationService.GoBackAsync();
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
