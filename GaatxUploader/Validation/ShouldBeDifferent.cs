using System.ComponentModel.DataAnnotations;
using System.Data;

namespace GaatxUploader.Validation;

public sealed class ShouldBeDifferent : ValidationAttribute
{
    public ShouldBeDifferent(string otherObject)
    {
        OtherObject = otherObject;
    }

    public string OtherObject { get; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        object
            instance = validationContext.ObjectInstance,
            otherObject = instance.GetType().GetProperty(OtherObject)?.GetValue(instance);
        
        if (value is not DataColumn column) return new("No es una DataColumn");
        if (otherObject is not DataColumn otherColumn) return new("No es una DataColumn");
        
        if (column.ColumnName == otherColumn.ColumnName) return new ValidationResult("Se deben seleccionar columnas diferentes.");
        return ValidationResult.Success;
    }
}
    