using Plugin.FirebasePushNotification;
using Prism;
using Prism.Ioc;
using Prism.Services;
using Prism.Unity;
using System;
using Test.ViewModels;
using Test.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Test
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer Initializer = null) : base(Initializer)
        {
            //MainPage = new MainPage();
            NavigationService.NavigateAsync("Welcome");
        }

        protected override void OnStart()
        {
            // Handle when your app starts

        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //Navigation
            containerRegistry.RegisterForNavigation<MasterDetailPageView, MasterDetailViewModel>();
            containerRegistry.RegisterForNavigation<NavigationPage>();
            //Page
            containerRegistry.RegisterForNavigation<HomeView, HomeViewModel>("Home");
            containerRegistry.RegisterForNavigation<MainPageView, MainPageViewModel>("Welcome");
            containerRegistry.RegisterForNavigation<Page1View, Page1ViewModel>("Page1");
        }

        protected override void OnInitialized()
        {
            InitializeComponent();

            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN : {p.Token}");
            };

            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                if (p.Data != null)
                {
                    var alert = Container.Resolve<IPageDialogService>();
                    alert.DisplayAlertAsync(p.Data["title"].ToString(), p.Data["body"].ToString(), "OK");
                }
                 
            };

            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine("Opened");
                foreach (var data in p.Data)
                {
                    System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                }
            };

            CrossFirebasePushNotification.Current.OnNotificationAction += (s, p) =>
            {
                
            };

            CrossFirebasePushNotification.Current.OnNotificationDeleted += (s, p) =>
            {


            };
        }
    }
}
