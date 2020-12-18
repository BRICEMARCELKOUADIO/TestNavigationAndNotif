namespace Kola.Services.Infrastructure.LocalStorage.Contract
{
    public interface IUserInformationLocalStorage
    {
        bool IsLoaded { get; }
        bool DontShowMeAgain { get; }
        string UserName { get; }
        string Password { get; }
        string FUserName { get; }
        string FPassword { get; }
        bool RememberMe { get; }
        bool FingerPrint { get; }

        string DeviceTokenNotification { get; }
        bool HasSubscribeNotification { get; }
        string UserForNotification { get; }
       
        /// <summary>
        /// Load Local storage data
        /// </summary>
        void Load();

        /// <summary>
        /// Clear user information local storage
        /// </summary>
        void Clear();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pkey"></param>
        void Remove(string pkey);

        /// <summary>
        /// Set Dont Show Me Again
        /// </summary>
        /// <param name="pdontShowMeAgain"></param>
        void SetDontShowMeAgain(bool pdontShowMeAgain);

        /// <summary>
        /// Set Credentials
        /// </summary>
        /// <param name="puserName"></param>
        /// <param name="ppassword"></param>
        void SetCredentials(string puserName, string ppassword);

        /// <summary>
        /// Set Credentials
        /// </summary>
        /// <param name="puserName"></param>
        /// <param name="ppassword"></param>
        void SetFingerCredentials(string puserName, string ppassword);

        /// <summary>
        /// SetUserName
        /// </summary>
        /// <param name="puserName"></param>
        void SetUserName(string puserName);

        /// <summary>
        /// SetPassword
        /// </summary>
        /// <param name="ppassword"></param>
        void SetPassword(string ppassword);

        /// <summary>
        /// Set RememebrMe
        /// </summary>
        /// <param name="prememebrMe"></param>
        void SetRememberMe(bool prememebrMe);

        /// <summary>
        /// Set RememebrMe
        /// </summary>
        /// <param name="pFingerPrint"></param>
        void SetFingerPrint(bool pFingerPrint);

        /// <summary>
        /// Set Finger Print For This Instance
        /// </summary>
        /// <param name="pFingerPrint"></param>
        void SetFingerPrintForThisInstance(bool pFingerPrint);

        /// <summary>
        /// Set Device Token Notification
        /// </summary>
        /// <param name="ptoken"></param>
        void SetDeviceTokenNotification(string ptoken);

        /// <summary>
        /// Subscribe Notification Queue
        /// </summary>
        /// <param name="userName"></param>
        void SubscribeNotification(string userName);

        void UnSubscribeNotification();
    }
}
