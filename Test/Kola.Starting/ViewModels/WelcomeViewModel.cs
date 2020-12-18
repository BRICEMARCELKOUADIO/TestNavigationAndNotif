
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
    public class WelcomeViewModel : ViewModelBase
    {
        public ICommand GotoPageCommand => new Command<string>(OnGotoPageCommand);
        public WelcomeViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        private async void OnGotoPageCommand(string p)
        {
            if (IsBusy || IsLoading)
            {
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }

            IsBusy = true;
            await NavigationService.NavigateAsync($"{p}");
            IsBusy = false;
        }
    }
}
