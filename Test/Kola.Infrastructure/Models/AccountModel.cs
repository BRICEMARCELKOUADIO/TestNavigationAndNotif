using System;
using System.Collections.Generic;
using System.Text;

namespace Kola.Infrastructure.Models
{
    public class AccountModel
    {
        public string AccountNumber { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Balance { get; set; }
        public string Commission { get; set; }
        public string Currency { get; set; }
        public bool HasCommission { get; set; }
    }


}
