using System;
using System.Collections.ObjectModel;

namespace Kola.Infrastructure.Models
{
    public class StatetementGroup : ObservableCollection<StatementDetails>
    {
        #region Properties
        public string OperationDate { get; set; }
        #endregion Properties

        #region Constructors
        public StatetementGroup(string operationDate)
        {
            OperationDate = operationDate;
        }
        public StatetementGroup()
        {
        }

        #endregion Constructors
    }
}
