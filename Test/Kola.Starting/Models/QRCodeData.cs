using Newtonsoft.Json;
using reewire.core.services.models;

namespace Kola.Starting.Models
{
    public class QRCodeData : QRCodeData4StandardPayment
    {
        public string MerchantName { get; set; }
        public string Currency { get; set; }
    }
}
