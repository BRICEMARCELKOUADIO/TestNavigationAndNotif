using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Kola.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(DatePicker), typeof(CustomDatePikerRenderer))]
namespace Kola.Droid.Renderers
{
    public class CustomDatePikerRenderer : DatePickerRenderer
    {
        public CustomDatePikerRenderer()
        {
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (Control != null)
            {
                Control.Background = null;
            }
        }
    }
}