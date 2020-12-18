using Kola.Infrastructure.EventAggregator;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Test.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomMasterDetailPageView : MasterDetailPage, IMasterDetailPageOptions
    {
        public CustomMasterDetailPageView(IEventAggregator ea)
        {
            InitializeComponent();
            ea.GetEvent<BurgerEvent>().Subscribe(BurgerDisplay);
        }

        public bool IsPresentedAfterNavigation => false;

        private void Burger_Click(object sender, EventArgs e)
        {
            IsPresented = false;
        }
        private void BurgerDisplay()
        {
            IsPresented = true;
        }
    }
}