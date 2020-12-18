using Kola.Infrastructure;
using Kola.Infrastructure.Hepler;
using Kola.Services.Infrastructure.LocalStorage.Contract;
using Plugin.FirebasePushNotification;

namespace Kola.Services.Infrastructure.LocalStorage
{
    public class UserInformationLocalStorage : IUserInformationLocalStorage
    {
        public bool IsLoaded { get; private set; }
        public bool DontShowMeAgain { get; set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string FUserName { get; private set; }
        public string FPassword { get; private set; }
        public bool RememberMe { get; private set; }
        public bool FingerPrint { get; private set; }
       
        public string UserForNotification { get; private set; }
        public string DeviceTokenNotification { get; private set;  }
        public bool HasSubscribeNotification { get; private set; }

        public void Load()
        {
            if (!IsLoaded)
            {
                DontShowMeAgain = CrossSettingsHelper.GetValueOrDefault("DontShowMeAgain", false, Constants.LocalConfigVirtualFileUser);
                UserName = CrossSettingsHelper.GetValueOrDefault("UserName", "", Constants.LocalConfigVirtualFileUser);
                Password = CrossSettingsHelper.GetValueOrDefault("Password", "", Constants.LocalConfigVirtualFileUser);
                FUserName = CrossSettingsHelper.GetValueOrDefault("FUserName", "", Constants.LocalConfigVirtualFileUser);
                FPassword = CrossSettingsHelper.GetValueOrDefault("FPassword", "", Constants.LocalConfigVirtualFileUser);
                RememberMe = CrossSettingsHelper.GetValueOrDefault("RememberMe", false, Constants.LocalConfigVirtualFileUser);
                FingerPrint = CrossSettingsHelper.GetValueOrDefault("FingerPrint", false, Constants.LocalConfigVirtualFileUser);
                UserForNotification = CrossSettingsHelper.GetValueOrDefault("UserForNotification", "", Constants.LocalConfigVirtualFileUser);
                HasSubscribeNotification = !string.IsNullOrEmpty(UserForNotification);

                DeviceTokenNotification = CrossSettingsHelper.GetValueOrDefault("DeviceTokenNotification", "", Constants.LocalConfigVirtualFileUser);
                IsLoaded = true;
            }
        }

        public void Clear()
        {
            CrossSettingsHelper.ClearVirtualFile(Constants.LocalConfigVirtualFileUser);
            Reload();
        }

        public void Remove(string pkey)
        {
            CrossSettingsHelper.Remove(pkey, Constants.LocalConfigVirtualFileUser);
            Reload();
        }

        private void Reload()
        {
            IsLoaded = false;
            this.Load();
        }

        public void SetCredentials(string puserName, string ppassword)
        {
            bool ok = CrossSettingsHelper.AddOrUpdateValue("UserName", puserName, Constants.LocalConfigVirtualFileUser);
            if (ok)
            {
                UserName = puserName;
            }

            ok = CrossSettingsHelper.AddOrUpdateValue("Password", ppassword, Constants.LocalConfigVirtualFileUser);
            if (ok)
            {
                Password = ppassword;
            }
        }

        public void SetFingerCredentials(string puserName, string ppassword)
        {
            bool ok = CrossSettingsHelper.AddOrUpdateValue("FUserName", puserName, Constants.LocalConfigVirtualFileUser);
            if (ok)
            {
                FUserName = puserName;
            }

            ok = CrossSettingsHelper.AddOrUpdateValue("FPassword", ppassword, Constants.LocalConfigVirtualFileUser);
            if (ok)
            {
                FPassword = ppassword;
            }
        }

        public void SetUserName(string puserName)
        {
            bool ok = CrossSettingsHelper.AddOrUpdateValue("UserName", puserName, Constants.LocalConfigVirtualFileUser);
            if (ok)
            {
                UserName = puserName;
            }
        }

        public void SetPassword(string ppassword)
        {
            bool ok = CrossSettingsHelper.AddOrUpdateValue("Password", ppassword, Constants.LocalConfigVirtualFileUser);
            if (ok)
            {
                Password = ppassword;
            }
        }


        public void SetDontShowMeAgain(bool pdontShowMeAgain)
        {
            bool ok = CrossSettingsHelper.AddOrUpdateValue("DontShowMeAgain", pdontShowMeAgain, Constants.LocalConfigVirtualFileUser);
            if (ok)
            {
                DontShowMeAgain = pdontShowMeAgain;
            }
        }

        public void SetRememberMe(bool prememebrMe)
        {
            bool ok = CrossSettingsHelper.AddOrUpdateValue("RememberMe", prememebrMe, Constants.LocalConfigVirtualFileUser);
            if (ok)
            {
                RememberMe = prememebrMe;
            }
        }

        public void SetFingerPrint(bool pFingerPrint)
        {
            bool ok = CrossSettingsHelper.AddOrUpdateValue("FingerPrint", pFingerPrint, Constants.LocalConfigVirtualFileUser);
            if (ok)
            {
                FingerPrint = pFingerPrint;
            }
        }

        public void SetFingerPrintForThisInstance(bool pFingerPrint)
        {
            FingerPrint = pFingerPrint;
        }

        public void SetDeviceTokenNotification(string ptoken)
        {
            bool ok = CrossSettingsHelper.AddOrUpdateValue("DeviceTokenNotification", ptoken, Constants.LocalConfigVirtualFileUser);
            if (ok)
            {
                DeviceTokenNotification = ptoken;
            }
        }

        private void SetUserNameForNotification(string puserNameForNotification)
        {
            bool ok = CrossSettingsHelper.AddOrUpdateValue("UserForNotification", puserNameForNotification, Constants.LocalConfigVirtualFileUser);
            if (ok)
            {
                UserForNotification = puserNameForNotification;
            }
            HasSubscribeNotification = !string.IsNullOrEmpty(UserForNotification);
        }


        public void SubscribeNotification(string userName)
        {
            // subscribe new user and set for notification
            CrossFirebasePushNotification.Current.Subscribe($"{ApiConstants.NotificationQueue}");
            CrossFirebasePushNotification.Current.Subscribe($"{ApiConstants.NotificationQueue}_{userName}");
            SetUserNameForNotification(userName);
        }

        public void UnSubscribeNotification()
        {
            // unsubscribe new user and set for notification
            CrossFirebasePushNotification.Current.Unsubscribe($"{ApiConstants.NotificationQueue}");
            CrossFirebasePushNotification.Current.Unsubscribe($"{ApiConstants.NotificationQueue}_{UserForNotification}");
            SetUserNameForNotification("");
        }
    }
}
