using GeoFluent.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;

namespace GeoFluent.Views;

public sealed partial class HistoryPage : Page
{
    public HistoryPage()
    {
        InitializeComponent();
        Loaded += (s, e) => RefreshList();
    }

    private void RefreshList()
    {
        HistoryItemsList.ItemsSource = HistoryService.Entries;
        EmptyCard.Visibility = HistoryService.Entries.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void BtnRestore_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string id)
        {
            var entry = HistoryService.Entries.FirstOrDefault(x => x.Id == id);
            if (entry != null)
            {
                HistoryService.PendingRestore = entry;
                if (entry.Category == "2D")
                {
                    Frame.Navigate(typeof(TwoDimensionalPage));
                }
                else
                {
                    Frame.Navigate(typeof(ThreeDimensionalPage));
                }
            }
        }
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string id)
        {
            HistoryService.Remove(id);
            RefreshList();
            ShowToast("Calculation removed from history.");
        }
    }

    private void BtnClear_Click(object sender, RoutedEventArgs e)
    {
        HistoryService.Clear();
        RefreshList();
        ShowToast("Calculation history cleared.");
    }

    private void BtnExport_Click(object sender, RoutedEventArgs e)
    {
        if (HistoryService.Entries.Count == 0)
        {
            ShowToast("No calculations to export.");
            return;
        }

        string md = HistoryService.ExportAllToMarkdown();
        var pkg = new DataPackage();
        pkg.SetText(md);
        Clipboard.SetContent(pkg);
        ShowToast($"Exported {HistoryService.Entries.Count} calculations to Markdown (copied to clipboard)!");
    }

    private void ShowToast(string message)
    {
        ToastBar.Message = message;
        ToastBar.IsOpen = true;
    }
}
