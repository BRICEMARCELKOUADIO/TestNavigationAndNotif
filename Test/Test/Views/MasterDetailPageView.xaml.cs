using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Event;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Test.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterDetailPageView : MasterDetailPage, IMasterDetailPageOptions
    {
        public MasterDetailPageView(IEventAggregator ea)
        {
            InitializeComponent();
            ea.GetEvent<BurgerEvent>().Subscribe(BurgerDisplay);
        }
        private void Burger_Click(object sender, EventArgs e)
        {
            IsPresented = false;
        }
        private void BurgerDisplay()
        {
            IsPresented = true;
        }
        public bool IsPresentedAfterNavigation => false;
    }
}