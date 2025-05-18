using System.ComponentModel.DataAnnotations;
using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GaatxUploader.Services.Contracts;
using GaatxUploader.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using OfficeOpenXml;

namespace GaatxUploader.Features.Settings.Presentation;

public partial class SettingsViewModel : ObservableValidator
{
    private readonly IBrowserAutomationService _browserAutomationService = App.Current.Services.GetService<IBrowserAutomationService>()!; 
    
    
    [ObservableProperty]
    [Required]
    private string _numeroDocente;
    
    [ObservableProperty]
    [Required]
    private string _password;
    
    [ObservableProperty]
    [Required]
    private string _grupo;
    
    [ObservableProperty]
    [Required]
    private string _criterio;
    
    [ObservableProperty]
    [Required]
    private string _evaluacion;
    
    [ObservableProperty]
    [Required]
    private ExcelWorksheet _selectedWorksheet;
    
    [ObservableProperty]
    [Required]
    [ShouldBeDifferent(nameof(CalificacionesColumn))]
    private DataColumn _controlColumn;
    
    [ObservableProperty]
    [Required]
    [ShouldBeDifferent(nameof(ControlColumn))]
    private DataColumn _calificacionesColumn;
    
    [ObservableProperty] private string? _filename;
    [ObservableProperty] private ExcelPackage _spreadsheet;
    [ObservableProperty] private bool _excelCargado;
    [ObservableProperty] private bool _isSheetSelected;
    [ObservableProperty] private DataTable _dataTable;

    [RelayCommand]
    void CargarExcel()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Archivos de Excel (*.xlsx)|*.xlsx",
            Title = "Seleccione el archivo de Excel donde tiene las calificaciones"
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

    [RelayCommand]
    void Upload()
    {
        ClearErrors();
        ValidateAllProperties();
        
        Console.WriteLine("---------");
        foreach (var error in GetErrors())
        {
            Console.WriteLine(error.ErrorMessage);
        }

        if (HasErrors) return;
        var calificaciones = DataTable.DefaultView.ToTable(false, [ControlColumn.ColumnName, CalificacionesColumn.ColumnName]);
        _browserAutomationService.UploadDataTable(calificaciones, NumeroDocente, Password, Grupo, Criterio, Evaluacion);
    }
}