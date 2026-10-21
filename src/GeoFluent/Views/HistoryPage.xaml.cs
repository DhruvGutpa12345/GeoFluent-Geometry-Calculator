using GeoFluent.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GeoFluent.Views;

public sealed partial class HistoryPage : Page
{
    public HistoryPage()
    {
        InitializeComponent();
        Loaded += (s, e) => RefreshView();
    }

    private void RefreshView()
    {
        HistoryListView.ItemsSource = HistoryService.Entries;
        TxtEmptyState.Visibility = HistoryService.Entries.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void BtnClear_Click(object sender, RoutedEventArgs e)
    {
        HistoryService.Clear();
        RefreshView();
    }
}
