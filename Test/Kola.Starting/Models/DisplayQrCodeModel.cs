using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Kola.Starting.Models
{
    public class DisplayQrCodeModel
    {
        public string Title { get; set; }
        public string HigthTitle { get; set; }
        public string Code1 { get; set; }
        public string Code2 { get; set; }
        public string Currency { get; set; }
        public bool IsDisplayIcon { get; set; }
        public string Icon { get; set; }
        public string QrCode { get; set; }
        public ImageSource QrCodeImage { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}
