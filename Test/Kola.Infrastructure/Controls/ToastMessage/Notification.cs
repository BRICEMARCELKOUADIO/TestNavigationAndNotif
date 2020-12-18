using Plugin.FirebasePushNotification;
using Plugin.LocalNotifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kola.Infrastructure.Controls.ToastMessage
{
    public static class Notification
    {
        /// <summary>
        /// Show Notification With Scheduler
        /// </summary>
        /// <param name="ptitle"></param>
        /// <param name="pdescription"></param>
        /// <param name="pid"></param>
        /// <param name="pdelay">pdelay before to show (secondes)</param>
        public static void ShowNotificationScheduler(string ptitle, string pdescription, int pid, int pdelay = 10)
        {
            CrossLocalNotifications.Current.Show(ptitle, pdescription, pid, DateTime.Now.AddSeconds(pdelay));
        }

        /// <summary>
        /// Show Notification Immediatly
        /// </summary>
        /// <param name="ptitle"></param>
        /// <param name="pdescription"></param>
        public static void ShowNotificationImmediatly(string ptitle, string pdescription)
        {
            CrossLocalNotifications.Current.Show(ptitle, pdescription);
        }
    }
}
