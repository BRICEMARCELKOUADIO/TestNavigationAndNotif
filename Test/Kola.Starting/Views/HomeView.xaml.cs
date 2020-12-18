using Kola.Starting.ViewModels;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace Kola.Starting.Views
{
    public partial class HomeView : ContentPage
    {
        public HomeView(HomeViewModel homeViewModel)
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            ((HomeViewModel)BindingContext).OpenLogoutPopup();
            return true;
            //return base.OnBackButtonPressed();
        }

        private void hamburgerButton_Clicked(object sender, System.EventArgs e)
        {
            //navigationDrawer.ToggleDrawer();
        }
    }
}
