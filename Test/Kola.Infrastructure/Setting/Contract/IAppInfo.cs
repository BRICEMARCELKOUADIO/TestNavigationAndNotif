using reewire.core.services.models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kola.Infrastructure.Setting.Contract
{
    public interface IAppInfo
    {
        applicationdto App { get; }
        void Load();
        applicationdto GetApp();
    }
}
