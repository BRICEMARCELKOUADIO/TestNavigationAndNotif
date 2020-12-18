using Kola.Infrastructure.Helper;
using Kola.Infrastructure.Navigation;
using Kola.Starting.Models;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Test.ViewModels.Base;

namespace Test.ViewModels
{
    public class CustomMasterDetailPageViewModel : ViewModelBase
    {
        protected ObservableCollection<MenuItemModel> _navigationMenuItems = new ObservableCollection<MenuItemModel>();
        public ObservableCollection<MenuItemModel> NavigationMenuItems
        {
            get => _navigationMenuItems;
            set => SetProperty(ref _navigationMenuItems, value);
        }

        public DelegateCommand<string> NavigateCommand { get; set; }
        INavigationService _navigationService;
        public CustomMasterDetailPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            NavigateCommand = new DelegateCommand<string>(OnNavigateCommand);
            Initialize();
        }

        private async void OnNavigateCommand(string page)
        {
            if (IsBusy || IsLoading)
            {
                //Message Toast
                return;
            }
            IsLoading = true;
            await _navigationService.NavigateAsync($"/CustomMasterDetailPageView/NavigationPage/{FunctionName.Home}/{page}").ConfigureAwait(false);
            IsLoading = false;
        }

        protected void Initialize()
        {
            _navigationMenuItems = new ObservableCollection<MenuItemModel>();
            _navigationMenuItems.Add(new MenuItemModel { Text = TranslateManagerHelper.Convert("home").ToUpperInvariant(), Icon = "icon-tab_home", Url = FunctionName.Home, IsActive = true });
            _navigationMenuItems.Add(new MenuItemModel { Text = TranslateManagerHelper.Convert("my_account").ToUpperInvariant(), Icon = "icon-tab_my-account", Url = FunctionName.Account, IsActive = true });
            _navigationMenuItems.Add(new MenuItemModel { Text = TranslateManagerHelper.Convert("transfers").ToUpperInvariant(), Icon = "icon-tab_transfers", Url = FunctionName.Transfers, IsActive = true });
            _navigationMenuItems.Add(new MenuItemModel { Text = TranslateManagerHelper.Convert("merchant_payment").ToUpperInvariant(), Icon = "icon-merchant_payment", Url = FunctionName.MerchantPayment, IsActive = true });
            _navigationMenuItems.Add(new MenuItemModel { Text = TranslateManagerHelper.Convert("add_kash").ToUpperInvariant(), Icon = "icon-kash_in", Url = FunctionName.AddKah, IsActive = true });
            _navigationMenuItems.Add(new MenuItemModel { Text = TranslateManagerHelper.Convert("kola_pay").ToUpperInvariant(), Icon = "icon-kola_pay", Url = FunctionName.KolaPay, IsActive = true });
            _navigationMenuItems.Add(new MenuItemModel { Text = TranslateManagerHelper.Convert("options").ToUpperInvariant(), Icon = "icon-tab_options", Url = FunctionName.Options, IsActive = true });
        }
    }
}
