using Pronia.Utilities;

namespace Pronia.Extensions;

public static class FileHelper
{
    public static bool ValidateFileType(this IFormFile file , FileTypeEnum type = FileTypeEnum.Image)
        => type switch
        {
            FileTypeEnum.Image => file.ContentType.Contains("image"),
            FileTypeEnum.Video => file.ContentType.Contains("video"),
            FileTypeEnum.Audio => file.ContentType.Contains("audio"),
            FileTypeEnum.Document => file.ContentType.Contains("application") || file.ContentType.Contains("text"),
            _ =>  false
        };

    public static bool ValidateFileSize(this IFormFile file, long maxSize,FileSizeEnum size)
        => size switch
        {
            FileSizeEnum.Kb => file.Length <= maxSize * 1024,
            FileSizeEnum.Mb => file.Length <= maxSize * 1024 * 1024,
            FileSizeEnum.Gb => file.Length <= maxSize * 1024 * 1024 * 1024,
            _ => false,
        };

    public static async Task<string> CreateFileAsync(this IFormFile file, string rootPath, params string[] directories)
    {
        string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

        FileStream fileStream = new FileStream(fileName.GetPath(rootPath, directories), FileMode.Create);
        
            await file.CopyToAsync(fileStream);
            fileStream.Close();
        
        
        return fileName;
    }

    public static void DeleteFile(this string fileName, string rootPath, params string[] directories)
    {
        string path = fileName.GetPath(rootPath, directories);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
    
    private static string GetPath(this string fileName,string rootPath, params string[] directories)
    {
        string path = rootPath;

        foreach (var directory in directories)
        {
            path = Path.Combine(path, directory);
        }
        return Path.Combine(path, fileName);;
    }
    
}