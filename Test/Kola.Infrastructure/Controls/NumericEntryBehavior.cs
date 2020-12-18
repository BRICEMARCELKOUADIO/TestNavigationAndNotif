using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Kola.Infrastructure.Controls
{
    public class NumericEntryBehavior : Behavior<Entry>
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

            double _;
            if (!double.TryParse(e.NewTextValue, out _))
                ((Entry)sender).Text = e.OldTextValue;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.TextChanged -= TextChange_Handler;
        }
    }
}
