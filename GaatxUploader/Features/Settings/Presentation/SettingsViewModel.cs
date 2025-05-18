using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using Microsoft.Win32;
using OfficeOpenXml;

namespace GaatxUploader.Features.Settings.Presentation;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty] private string? _filename;
    [ObservableProperty] private ExcelPackage _spreadsheet;
    [ObservableProperty] private bool _excelCargado;
    [ObservableProperty] private bool _isSheetSelected;
    [ObservableProperty] private ExcelWorksheet _selectedWorksheet;
    [ObservableProperty] private DataTable _dataTable;

    [RelayCommand]
    void CargarExcel()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Excel Files (*.xlsx)|*.xlsx",
            Title = "Seleccione el archivo de excel donde tiene las calificaciones"
        };

        if (openFileDialog.ShowDialog() ==  true)
        {
            Filename = openFileDialog.FileName;
            Spreadsheet = new ExcelPackage(openFileDialog.FileName);
        }
    }

    partial void OnFilenameChanged(string? value)
    {
        ExcelCargado = value != null;
    }

    partial void OnSelectedWorksheetChanged(ExcelWorksheet value)
    {
        if (value == null) return;
        IsSheetSelected = true;
        WorksheetToDataTable(value);
    }
    
    public void WorksheetToDataTable(ExcelWorksheet worksheet)
    {
        var table = new DataTable();

        // Assume first row has headers
        for (var col = 1; col <= worksheet.Dimension.End.Column; col++)
            table.Columns.Add(worksheet.Cells[1, col].Text);

        for (var row = 2; row <= worksheet.Dimension.End.Row; row++)
        {
            var dataRow = table.NewRow();
            for (var col = 1; col <= worksheet.Dimension.End.Column; col++)
                dataRow[col - 1] = worksheet.Cells[row, col].Text;

            table.Rows.Add(dataRow);
        }

        DataTable = table;
    }
}