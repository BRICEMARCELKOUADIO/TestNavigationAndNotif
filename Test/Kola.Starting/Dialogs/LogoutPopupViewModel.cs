using Kola.Infrastructure.Navigation;
using Kola.Infrastructure.UserAuthentication.Contracts;
using Kola.Services.Infrastructure.LocalStorage.Contract;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Windows.Input;

namespace Kola.Starting.Dialogs
{
    public class LogoutPopupViewModel : ViewModelBase
    {
        private readonly IUserInformationLocalStorage _userInformationLocalStorage;
        private readonly IAuthentication _authentication;
        public ICommand GotoValidateCommand => new DelegateCommand(GotoValidate);
        public LogoutPopupViewModel(INavigationService navigationService, IUserInformationLocalStorage userInformationLocalStorage, IAuthentication authentication) : base(navigationService)
        {
            _userInformationLocalStorage = userInformationLocalStorage ?? throw new ArgumentNullException(nameof(userInformationLocalStorage));
            _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
        }

        private async void GotoValidate()
        {
            await NavigationService.ClearPopupStackAsync("", "", true);
            _authentication.Logout();
            await NavigationService.NavigateAsync($"/{FunctionName.Login}");
        }
    }
}
