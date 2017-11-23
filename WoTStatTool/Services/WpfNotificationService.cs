using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WotStatsTool.Services
{
    public class WpfNotificationService : INotificationService
    {
        public MessageBoxResult Notify(string text, string caption, MessageBoxButton buttons)
        {
            return MessageBox.Show(text, caption, buttons);
        }
    }
}
