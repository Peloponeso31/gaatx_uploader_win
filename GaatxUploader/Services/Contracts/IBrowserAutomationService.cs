using System.Data;

namespace GaatxUploader.Services.Contracts;

public interface IBrowserAutomationService
{
    void UploadDataTable(DataTable calificaciones, string username, string password, string grupo, string criterio, string evaluacion);
}