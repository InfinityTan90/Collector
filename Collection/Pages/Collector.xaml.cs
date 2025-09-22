using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Collection.Model;
using Microsoft.UI.Xaml.Automation.Peers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Collection.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Collector : Page
{
    public bool IsPlayerWorking
    {
        set
        {
            Play.IsEnabled = value == true ? true : false;
            Play.Content = "Suspend";
        }
    }

    public Collector()
    {
        InitializeComponent();
        VoiceTable.ItemsSource = Data.AuList_Filtered;
        Collection.Global_var.CollectorPageInstance.Instance = this;
    }

    public void LoadData(List<string> path)
    {
        foreach (string item in path)
        {
            var name = Path.GetFileNameWithoutExtension(item);
            Data.AuList.Add(new Items(name, item));
        }
    }

    private async void FindSource(object sender, RoutedEventArgs e)
    {
        FilterText.Text = string.Empty;

        ProcessStatusDisplay(true);

        var FolderPicker = new Windows.Storage.Pickers.FolderPicker();

        var Window = Global_var.HInstance.Instance;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(Window);
        WinRT.Interop.InitializeWithWindow.Initialize(FolderPicker, hWnd);


        FolderPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
        FolderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
        FolderPicker.FileTypeFilter.Add(".wav");

        var file = await FolderPicker.PickSingleFolderAsync();
        if (file == null)
        {
            ProcessStatusDisplay(false);
            return;
            
        }
        

        Data.AuList.Clear();

        Data.Directory = file.Path;

        var Paths = await GetFilesAsync(file.Path);

        if(Paths.Count == 0)
        {
            Collection.Global_var.HInstance.MessagePopup("Error", "None of a file vaild", InfoBarSeverity.Warning);
            ProcessStatusDisplay(false);
            return;
        }
        LoadData(Paths);

        ItemUpdate();

        Collection.Global_var.HInstance.MessagePopup("Success", $"{Data.AuList_Filtered.Count} file"+ (Data.AuList_Filtered.Count > 1 ? "s":"") + " searched", InfoBarSeverity.Success);

        ProcessStatusDisplay(false);
    }

    public async Task<List<string>> GetFilesAsync(string directoryPath)
    {
        return await Task.Run(() =>
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Directory is not exist: {directoryPath}");
            }
            return Directory.GetFiles(directoryPath ,"*.wav").ToList();
        });
    }

    public bool Filter(string FilterText, ObservableCollection<Items> Collection ,bool Refresh = false)
    {
        if(Refresh)
            Data.AuList_Filtered.Clear();

        if(FilterText.Length == 0)
        {
            if (Data.AuList_Filtered.Count == Data.AuList.Count)
                return false;
            Data.AuList_Filtered.Clear();
            foreach (var item in Collection)
            {
                Data.AuList_Filtered.Add(item);
            }
            return true;
        }

        Data.AuList_Filtered.Clear();

        switch (MatchMode.SelectedIndex)
        {
            case 0:
                foreach (var item in Collection)
                {
                    if (item.Name.StartsWith(FilterText))
                    {
                        Data.AuList_Filtered.Add(item);
                    }
                }
                    
                break;
    
            case 1:
                foreach (var item in Collection)
                {
                    if (item.Name.EndsWith(FilterText))
                    {
                        Data.AuList_Filtered.Add(item);
                    }
                }
                break;
    
            case 2:
                foreach (var item in Collection)
                {
                    if (item.Name.Contains(FilterText))
                    {
                        Data.AuList_Filtered.Add(item);
                    }
                }
                break;
    
            case 3:
                foreach (var item in Collection)
                {
                    if (item.Name == FilterText)
                    {
                        Data.AuList_Filtered.Add(item);
                    }
                }
                break;
        }
        return true;
    }

    private void StopPlaying(object sender, RoutedEventArgs e)
    {
        Player.Stop();
    }
    private async void Refresh(object sender, RoutedEventArgs e)
    {
        Data.AuList.Clear();
        var Paths = await GetFilesAsync(Data.Directory);

        LoadData(Paths);

        Filter(string.Empty, Data.AuList, true);
    }
    private void Clear(object sender, RoutedEventArgs e)
    {
        Data.AuList.Clear();
        Data.AuList_Filtered.Clear();
        Data.Directory = string.Empty;
        Player.stop();
        Nums.Text = string.Empty;
    }
   
    private void VoiceTab_btnClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Items item)
        {
            int index = Data.AuList.IndexOf(item);

            FileName.Text = Data.AuList[index].Name;
            FilePath.Text = Data.AuList[index].Path;

            Player.Play(item.Path, (float)Volum.Value / 100);
        }
    }
    private void ProcessStatusDisplay(bool IsWorking)
    {
        ProcessStatus.IsIndeterminate = IsWorking;
        ProcessStatus.Visibility = IsWorking ? Visibility.Visible : Visibility.Collapsed;
    }

    private void Inquired(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        ItemUpdate();
    }

    private void FilterText_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        ItemUpdate();
    }

    private void MatchModeChanged(object sender, SelectionChangedEventArgs e)
    {
        ItemUpdate();
    }

    private void ItemUpdate()
    {
        if (Filter(FilterText.Text, Data.AuList))
        {
            FileName.Text = string.Empty;
            FilePath.Text = string.Empty;
            Nums.Text = Data.AuList_Filtered.Count.ToString();
        }
    }
    private void VolumChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        Player.SetVolume((float)Volum.Value);
    }

    private void SuspendOrContinue(object sender, RoutedEventArgs e)
    {
        if (Play.Content == "Suspend")
        { 
            Player.Suspend();
            Play.Content = "Continue";
        }
        else
        { 
            Player.Continue();
            Play.Content = "Suspend";
        }
    }
}
public class Items
{
    public string Name { get; set; }
    public string Path {  get; set; }
        
    public Items(string name,string path)
    {
        Path = path;
        if (path == null)
            Path = "Not Exist";
        Name = name;
        if (name == null)
            Name = "Unknow";
    }
}

public class WidthSubtractConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double width)
        {
            double subtractValue = 52;

            if (parameter != null && double.TryParse(parameter.ToString(), out double customValue))
            {
                subtractValue = customValue;
            }

            double result = width - subtractValue;
            return result > 0 ? result : 0; 
        }

        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}