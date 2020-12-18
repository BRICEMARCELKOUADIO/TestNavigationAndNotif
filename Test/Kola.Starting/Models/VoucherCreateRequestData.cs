namespace Kola.Starting.Models
{
    public class VoucherCreateRequestData
    {
        public string BusinessCategory { get; set; }
        public string BusinessSubCategory { get; set; }
        public string vPhone { get; set; } // recipient phone
        public decimal vAmount { get; set; } // amount
        public string SourceAccountNumber { get; set; }
        public string Company { get; set; } // issuier of voucher
        public string CompanyName { get; set; } // issuier of voucher

        public string Fname { get; set; } // beneficiary's first name
        public string Lname { get; set; } // beneficiary's last name
        public long IdentificationTypeId { get; set; } // identity type (PASSPORT, ;..)
        public string IdNUmber { get; set; } // identity number.
    }
}
