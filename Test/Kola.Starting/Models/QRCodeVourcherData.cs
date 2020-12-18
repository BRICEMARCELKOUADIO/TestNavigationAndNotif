namespace Kola.Starting.Models
{
    public class QRCodeVourcherData
    {
        public string Number { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string UserName { get; set; }
        public string SystemId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyId { get; set; }
        public string Owner { get; set; }

        public string PhoneNumber { get; set; } // recipient phone
        public long IdentificationTypeId { get; set; } // identity type (PASSPORT, ;..)
        public string IdNumber { get; set; } // identity number.
    }
}
