using System;
using System.Collections.Generic;
using System.Text;

namespace Kola.Infrastructure.Logger.Abstractions
{
    public interface  ILogWriter
    {
        void LogWrite(string logMessage);

    }
}
