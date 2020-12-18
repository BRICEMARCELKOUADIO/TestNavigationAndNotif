using Kola.Infrastructure.Localization.Contract;
using System;
using DryIoc;
using Xamarin.Forms;
using Prism.Ioc;

namespace Kola.Infrastructure.Helper
{
    public static class TranslateManagerHelper
    {
        public static string Convert(string ptext)
        {
            try
            {
                if (string.IsNullOrEmpty(ptext) || ptext?.Length == 0)
                    return "";

                var _translate = ((Prism.PrismApplicationBase)Application.Current).Container.Resolve<ITranslateManager>();
                return string.IsNullOrEmpty(_translate.TranslateHastable[$"{ptext.ToLower()}"] as string) ? ptext : _translate.TranslateHastable[$"{ptext.ToLower()}"] as string;
            }
            catch (Exception ex)
            {
                Console.Write($"TranslateConverter: Error occured to translate {ptext} ==> {ex.Message}");
                return ptext;
            }
        }

        public static string ConvertToUpper(string ptext)
        {
            if (string.IsNullOrEmpty(ptext) || ptext?.Length == 0)
                return "";

            return Convert(ptext).ToUpper();
        }
    }
}
