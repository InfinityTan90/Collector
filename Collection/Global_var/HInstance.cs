using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection.Global_var
{
    static class HInstance
    {
        public static MainWindow Instance;
        public static void MessagePopup(string Title, string Message, InfoBarSeverity severity)
        {
            Instance.MessagePopup(Title, Message, severity);
        }

        public static void UpdateTitleBarTheme(TitleBarTheme Theme)
        {
            Instance.UpdateTitleBarTheme(Theme);
        }

        public static int Theme = 0;

        public static bool Isworking = false;
    }
}
