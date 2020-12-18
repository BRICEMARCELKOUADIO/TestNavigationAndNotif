using System.Collections.Generic;
using System.Text;

namespace Kola.Infrastructure.Models
{
    public class StatementDetails
    {
        public string OperationNumber { get; set; }
        public string OperationType { get; set; }
        public string OperationTime { get; set; }
        public string OpDate { get; set; }
        public string OperationSign { get; set; }
        public string Description { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public bool IsPositive { get; set; }

        public StatementDetails()
        { }
    }
}
