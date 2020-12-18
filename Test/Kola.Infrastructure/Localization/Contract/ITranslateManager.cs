using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Kola.Infrastructure.Localization.Contract
{
    public interface ITranslateManager
    {
        bool IsLoaded { get; }
        string CurrentLanguage { get; }
        Hashtable TranslateHastable { get; }
        void Load();
        void SetCurrentLangage(string planguage);
    }
}
