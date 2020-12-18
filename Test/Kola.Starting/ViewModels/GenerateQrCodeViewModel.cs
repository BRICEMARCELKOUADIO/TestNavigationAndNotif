using Kola.Infrastructure;
using Kola.Infrastructure.EventAggregator;
using Kola.Infrastructure.Navigation;
using Kola.Starting.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Kola.Starting.ViewModels
{
    public class GenerateQrCodeViewModel : ViewModelBase
    {
        private ImageSource _qrCodeImage;
        public ImageSource QrCodeImage
        {
            get { return _qrCodeImage; }
            set
            {
                SetProperty(ref _qrCodeImage, value);
            }
        }

        public Field<string> Code1 { get; set; } = new Field<string>();
        public Field<string> Code2 { get; set; } = new Field<string>();
        public Field<string> Currency { get; set; } = new Field<string>();
        public new Field<string> Icon { get; set; } = new Field<string>();
        public Field<string> TitleQrCode { get; set; } = new Field<string>();
        public Field<bool> IsDisplayIcon { get; set; } = new Field<bool>(false);

        public Field<string> CompanyId { get; set; } = new Field<string>();
        public Field<string> CompanyName { get; set; } = new Field<string>();

        private IEventAggregator _ea;
        public ICommand DisplayBurger => new DelegateCommand(OnDisplayBurger);
        public GenerateQrCodeViewModel(INavigationService navigationService, IEventAggregator ea) : base(navigationService)
        {
            _ea = ea;
            Initialize();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            IsLoading = false;
            base.OnNavigatedTo(parameters);
            var qrCodeId = parameters.GetValue<string>(Constants.QrCodeId);

            switch (qrCodeId)
            {
                case "DISPLAY_QRCODE":
                    var selectedData = parameters.GetValue<DisplayQrCodeModel>(Constants.QrCodeResponseData);
                    TitleQrCode.Value = selectedData.Title;
                    Code1.Value = selectedData.Code1;
                    Code2.Value = selectedData.Code2;
                    Currency.Value = selectedData.Currency;
                    Icon.Value = selectedData.Icon;
                    QrCodeImage = selectedData.QrCodeImage;
                    IsDisplayIcon.Value = selectedData.IsDisplayIcon;
                    Title = selectedData.HigthTitle;
                    CompanyId.Value = selectedData.CompanyId;
                    CompanyName.Value = selectedData.CompanyName;
                    break;
                default:
                    break;
            }
        }

        private void Initialize()
        {
        }

        private void OnDisplayBurger()
        {
            _ea.GetEvent<BurgerEvent>().Publish();
        }
    }
}
