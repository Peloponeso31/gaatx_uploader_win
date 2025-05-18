using System.Windows;
using System.Windows.Data;
using OfficeOpenXml;
using Wpf.Ui.Controls;

namespace GaatxUploader.Features.Settings.Presentation;

public partial class SettingsWindow : FluentWindow
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    private void PrimerasColumnas_OnFilter(object sender, FilterEventArgs e)
    {
        var item = e.Item as ExcelRange;
        var worksheet = item?.Worksheet;
        if (worksheet == null) return;
        
        var first = worksheet.Cells[1, worksheet.Dimension.Start.Column, 1, worksheet.Dimension.Start.Column];
        e.Accepted = first.Any(cell => Equals(item, cell));
    }
}