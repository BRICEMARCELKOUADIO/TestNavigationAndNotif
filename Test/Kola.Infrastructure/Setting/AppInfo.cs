using Kola.Infrastructure.Controls.ToastMessage;
using Kola.Infrastructure.Helper;
using Kola.Infrastructure.Setting.Contract;
using Kola.Infrastructure.UserAuthentication.Contracts;
using reewire.core.services.contracts.platform;
using reewire.core.services.models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kola.Infrastructure.Setting
{
    public class AppInfo : IAppInfo
    {
        private readonly IFMPlatform _fMPlatform;
        private readonly IAuthentication _authentication;

        public bool IsLoaded { get; private set; }
        public applicationdto App { get; private set; }

        public AppInfo(IFMPlatform fMPlatform, IAuthentication authentication)
        {
            _fMPlatform = fMPlatform ?? throw new ArgumentException(nameof(fMPlatform));
            _authentication = authentication ?? throw new ArgumentException(nameof(authentication));
            App = new applicationdto();
        }

        public async void Load()
        {
            if (!IsLoaded)
            {
                var response = await _fMPlatform.GetApplicationBySystemId(_authentication.GetToken(), ApiConstants.SystemId);
                if (response?.isuccess == true)
                {
                    App = response.data;
                    IsLoaded = true;
                }
                else
                {
                    DependencyService.Get<IMessage>().ShowSnackBarNetwork(TranslateManagerHelper.Convert("network_unavailable_token"));
                }
            }            
        }

        public applicationdto GetApp()
        {
            if(!IsLoaded)
            {
                Load();
            }
            return App;
        }
    }
}
