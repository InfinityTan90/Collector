using Collection.Global_var;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Collection.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Settings : Page
{
    public Settings()
    {
        InitializeComponent();
    }

    private void AppTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedTheme = AppThemeSelection.SelectedIndex;
        ElementTheme AppTheme = selectedTheme == 1 ? ElementTheme.Dark : ElementTheme.Light;
        TitleBarTheme TitleTheme = selectedTheme == 1 ? TitleBarTheme.Dark : TitleBarTheme.Light;

        if (HInstance.Instance.Content is FrameworkElement RootElement && HInstance.Theme != selectedTheme)
        { 
            RootElement.RequestedTheme = AppTheme; 
            HInstance.UpdateTitleBarTheme(TitleTheme);
            HInstance.Theme = selectedTheme;
        }
    }
}
