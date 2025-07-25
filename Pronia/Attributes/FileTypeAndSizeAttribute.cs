using System.ComponentModel.DataAnnotations;
using Pronia.Extensions;
using Pronia.Utilities;
namespace Pronia.Models.Attributes;

public class FileTypeAndSizeAttribute : ValidationAttribute
{
    private readonly long _maxSize;
    private readonly FileSizeEnum _sizeType;
    private readonly FileTypeEnum _fileType;

    public FileTypeAndSizeAttribute(long maxSize, FileSizeEnum sizeType, FileTypeEnum fileType)
    {
        _maxSize = maxSize;
        _sizeType = sizeType;
        _fileType = fileType;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
          
            if (!file.ValidateFileType(_fileType))
                return new ValidationResult("Tip səhvdir!");

            if (!file.ValidateFileSize(_maxSize, _sizeType))
                return new ValidationResult("Ölçü səhvdir!");
        }

        if (value is IEnumerable<IFormFile> files)
        {
            foreach (var f in files)
            {
                if (!f.ValidateFileType(_fileType))
                    return new ValidationResult("Tip səhvdir!");

                if (!f.ValidateFileSize(_maxSize, _sizeType))
                    return new ValidationResult("Ölçü səhvdir!");
            }
            
        }
        return ValidationResult.Success; 
    }

}

