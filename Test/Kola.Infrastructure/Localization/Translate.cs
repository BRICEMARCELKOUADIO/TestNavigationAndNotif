using Kola.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kola.Infrastructure.Localization
{
    [ContentProperty("Text")]
    public class Translate : IMarkupExtension
    {
        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return "";

            return TranslateManagerHelper.Convert(Text);
        }
    }
}
