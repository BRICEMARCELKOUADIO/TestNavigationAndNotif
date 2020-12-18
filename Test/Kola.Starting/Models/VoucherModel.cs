using System;
using System.Collections.Generic;
using System.Text;

namespace Kola.Starting.Models
{
    public class VoucherModel
    {
        public long Id { get; set; }
        public long CurrencyId { get; set; }
        public string QrCode { get; set; }
        public string VoucherNumber { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string Icon { get; set; }
    }
}
