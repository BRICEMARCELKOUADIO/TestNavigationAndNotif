using Kola.Infrastructure.Controls.ToastMessage;
using Kola.Infrastructure.Helper;
using Kola.Infrastructure.UserAuthentication.Contracts;
using reewire.core.services.authentication;
using reewire.core.services.contracts.repository;
using System;
using System.Timers;
using Xamarin.Forms;

namespace Kola.Infrastructure.UserAuthentication.UserAuthentication
{
    public class Authentication : IAuthentication
    {
        private readonly IGenericRepository _genericRepository;

        public DateTime StartActivityDate { get; private set; }
        public DateTime SleepActivityDate { get; private set; }
        public bool IsConnected { get; private set; }
        public bool UseExternal { get; private set; }
        public bool IsGuest { get; private set; }
        public TokenResult Token { get; private set; }
        public bool IsAuthenticated { get; private set; }

        public Authentication(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository ?? throw new ArgumentNullException(nameof(genericRepository));

            // au demarrage , laisser 120 secondes de repit car l init token est deja lancé
            Token = new TokenResult  {ExpiresIn = 120, Value = "" };

            //user est vue comme un invité au demarrage de l application
            IsGuest = true;
        }

        public void SaveStartActivityDate()
        {
            StartActivityDate = DateTime.Now;
        }

        public void SaveSleepActivityDate()
        {
            SleepActivityDate = DateTime.Now;
        }
        public void Logout()
        {
            SleepActivityDate = new DateTime(1900, 01, 01);
            StartActivityDate = new DateTime(1900, 01, 01);
            IsConnected = false;
        }

        public void DisconnectWhenBackToSleepCurrentUser()
        {
            var canDisconnect = StartActivityDate - SleepActivityDate;

            if (canDisconnect.TotalSeconds > Constants.TTL_To_DisconnectUser)
            {
                if (IsGuest || UseExternal) // si Invité ou utilisation fonction externe (appareil photo, qrcode, etc...)
                {
                    SaveStartActivityDate();
                }
                else if (IsConnected) // Si connecter alors deconnexion du user
                {
                    Logout();
                }
                else // Etat non gere , ça ne doit pas se produire en principe, alors A analyser si cela arrive!
                {
                    Logout();
                    System.Diagnostics.Debug.WriteLine($"Appli Client Warning Authentication.DisconnectWhenBackToSleepCurrentUser: ETAT NON GERER: PROTECTIO PAR DECONNEXION, ANALYSE A MENER");
                }
            }
            else
            {
                SaveStartActivityDate();
            }
        }

        public void Connect()
        {
            IsConnected = true;
            IsGuest = false;
        }

        public void SetGuest(bool value)
        {
            IsGuest = value;
        }

        public void SetUseExternal(bool value)
        {
            UseExternal = value;
        }

        public string GetToken()
        {
            try
            {
                if (!IsAuthenticated)
                {
                    rwtoken rt = new rwtoken(ApiConstants.AuthBaseApiUrl, ApiConstants.TokenEndPoint);                   
                    rt .GetToken(ApiConstants.AuthSystemId, ApiConstants.AuthSecret);
                    Token = rwtoken.Token;

                    if (!string.IsNullOrEmpty(Token.Value))
                        IsAuthenticated = true;
                    else
                    {
                        Token.ExpiresIn = 10; // relancer à nouveau la recup du token dans 10 secondes
                        DependencyService.Get<IMessage>().ShowSnackBarNetwork(TranslateManagerHelper.Convert("error_occured_application_loading"));
                    }                       
                }
            }
            catch (Exception ex)
            {
                DependencyService.Get<IMessage>().ShowSnackBarNetwork(TranslateManagerHelper.Convert("network_unavailable_token"));
                System.Diagnostics.Debug.WriteLine($"Appli Client error Authentication.GetToken: {ex.Message}");

                Token.ExpiresIn = 10; // relancer à nouveau la recup du token dans 10 secondes
            }
            return Token.Value;
        }

        public void HandleTokenTimer()
        {
            Timer aTimer = new Timer()
            {
                Interval = Token.ExpiresIn * 1000,
                Enabled = true
            };
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            IsAuthenticated = false;
            GetToken();
        }
    }
}
