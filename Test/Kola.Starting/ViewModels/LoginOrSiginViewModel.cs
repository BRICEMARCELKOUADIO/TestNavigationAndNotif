
using Kola.Infrastructure.Helper;
using Kola.Infrastructure.Navigation;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Kola.Starting.ViewModels
{
    public class LoginOrSiginViewModel : ViewModelBase
    {
        public Field<string> LoginText { get; set; } = new Field<string>("login");
        public Field<string> SignUpText { get; set; } = new Field<string>("sign_up");
        public ICommand GotoPageCommand => new Command<string>(OnGotoPageCommand);

        public LoginOrSiginViewModel(INavigationService navigationService) : base(navigationService, false)
        {
            //ShowNotificationImmediatly("Titre", "You're received new transfer 50,00 USD from Mr WILSON ANDERSON at 2020-10-13 12:00. !");
            //ShowNotificationScheduler("Titre", "Hello it's me!", 100, 10);
        }

        private async void OnGotoPageCommand(string p)
        {
            if (IsBusy || IsLoading)
            {
                // Message toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }

            IsLoading = true;
            await NavigationService.NavigateAsync($"{p}").ConfigureAwait(false);
            IsLoading = false;
        }
    }
}
