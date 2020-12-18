using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kola.Infrastructure.Config.Contract
{
    public interface IXmlLoader
    {
        // <summary>
        /// Gets the theme dictionnary.
        /// </summary>
        /// <param name="pfolder">The config folder name.</param>
        /// <param name="pfileName">The config file name.</param>
        /// <param name="pnodeName">The node name </param>
        /// <param name="pvalue">The value , default value egal "true" .Example value = "#12DBC" </param>
        /// <returns> Gets Dictionnary (name, value) </returns>
        Task<Dictionary<string, string>> GetAsync(string pfolder, string pfileName, string pnodeName, string pattributeValueName = "value");
    }
}
