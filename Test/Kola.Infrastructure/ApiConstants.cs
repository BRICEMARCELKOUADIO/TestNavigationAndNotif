using Kola.Infrastructure.Controls.ToastMessage;
using Kola.Infrastructure.Helper;
using Kola.Infrastructure.Models;
using Xamarin.Forms;

namespace Kola.Infrastructure
{
    public static class ApiConstants
    {
        public static string AuthBaseApiUrl { get; private set; } 
        public static string CatalogBaseApiUrl { get; private set; }
        public static string PlateformBaseApiUrl { get; private set; }
        public static string ApplicationBaseApiUrl { get; private set; }
        public static string FetaureBaseApiUrl { get; private set; }
        public static string TokenEndPoint { get; private set; } 
        public static string AuthSystemId { get; private set; }
        public static string AuthSecret { get; private set; }
        public static string SystemId { get; private set; }
        public static string CountryPrefix { get; private set; }
        public static string NotificationQueue { get; private set; }

        public static void LoadApiConfiguration(ApiConfiguration apiConfiguration)
        {
            if(apiConfiguration == null || apiConfiguration == default)
            {
                DependencyService.Get<IMessage>().ShowSnackBarNetwork(TranslateManagerHelper.Convert("error_occured_application_loading"));
                System.Diagnostics.Debug.WriteLine($"Appli Client error ApiConstants.LoadApiConfiguration: Error occured");
            }

            AuthBaseApiUrl = apiConfiguration.AuthBaseApiUrl;
            CatalogBaseApiUrl = apiConfiguration.CatalogBaseApiUrl;
            FetaureBaseApiUrl = apiConfiguration.FetaureBaseApiUrl;
            TokenEndPoint = apiConfiguration.TokenEndPoint;
            AuthSystemId = apiConfiguration.AuthSystemId;
            AuthSecret = apiConfiguration.AuthSecret;
            SystemId = apiConfiguration.SystemId;
            CountryPrefix = apiConfiguration.CountryPrefix;
            PlateformBaseApiUrl = apiConfiguration.PlateformBaseApiUrl;
            ApplicationBaseApiUrl = apiConfiguration.ApplicationBaseApiUrl;
            NotificationQueue = apiConfiguration.NotificationQueue;
        }
    }
}

