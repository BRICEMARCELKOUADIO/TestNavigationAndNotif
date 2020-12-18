using Kola.Infrastructure;
using Kola.Infrastructure.Navigation;
using Prism.Commands;
using Prism.Navigation;
using System.Windows.Input;
using Xamarin.Forms;

namespace Kola.Starting.Dialogs
{
    public class SuccessfullPopupViewModel : ViewModelBase
    {
        private string _rootPage = string.Empty;
        private string _id;
        private bool _isSuccess;
        private bool _isBeforeHome;
       
        public Field<string> Message { get; set; } = new Field<string>();
        public Field<string> Mtcn { get; set; } = new Field<string>("","", false);
        public Field<Color> MessageTextColor { get; set; } = new Field<Color>();

        public ICommand GotoPageCommand => new DelegateCommand(GotoPage);
        private INavigationService _navigationService;
        public SuccessfullPopupViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            _id = parameters.GetValue<string>(Constants.PopupId);
            _rootPage = parameters.GetValue<string>(Constants.PopupNextPage);
            Message.Value = parameters.GetValue<string>(Constants.PopupMessage);
            _isSuccess = parameters.GetValue<bool>(Constants.PopupIsSucces);
            _isBeforeHome = parameters.GetValue<bool>(Constants.PopupIsBeforeHome);

            MessageTextColor.Value = _isSuccess ? (Color)Application.Current.Resources["color-success"] : (Color)Application.Current.Resources["color-error"];
            Mtcn.Value = parameters.GetValue<string>(Constants.PopupMessageMTCN);
            Mtcn.IsVisible = !string.IsNullOrEmpty(Mtcn.Value);

        }

        protected async void GotoPage()
        {
            if(_isSuccess)
            {
                await NavigationService.ClearPopupStackAsync("", "", true);
                if (_isBeforeHome)
                    await _navigationService.NavigateAsync($"/{_rootPage}").ConfigureAwait(false);
                else
                    await _navigationService.NavigateAsync($"/CustomMasterDetailPageView/NavigationPage/{FunctionName.Home}/{_rootPage}").ConfigureAwait(false);
            }
            else
            {
                await NavigationService.GoBackAsync();
            }
            
        }
    }
}
