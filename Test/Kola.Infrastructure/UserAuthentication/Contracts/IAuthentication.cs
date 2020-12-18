using Kola.Infrastructure.Navigation;
using reewire.core.services.authentication;
using System;
using System.Threading.Tasks;

namespace Kola.Infrastructure.UserAuthentication.Contracts
{

    public interface IAuthentication
    {
        DateTime StartActivityDate { get; }
        DateTime SleepActivityDate { get; }
        bool IsConnected { get; }
        bool UseExternal { get; }
        bool IsGuest { get; }
        bool IsAuthenticated { get; }
        TokenResult Token { get; }

        void SaveStartActivityDate();

        /// <summary>
        /// Connect user
        /// </summary>
        void Connect();

        /// <summary>
        /// Log out connected user
        /// </summary>
        void Logout();
        void DisconnectWhenBackToSleepCurrentUser();
        void SaveSleepActivityDate();

        void SetGuest(bool value);
        void SetUseExternal(bool value);

        string GetToken();
        void HandleTokenTimer();
    }
}
