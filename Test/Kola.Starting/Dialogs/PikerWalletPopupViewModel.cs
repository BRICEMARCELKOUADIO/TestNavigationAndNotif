using Kola.Infrastructure;
using Kola.Infrastructure.Models;
using Kola.Infrastructure.Navigation;
using Kola.Starting.Models;
using Prism.Commands;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Kola.Starting.Dialogs
{
    public class PikerWalletPopupViewModel : ViewModelBase
    {
        private string _id;
        private PikerItem _currentItem;

        public Field<string> CancelText { get; set; } = new Field<string>("cancel");

        private ObservableCollection<PikerItem> _items;
        private AccountModel _selectedAccount;

        public ObservableCollection<PikerItem> Items
        {
            get { return _items; }
            set
            {
                SetProperty(ref _items, value);
            }
        }
        public Field<string> Search { get; set; } = new Field<string>("","", false);

        public new ICommand OnSelectedCommand => new DelegateCommand<string>(OnSelected);
        public new ICommand GotoCancelCommand => new DelegateCommand(GotoCancel);

        public PikerWalletPopupViewModel(INavigationService navigationService) : base(navigationService)
        {
            _items = new ObservableCollection<PikerItem>();
            _currentItem = new PikerItem();
            Initialize();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            IsLoading = true;
            _id = parameters.GetValue<string>(Constants.PopupId);
            Title = parameters.GetValue<string>(Constants.PopupTitle);
            //Items = parameters.GetValue<ObservableCollection<PikerItem>>(Constants.PopupResquestData);
            Search.IsVisible = Items.Count > 4;
            _selectedAccount = parameters.GetValue<AccountModel>(Constants.PopupCurrentData);
            if (_selectedAccount?.AccountNumber != null)
            {
                var item = Items.FirstOrDefault(x => x.Id == _selectedAccount.AccountNumber);
                if (item != null)
                {
                    item.IsSelected.Value = true;
                }
            }
            IsLoading = false;
        }

        private async void OnSelected(string id)
        {
            IsLoading = true;

            _currentItem = _items.FirstOrDefault(x => x.Id == id);
            if(_currentItem != null)
            {
                var parameters = new NavigationParameters
                {
                    { Constants.PopupId, _id },
                    {Constants.PopupResponseData, _currentItem},
                };
                await NavigationService.GoBackAsync(parameters);
            }
           
            IsLoading = false;
        }

        private async void GotoCancel()
        {
            await NavigationService.GoBackAsync();
        }

        private void Initialize()
        {
            Items = new ObservableCollection<PikerItem>() {  new PikerItem
                {
                    Id = _accountUSD.AccountNumber,
                    Name_1 = _accountUSD.AccountNumber,
                    Name_2 = _accountUSD.Type,
                    Name_3 = _accountUSD.Balance,
                    Name_4 = _accountUSD.Currency
                },

                new PikerItem
                {
                    Id = _accountLRD.AccountNumber,
                    Name_1 = _accountLRD.AccountNumber,
                    Name_2 = _accountLRD.Type,
                    Name_3 = _accountLRD.Balance,
                    Name_4 = _accountLRD.Currency
                }
            };
        }
    }
}
