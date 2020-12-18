using Kola.Infrastructure;
using Kola.Infrastructure.Config;
using Kola.Infrastructure.Config.Contract;
using Kola.Infrastructure.Controls.ToastMessage;
using Kola.Infrastructure.Design;
using Kola.Infrastructure.Helper;
using Kola.Infrastructure.Localization;
using Kola.Infrastructure.Localization.Contract;
using Kola.Infrastructure.Logger;
using Kola.Infrastructure.Logger.Abstractions;
using Kola.Infrastructure.Models;
using Kola.Infrastructure.Setting.Contract;
using Kola.Infrastructure.UserAuthentication.Contracts;
using Kola.Infrastructure.UserAuthentication.UserAuthentication;
using Kola.Services.Infrastructure.LocalStorage;
using Kola.Services.Infrastructure.LocalStorage.Contract;
using Plugin.FirebasePushNotification;
using Prism;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Plugin.Popups;
using Prism.Services;
using reewire.core.services.contracts.repository;
using reewire.core.services.contracts.services;
using reewire.core.services.repository;
using reewire.core.services.services;
using System;
using Test.ViewModels;
using Test.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Test
{
   [Preserve(AllMembers = true)]
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */

        [Preserve(AllMembers = true)]
        public App() : this(null) { }

        [Preserve(AllMembers = true)]
        public App(IPlatformInitializer initializer) : base(initializer) { }

        [Preserve(AllMembers = true)]
        protected override async void OnInitialized()
        {
            //Use License synfusion
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Constants.SfLicense);

            //Load default Langeage resx file
            var translate = Container.Resolve<ITranslateManager>();
            if (!translate.IsLoaded)
            {
                translate.Load();
            }

            InitializeComponent();

            //Load user local storage
            var userInfo = Container.Resolve<IUserInformationLocalStorage>();
            if (!userInfo.IsLoaded)
            {
                userInfo.Load();
                //Load Theme(picto, color) 
                var xmlLoader = Container.Resolve<IXmlLoader>();
                await new Theme().Load(xmlLoader);

                //Load service configuration
                var json = Container.Resolve<IJsonLoader<RootConfiguration>>();
                // Choose one specific environment from this list : [Development, Test, Production]
                RootConfiguration config = await json.GetAsync("Test");
                ApiConstants.LoadApiConfiguration(config?.ApiConfiguration);

                var gr = Container.Resolve<IGenericRepository>();
                gr.ApplicationBaseApiUrl = ApiConstants.ApplicationBaseApiUrl;
                gr.AuthBaseApiUrl = ApiConstants.AuthBaseApiUrl;
                gr.CatalogBaseApiUrl = ApiConstants.CatalogBaseApiUrl;
                gr.WebcBaseApiUrl = ApiConstants.FetaureBaseApiUrl;
                gr.PlatformBaseApiUrl = ApiConstants.PlateformBaseApiUrl;
            }

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            // Load Authentication
            var authentication = Container.Resolve<IAuthentication>();
            authentication.GetToken();

            // Load IAppInfo
            var appInfo = Container.Resolve<IAppInfo>();
            appInfo.Load();

            if (authentication.IsConnected) // Si Connecté -> redirection pge de connexion
            {
                authentication.Logout();
                await NavigationService.NavigateAsync("Login");
            }
            else // Otherwise
            {
                await NavigationService.NavigateAsync("LoginOrSigin");
            }


            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Opened");
            };

            CrossFirebasePushNotification.Current.OnNotificationDeleted += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Dismissed");
            };

            CrossFirebasePushNotification.Current.OnNotificationAction += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Action");
            };

            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                try
                {
                    if (p == null)
                    {
                        return;
                    }
                    else
                    {
                        string title = p.Data.ContainsKey("title") ? p.Data["title"] as string : "";
                        string body = p.Data.ContainsKey("body") ? p.Data["body"] as string : "";
                        Notification.ShowNotificationImmediatly(title, body);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            };

            var userLocalStorage = Container.Resolve<IUserInformationLocalStorage>();
            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                userLocalStorage.SetDeviceTokenNotification(p?.Token);
            };
        }

        protected async override void OnResume()
        {
            base.OnResume();
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

            // Activer le handle de reload token
            var authentication = Container.Resolve<IAuthentication>();
            authentication.HandleTokenTimer(); // Timer

            authentication.SaveStartActivityDate();
            authentication.DisconnectWhenBackToSleepCurrentUser(); // Timer
            if (authentication.IsGuest)
            {

            }
            else if (authentication.UseExternal)
            {
                authentication.SetUseExternal(false);
            }
            else if (!authentication.IsConnected)
            {
                authentication.SetGuest(true);
                await NavigationService.NavigateAsync("Login");
            }
        }

        protected override void OnSleep()
        {
            base.OnSleep();

            var authentification = Container.Resolve<IAuthentication>();
            if (authentification.IsConnected)
            {
                authentification.SaveSleepActivityDate();
            }
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var current = Connectivity.NetworkAccess;

            switch (current)
            {
                case NetworkAccess.Unknown:
                case NetworkAccess.None:
                    DependencyService.Get<IMessage>().ShowSnackBarNetwork(TranslateManagerHelper.Convert("network_unavailable"));
                    break;
                case NetworkAccess.Local:
                case NetworkAccess.ConstrainedInternet:
                    DependencyService.Get<IMessage>().ShowSnackBarNetwork(TranslateManagerHelper.Convert("network_limited"));
                    break;
                case NetworkAccess.Internet:
                    DependencyService.Get<IMessage>().ShowSnackBarNetwork(TranslateManagerHelper.Convert("network_available"), true);
                    break;
            }
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //******************************************
            //Navigation
            containerRegistry.RegisterForNavigation<MasterDetailPageView, CustomMasterDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<NavigationPage>();
            //Page
            containerRegistry.RegisterForNavigation<HomeView, HomeViewModel>("Home");
            containerRegistry.RegisterForNavigation<MainPageView, MainPageViewModel>("Welcome");
            containerRegistry.RegisterForNavigation<Page1View, Page1ViewModel>("Page1");

            //*****************************************

            containerRegistry.RegisterSingleton<ITranslateManager, TranslateManager>();
            containerRegistry.RegisterSingleton<IXmlLoader, XmlLoader>();
            containerRegistry.RegisterSingleton<IJsonLoader<RootConfiguration>, JsonLoader<RootConfiguration>>();
            containerRegistry.RegisterSingleton<IUserInformationLocalStorage, UserInformationLocalStorage>();
            containerRegistry.RegisterSingleton<IGenericRepository, GenericRepository>();
            containerRegistry.RegisterSingleton<IFeature, Feature>();
            containerRegistry.RegistryCatalog();
            containerRegistry.RegisterSingleton<IAuthentication, Authentication>();
            containerRegistry.RegisterSingleton<IAppInfo, Kola.Infrastructure.Setting.AppInfo>();

            //containerRegistry.RegisterSingleton<ViewModelBase>();
            containerRegistry.RegisterPopupNavigationService();
            containerRegistry.RegisterForNavigation<CustomMasterDetailPageView, CustomMasterDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<NavigationPage>();

            // Register for CrossFirebasePushNotification
            CrossFirebasePushNotification.Current.RegisterForPushNotifications();

            //For test            
            //containerRegistry.RegisterForNavigation<test, testViewModel>();

            // Log
            containerRegistry.RegisterSingleton<ILogWriter, LogWriter>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            //base.ConfigureModuleCatalog(moduleCatalog);

            //moduleCatalog.AddModule(new ModuleInfo(typeof(Starting.StartingModule))
            //{
            //    InitializationMode = InitializationMode.WhenAvailable
            //});
            //moduleCatalog.AddModule(new ModuleInfo(typeof(Options.OptionsModule))
            //{
            //    InitializationMode = InitializationMode.WhenAvailable
            //});
            //moduleCatalog.AddModule(new ModuleInfo(typeof(Transfers.TransfersModule))
            //{
            //    InitializationMode = InitializationMode.WhenAvailable
            //});
            //moduleCatalog.AddModule(new ModuleInfo(typeof(Account.AccountModule))
            //{
            //    InitializationMode = InitializationMode.WhenAvailable
            //});
        }
    }

    internal static class RegistryExtension
    {
        public static void RegistryCatalog(this IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterSingleton<IAccountStatus, AccountStatus>();
            //containerRegistry.RegisterSingleton<IAccountStypes, AccountStypes>();
            //containerRegistry.RegisterSingleton<IAccountTypes, AccountTypes>();
            //containerRegistry.RegisterSingleton<IApplicationSubTypes, ApplicationSubTypes>();
            //containerRegistry.RegisterSingleton<IApplicationTypes, ApplicationTypes>();
            //containerRegistry.RegisterSingleton<IChannels, Channels>();
            //containerRegistry.RegisterSingleton<ICommissionTypes, CommissionTypes>();
            //containerRegistry.RegisterSingleton<IDocumentTypes, DocumentTypes>();
            //containerRegistry.RegisterSingleton<IGenders, Genders>();
            //containerRegistry.RegisterSingleton<IIdentificationTypes, IdentificationTypes>();
            //containerRegistry.RegisterSingleton<IInterfaceTypes, InterfaceTypes>();
            //containerRegistry.RegisterSingleton<ILanguages, Languages>();
            //containerRegistry.RegisterSingleton<ILimitTypes, LimitTypes>();
            //containerRegistry.RegisterSingleton<IMessageTypes, MessageTypes>();
            //containerRegistry.RegisterSingleton<IMobileStatus, MobileStatus>();
            //containerRegistry.RegisterSingleton<IModuleCategories, ModuleCategories>();
            //containerRegistry.RegisterSingleton<INotificationTypes, NotificationTypes>();
            //containerRegistry.RegisterSingleton<IOperationTypes, OperationTypes>();
            //containerRegistry.RegisterSingleton<IOperators, Operators>();
            //containerRegistry.RegisterSingleton<IOrganizations, Organizations>();
            //containerRegistry.RegisterSingleton<IPaymentMethods, PaymentMethods>();
            //containerRegistry.RegisterSingleton<IPeriodTypes, PeriodTypes>();
            //containerRegistry.RegisterSingleton<IPivotCurrencies, PivotCurrencies>();
            //containerRegistry.RegisterSingleton<IQRCode, QRCode>();
            //containerRegistry.RegisterSingleton<IRoleCategories, RoleCategories>();
            //containerRegistry.RegisterSingleton<ITransactionNatures, TransactionNatures>();
            //containerRegistry.RegisterSingleton<ITransactionValidationModes, TransactionValidationModes>();
            //containerRegistry.RegisterSingleton<IUserCategories, UserCategories>();
            //containerRegistry.RegisterSingleton<IUserStatus, UserStatus>();
            //containerRegistry.RegisterSingleton<IUserSubSubTypes, UserSubSubTypes>();
            //containerRegistry.RegisterSingleton<IUserSubTypes, UserSubTypes>();
            //containerRegistry.RegisterSingleton<IUserTypes, UserTypes>();
            //containerRegistry.RegisterSingleton<IZoneTypes, ZoneTypes>();
            //containerRegistry.RegisterSingleton<IApplications, Applications>();

            //containerRegistry.RegisterSingleton<ICountries, Countries>();
            //containerRegistry.RegisterSingleton<IFMPlatform, FMPlatform>();
            //containerRegistry.RegisterSingleton<IApplications, Applications>();
        }
    }
}
