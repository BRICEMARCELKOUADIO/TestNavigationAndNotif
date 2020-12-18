using System;
using System.Collections.Generic;
using System.Text;

namespace Kola.Infrastructure.Controls.ToastMessage
{
    public interface IMessage
    {
        void ShowSnackBarNetwork(string message, bool IsSuccess = false);
        void ShowToast(string message);
        void ShowSnackBar(string message);
        void ShowSnackBarWithAction(string message, Action<object> action, string actionText = "Ok", string actionTextColor = "#F48120");
    }
}
