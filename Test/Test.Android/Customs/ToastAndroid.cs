using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Kola.Droid.Customs;
using Plugin.CurrentActivity;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(ToastAndroid))]
namespace Kola.Droid.Customs
{
    public class ToastAndroid : Kola.Infrastructure.Controls.ToastMessage.IMessage
    {
        public void ShowToast(string message)
        {
            //Obsolete
            //Activity activity = CrossCurrentActivity.Current.Activity;
            //Toast.MakeText(Forms.Context, message, ToastLength.Long).Show();

            Android.Widget.Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
        }

        public void ShowSnackBar(string message)
        {
            Activity activity = CrossCurrentActivity.Current.Activity;
            Snackbar.Make(activity.FindViewById(Android.Resource.Id.Content), message, Snackbar.LengthLong).Show();
        }

        public void ShowSnackBarNetwork(string message, bool isSuccess = false)
        {
            Activity activity = CrossCurrentActivity.Current.Activity;
            var snack = Snackbar.Make(activity.FindViewById(Android.Resource.Id.Content), message, Snackbar.LengthLong);

            //TextView textView = (TextView)snack.View. FindViewById(Android.Resource.Id.Text1);
            //textView.SetTextColor(Android.Graphics.Color.White);
            //snack.View.TextAlignment = Android.Views.TextAlignment.Center;
            var color = isSuccess ? Android.Graphics.Color.Green : Android.Graphics.Color.Red;
            snack.View.SetBackgroundColor(color);
            snack.Show();
        }


        public void ShowSnackBarWithAction(string message, Action<object> action, string actionText = "Ok", string actionTextColor = "#F48120")
        {
            Activity activity = CrossCurrentActivity.Current.Activity;
            var snack = Snackbar.Make(activity.FindViewById(Android.Resource.Id.Content), message, Snackbar.LengthLong);
            if (actionText != null && action != null)
            {
                snack.SetAction(actionText, action);
            }

            snack.Show();
        }
    }
}