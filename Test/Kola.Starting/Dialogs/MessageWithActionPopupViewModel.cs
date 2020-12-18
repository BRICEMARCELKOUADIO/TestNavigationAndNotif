using Kola.Infrastructure;
using Kola.Infrastructure.Navigation;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Windows.Input;

namespace Kola.Starting.Dialogs
{
    public class MessageWithActionPopupViewModel : ViewModelBase
    {
        private string _id;
        private INavigationService _navigationService;
        public Field<string> Message1 { get; set; } = new Field<string>();
        public Field<string> Message2 { get; set; } = new Field<string>();

        public ICommand GotoValidateCommand => new DelegateCommand<string>(GotoValidate);
        public MessageWithActionPopupViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            _id = parameters.GetValue<string>(Constants.PopupId);
            Message1.Value = parameters.GetValue<string>(Constants.PopupMessage);
            Message2.Value = parameters.GetValue<string>(Constants.PopupMessage2);
        }

        private async void GotoValidate(string value)
        {
            bool ok = false;
            bool.TryParse(value, out ok);

            var parameters = new NavigationParameters
                {
                    { Constants.PopupId, _id },
                    {Constants.PopupResponseData, ok},
                };
            await NavigationService.GoBackAsync(parameters);
        }
    }
}
