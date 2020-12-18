using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kola.Infrastructure.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HeaderPageOnlyTitle : Grid
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            propertyName: "Title",
            returnType: typeof(string),
            declaringType: typeof(HeaderPageOnlyTitle),
            defaultValue: "",
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanging: TitlePropertyChanged);

        public string Title { get; set; }
        public HeaderPageOnlyTitle()
        {
            InitializeComponent();
        }

        private static void TitlePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (HeaderPageOnlyTitle)bindable;
            control.headerTitle.Text =Helper.TranslateManagerHelper.Convert(newValue.ToString()).ToUpperInvariant();
        }
    }
}
