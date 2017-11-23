using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WotStatsTool.Services
{
    public interface INotificationService
    {
        //TODO: stop relying on MessageBox enums?
        MessageBoxResult Notify(string text, string caption, MessageBoxButton buttons);
    }
}
