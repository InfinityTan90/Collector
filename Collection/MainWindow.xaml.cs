using Collection.Global_var;
using Collection.Model;
using Collection.Pages;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Collection
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(TitleBar_Main);
            Global_var.HInstance.Instance = this;

            Player.init();

            this.Closed += (_, _) =>
            {
                Player.Dispose();
            };
        }
        private string CurrentPage = "Collector";
        private void Navigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked && CurrentPage != "Settings")
            {
                // 设置项被点击
                RootFrame.Navigate(typeof(Settings), null, new EntranceNavigationTransitionInfo());
                CurrentPage = "Settings";
            }

            else if (args.InvokedItemContainer is NavigationViewItem item && item.Tag is string tag)
            {
                Type pageType = null;
                switch (tag)
                {
                    case "Collector": pageType = typeof(Collector); break;
                }
                if (pageType != null && CurrentPage != tag)
                {
                    RootFrame.Navigate(pageType, null, new EntranceNavigationTransitionInfo());
                    CurrentPage = tag;
                }
            }
        }

        public void UpdateTitleBarTheme(TitleBarTheme Theme)
        {

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            var appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(hWnd));

            // 这行代码是关键：让标题栏按钮跟随应用主题
            appWindow.TitleBar.PreferredTheme = Theme;

        }

        private void InfoBarClose(InfoBar sender, InfoBarClosingEventArgs args)
        {
            args.Cancel = true;

            ReturnBack.Begin();
        }

        enum AnimationsState
        {
            Moving = 1,
            Standby = 0
        }
        struct AnimationStateOfEach
        {
            public AnimationsState RaiseUp;
            public AnimationsState ReturnBack;
            public AnimationsState MessageChanging;
            public AnimationsState MessageChanged_Resume;

            public AnimationStateOfEach()
            {
                RaiseUp = AnimationsState.Standby;
                ReturnBack = AnimationsState.Standby;
                MessageChanging = AnimationsState.Standby;
                MessageChanged_Resume = AnimationsState.Standby;
            }

            public void Default()
            {
                RaiseUp = AnimationsState.Standby;
                ReturnBack = AnimationsState.Standby;
                MessageChanging = AnimationsState.Standby;
                MessageChanged_Resume = AnimationsState.Standby;
            }
        }
        struct MessagePop
        {
            public string Message;
            public string Title;
            public InfoBarSeverity Severity;
        };

        MessagePop MSG;
        AnimationStateOfEach EachAnimationsState;

        public void MessagePopup(string Title, string Message, InfoBarSeverity severity)
        {
            MSG.Message = Message;
            MSG.Title = Title;
            MSG.Severity = severity;
            if (EachAnimationsState.ReturnBack == AnimationsState.Moving ||
                EachAnimationsState.MessageChanging == AnimationsState.Moving)
            {
                EachAnimationsState.Default();
                EachAnimationsState.MessageChanging = AnimationsState.Moving;
                MessageChanging.Begin();
            }
            else
            {
                MessageAlert.Severity = severity;
                MessageAlert.Title = Title;
                MessageAlert.Message = Message;

                EachAnimationsState.Default();
                EachAnimationsState.RaiseUp = AnimationsState.Moving;
                RaiseUp.Begin();
            }
        }
        public void RaiseUp_Completed(object sender, object e)
        {
            EachAnimationsState.RaiseUp = AnimationsState.Standby;
            EachAnimationsState.ReturnBack = AnimationsState.Moving;
            ReturnBack_3secDelay.Begin();
        }

        public void MessageChangeTime(object sender, object e)
        {
            EachAnimationsState.MessageChanging = AnimationsState.Standby;
            EachAnimationsState.MessageChanged_Resume = AnimationsState.Moving;

            MessageAlert.Severity = MSG.Severity;
            MessageAlert.Title = MSG.Title;
            MessageAlert.Message = MSG.Message;

            MessageChanged_Resume.Begin();
        }

        public void MessageChangeing_Resume_Complete(object sender, object e)
        {
            EachAnimationsState.MessageChanged_Resume = AnimationsState.Standby;
            EachAnimationsState.ReturnBack = AnimationsState.Moving;
            ReturnBack_3secDelay.Begin();
        }

        public void ReturnBack_Completed(object sender, object e)
        {
            EachAnimationsState.ReturnBack = AnimationsState.Standby;
        }
    }
}

