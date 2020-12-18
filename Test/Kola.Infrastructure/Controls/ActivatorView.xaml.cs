using Rg.Plugins.Popup.Pages;
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
    public partial class ActivatorView : StackLayout
    {
        public ActivatorView()
        {
            InitializeComponent();
            RotateLoadingContinously();
        }

        async void RotateLoadingContinously()
        {
            while (true) // a CancellationToken in real life ;-)
            {
                for (int i = 1; i < 7; i++)
                {
                    if (loading.Rotation >= 360f)
                        loading.Rotation = 0;
                    await loading.RotateTo(i * (360 / 6), 300, Easing.Linear);
                }
            }
        }
    }
}