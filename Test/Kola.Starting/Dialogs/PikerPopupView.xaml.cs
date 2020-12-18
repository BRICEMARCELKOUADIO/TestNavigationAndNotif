using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kola.Starting.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PikerPopupView : PopupPage
    {
        public PikerPopupView()
        {
            InitializeComponent();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((PikerPopupViewModel)BindingContext).SearchTapped(SearchText.Text);          
        }
    }
}