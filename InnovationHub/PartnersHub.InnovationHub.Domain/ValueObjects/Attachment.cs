using PartnersHub.InnovationHub.Domain.Common;
using PartnersHub.InnovationHub.Domain.Enums;

namespace PartnersHub.InnovationHub.Domain.ValueObjects;

public class Attachment 
{
  
    public string Name { get; private set; }
    public Format Format { get; private set; }
    public Extension Extension { get; private set; }
    public long SizeInBytes { get; private set; }
    public string Url { get; private set; }





    private Attachment() { }

    public Attachment(string name, Format format, Extension extension, long sizeInBytes, string url)
    {
        Name = name;
        Format = format;
        Extension = extension;
        SizeInBytes = sizeInBytes;
        Url = url;
    }

    public static Result<Attachment> Create(string fileName, long fileSizeInBytes, Format contentType , string url)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Result<Attachment>.Failure("File name is required");
        }

        if (fileName.Length > 255)
        {
            return Result<Attachment>.Failure("File name cannot exceed 255 characters");
        }

        var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(fileExtension))
        {
            return Result<Attachment>.Failure("File must have an extension");
        }
        var extension = GetExtension(fileExtension);
        // Validate allowed extensions
        var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".xls", ".jpg", ".jpeg", ".png" };
        if (!allowedExtensions.Contains(fileExtension))
        {
            return Result<Attachment>.Failure("Only PDF, Docx, excel sheets, JPG, PNG and JPEG are allowed");
        }

        // Validate file size (20 MB = 20,971,520 bytes)
        const long maxSizeInBytes = 20 * 1024 * 1024;
        if (fileSizeInBytes <= 0)
        {
            return Result<Attachment>.Failure("File size must be greater than 0");
        }

        if (fileSizeInBytes > maxSizeInBytes)
        {
            return Result<Attachment>.Failure("Max size allowed for attachments is 20 MB");
        }

        //if (string.IsNullOrWhiteSpace(contentType))
        //{
        //    return Result<Attachment>.Failure("Content type is required");
        //}

        var metadata = new Attachment(fileName, contentType, extension, fileSizeInBytes, url);
        return Result<Attachment>.Success(metadata);
    }

    public string GetFileSizeFormatted()
    {
        const int kilobyte = 1024;
        const int megabyte = kilobyte * 1024;

        if (SizeInBytes >= megabyte)
        {
            return $"{SizeInBytes / (double)megabyte:F2} MB";
        }
        if (SizeInBytes >= kilobyte)
        {
            return $"{SizeInBytes / (double)kilobyte:F2} KB";
        }
        return $"{SizeInBytes} bytes";
    }

    private static Extension GetExtension(string fileExtension)
    {
        fileExtension = fileExtension.ToLowerInvariant().Trim('.');

        return fileExtension switch
        {
            "pdf" => Extension.PDF,
            "docx" => Extension.DOCX,
            "xlsx" => Extension.XLSX,
            "xls" => Extension.XLS,
            "jpg" or "jpeg" => Extension.JPEG,
            "png" => Extension.PNG,
            _ => throw new ArgumentException("Invalid file extension"),
        };
    }
}
