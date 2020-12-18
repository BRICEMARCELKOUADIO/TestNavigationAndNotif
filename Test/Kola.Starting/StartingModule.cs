using Prism.Ioc;
using Prism.Modularity;
using Kola.Starting.Views;
using Kola.Starting.ViewModels;
using Kola.Infrastructure.Navigation;
using Kola.Starting.Dialogs;

namespace Kola.Starting
{
    public class StartingModule : IModule
    {
        public void Initialize()
        {

        }

        public void OnInitialized()
        {

        }

        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<LoginOrSiginView, LoginOrSiginViewModel>(FunctionName.LoginOrSigin);
            containerRegistry.RegisterForNavigation<LoginView, LoginViewModel>(FunctionName.Login);
            containerRegistry.RegisterForNavigation<SiginUpView, SiginUpViewModel>(FunctionName.SiginUp);
            containerRegistry.RegisterForNavigation<FigerprintPopupView, FigerprintPopupViewModel>(PopupName.FigerprintPopup);
            containerRegistry.RegisterForNavigation<PinCodeView, PinCodeViewModel>(FunctionName.PinCode);
            containerRegistry.RegisterForNavigation<HomeView, HomeViewModel>(FunctionName.Home);
            containerRegistry.RegisterForNavigation<KashView, KashViewModel>(FunctionName.Kash);
            containerRegistry.RegisterForNavigation<LogoutPopupView, LogoutPopupViewModel>(FunctionName.Logout);
            containerRegistry.RegisterForNavigation<PinCodePopupView, PinCodePopupViewModel>(PopupName.PinCodePopup);
            containerRegistry.RegisterForNavigation<RejectPopupView, RejectPopupViewModel>(PopupName.RejectPopup);
            containerRegistry.RegisterForNavigation<SuccessfullPopupView, SuccessfullPopupViewModel>(PopupName.SuccessfullPopup);
            containerRegistry.RegisterForNavigation<PikerPopupView, PikerPopupViewModel>(PopupName.PikerPopup);
            containerRegistry.RegisterForNavigation<PikerWalletPopupView, PikerWalletPopupViewModel>(PopupName.PikerWalletPopup);
            containerRegistry.RegisterForNavigation<AddKahView, AddKahViewModel>(FunctionName.AddKah);
            containerRegistry.RegisterForNavigation<MerchantPaymentView, MerchantPaymentViewModel>(FunctionName.MerchantPayment);
            containerRegistry.RegisterForNavigation<ManualPaymentView, ManualPaymentViewModel>(FunctionName.MerchantPaymentManual);
            containerRegistry.RegisterForNavigation<KolaPayView, KolaPayViewModel>(FunctionName.KolaPay);
            containerRegistry.RegisterForNavigation<AirtimeView, AirtimeViewModel>(FunctionName.Airtime);
            containerRegistry.RegisterForNavigation<UtilitiesView, UtilitiesViewModel>(FunctionName.Utilities);
            containerRegistry.RegisterForNavigation<GolView, GolViewModel>(FunctionName.Gol);
            containerRegistry.RegisterForNavigation<OthersView, OthersViewModel>(FunctionName.Others);
            containerRegistry.RegisterForNavigation<MerchantPaymentScanQrCodeView, MerchantPaymentScanQrCodeViewModel>(FunctionName.MerchantPaymentQRCode);
            containerRegistry.RegisterForNavigation<VouchersView, VouchersViewModel>(FunctionName.Vouchers);
            containerRegistry.RegisterForNavigation<CharityView, CharityViewModel>(FunctionName.Charity);
            containerRegistry.RegisterForNavigation<GiftCardView, GiftCardViewModel>(FunctionName.GiftCard);
            containerRegistry.RegisterForNavigation<GenerateQrCodeView, GenerateQrCodeViewModel>(FunctionName.GenerateQrCode);
            containerRegistry.RegisterForNavigation<ForcePincodeChangePopupView, ForcePincodeChangePopupViewModel>(FunctionName.ForcePincodeChange);
            containerRegistry.RegisterForNavigation<MessageWithActionPopupView, MessageWithActionPopupViewModel>(PopupName.MessageWithActionPopup);
            containerRegistry.RegisterForNavigation<WithBankView, WithBankViewModel>(FunctionName.KashInWithCard);

            
        }
    }
}
