using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Kola.Infrastructure.Controls
{
    public class UpperEntryBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += TextChange_Handler;
        }

        private void TextChange_Handler(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
                return;

            ((Entry)sender).Text = IsAlphaNumeric(e.NewTextValue) ? (e.NewTextValue).ToUpper() : e.OldTextValue;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.TextChanged -= TextChange_Handler;
        }

        public bool IsAlphaNumeric(string input)
        {
            return Regex.IsMatch(input, "^[a-zA-Z0-9]+$");
        }
    }
}

