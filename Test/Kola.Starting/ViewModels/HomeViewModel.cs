using Kola.Infrastructure;
using Kola.Infrastructure.EventAggregator;
using Kola.Infrastructure.Helper;
using Kola.Infrastructure.Models;
using Kola.Infrastructure.Navigation;
using Kola.Infrastructure.UserAuthentication.Contracts;
using Kola.Starting.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using reewire.core.services;
using reewire.core.services.contracts.services;
using reewire.core.services.models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Kola.Starting.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly IAuthentication _authentication;
        private readonly IFeature _balanceservice;
        private readonly IUserInformation _userinformation;

        #region Properties        
        protected MenuItemModel _selectedItem;
        private IEventAggregator _ea;
        public MenuItemModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnSelectedItemChanged(_selectedItem);
            }
        }

        private ObservableCollection<SlideItem> _items = new ObservableCollection<SlideItem>();
        public ObservableCollection<SlideItem> Items
        {
            get { return _items; }
            set
            {
                SetProperty(ref _items, value);
            }
        }

        protected ObservableCollection<MenuItemModel> _navigationMenuItems = new ObservableCollection<MenuItemModel>();
        public ObservableCollection<MenuItemModel> NavigationMenuItems
        {
            get => _navigationMenuItems;
            set => SetProperty(ref _navigationMenuItems, value);
        }

        public Field<string> Version { get; set; } = new Field<string>();
        public ICommand GotoPageCommand => new DelegateCommand<string>(GotoPage);
        public ICommand DisplayBurger => new DelegateCommand(OnDisplayBurger);
        #endregion

        public HomeViewModel(INavigationService navigationService, IEventAggregator ea,
            IAuthentication authentication, IFeature balanceservice, IUserInformation userinformation)
            : base(navigationService)
        {
            _authentication = authentication;
            _balanceservice = balanceservice;
            _userinformation = userinformation;

            _ea = ea;
            Initialize();
        }

        protected async void Initialize()
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                ShowSnackBarWithAction(TranslateManagerHelper.Convert("loading_please_wait"), null, TranslateManagerHelper.Convert("ok").ToUpperInvariant());
                return;
            }

            IsLoading = true;

            Items = new ObservableCollection<SlideItem>()
            {
                new SlideItem() { Icon ="icon-kash_in",   PrimayText =TranslateManagerHelper.Convert("kash"), Url= FunctionName.Kash },
                new SlideItem() { Icon ="icon-gift_card", PrimayText =TranslateManagerHelper.Convert("gift_card"), Url= FunctionName.GiftCard },
                new SlideItem() { Icon ="icon-voucher", PrimayText =TranslateManagerHelper.Convert("vourchers"), Url= FunctionName.Vouchers },
                new SlideItem() { Icon ="icon-kola_pay",  PrimayText =TranslateManagerHelper.Convert("kola_pay"), Url= FunctionName.KolaPay },
                new SlideItem() { Icon ="icon-charity", PrimayText =TranslateManagerHelper.Convert("charity"), Url= FunctionName.Charity },
            };
               
            IsLoading = false;
        }

        protected async void GotoPage(string p)
        {
            if (IsBusy || IsLoading)
            {
                // Message Tosat
                return;
            }
            IsLoading = true;
            await NavigationService.NavigateAsync($"{p}");
            IsLoading = false;
        }

        protected void OnSelectedItemChanged(MenuItemModel pitem)
        {
            if (pitem != null && !string.IsNullOrEmpty(pitem.Url) && pitem.IsActive)
            {
                GotoPage(pitem.Url);
                SelectedItem = null;
            }
        }

        private void OnDisplayBurger()
        {
            _ea.GetEvent<BurgerEvent>().Publish();
        }

        public async void OpenLogoutPopup()
        {
            await NavigationService.NavigateAsync(FunctionName.Logout);
        }
    }
}
