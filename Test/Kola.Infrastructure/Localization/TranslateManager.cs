using Kola.Infrastructure.Hepler;
using Kola.Infrastructure.Localization.Contract;
using System;
using System.Collections;
using System.Reflection;
using System.Resources;

namespace Kola.Infrastructure.Localization
{
    public class TranslateManager : ITranslateManager
    {
        public bool IsLoaded { get; private set; }
        public string CurrentLanguage { get; private set; }
        public Hashtable TranslateHastable { get; private set; }

        public TranslateManager()
        {
            CurrentLanguage = CrossSettingsHelper.GetValueOrDefault("Language", "", Constants.LocalConfigVirtualFile);
        }

        public void Load()
        {
            TranslateHastable = new Hashtable();
            TranslateHastable = Get($"{CurrentLanguage}");
            IsLoaded = true;
        }
        
        public void SetCurrentLangage(string planguage)
        {
            // Set This culture
            bool ok = CrossSettingsHelper.AddOrUpdateValue("Language", planguage, Constants.LocalConfigVirtualFile);
            if (ok)
            {
                CurrentLanguage = planguage;
                // Load langeage selected -> resx file
                Load();
            }
            else
            {
                Console.WriteLine($"Erreur survenue sur TranslateManager.SetCurrentLangage -->  param = {planguage}");
            }
        }

        private Hashtable Get(string planguage)
        {
            if (planguage.Contains("fr"))
            {
                planguage = Constants.en_US; // fr_FR -> lock englis translate
            }
            else  /*when this device language doesn't found --> default language is English*/
            {
                planguage = Constants.en_US;
            }

            var table = new Hashtable();
            Assembly resourceAssembly = Assembly.GetExecutingAssembly();
            try
            {
                using (ResourceReader reader = new ResourceReader(resourceAssembly.GetManifestResourceStream($"Kola.Infrastructure.Localization.{planguage}.resources")))
                {
                    IDictionaryEnumerator dic = reader.GetEnumerator();
                    while (dic.MoveNext())
                    {
                        table.Add(dic.Key as string, dic.Value as string);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write($"TranslateManager: Error occured to load a language {planguage} resources ==> {ex.Message}");
            }
            return table;
        }
    }
}
