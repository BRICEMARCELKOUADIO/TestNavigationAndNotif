
using Kola.Infrastructure.Navigation;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Kola.Starting.Dialogs
{
    public class RejectPopupViewModel : ViewModelBase
    {
        
        public ICommand GotoValidateCommand => new DelegateCommand(GotoValidate);
        public RejectPopupViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        private async void GotoValidate()
        {
            //await NavigationService.NavigateAsync($"/{FunctionName.Login}");
        }
    }
}
