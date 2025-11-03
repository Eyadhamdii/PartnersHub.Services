using PartnersHub.InfraBase.Domain.Common;

namespace PartnersHub.InfraBase.Domain.ValueObjects;

/// <summary>
/// Value object representing attachment metadata
/// </summary>
public class AttachmentMetadata : ValueObject {
    public string FileName { get; private set; }
    public string FileExtension { get; private set; }
    public long FileSizeInBytes { get; private set; }
    public string ContentType { get; private set; }

    private AttachmentMetadata() {
        FileName = string.Empty;
        FileExtension = string.Empty;
        ContentType = string.Empty;
    }

    private AttachmentMetadata(string fileName, string fileExtension, long fileSizeInBytes, string contentType) {
        FileName = fileName;
        FileExtension = fileExtension;
        FileSizeInBytes = fileSizeInBytes;
        ContentType = contentType;
    }

    public static Result<AttachmentMetadata> Create(string fileName, long fileSizeInBytes, string contentType) {
        if (string.IsNullOrWhiteSpace(fileName)) {
            return Result<AttachmentMetadata>.Failure("File name is required");
        }

        if (fileName.Length > 255) {
            return Result<AttachmentMetadata>.Failure("File name cannot exceed 255 characters");
        }

        var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(fileExtension)) {
            return Result<AttachmentMetadata>.Failure("File must have an extension");
        }

        // Validate allowed extensions
        var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".xls", ".jpg", ".jpeg", ".png" };
        if (!allowedExtensions.Contains(fileExtension)) {
            return Result<AttachmentMetadata>.Failure("Only PDF, Docx, excel sheets, JPG, PNG and JPEG are allowed");
        }

        // Validate file size (20 MB = 20,971,520 bytes)
        const long maxSizeInBytes = 20 * 1024 * 1024;
        if (fileSizeInBytes <= 0) {
            return Result<AttachmentMetadata>.Failure("File size must be greater than 0");
        }

        if (fileSizeInBytes > maxSizeInBytes) {
            return Result<AttachmentMetadata>.Failure("Max size allowed for attachments is 20 MB");
        }

        if (string.IsNullOrWhiteSpace(contentType)) {
            return Result<AttachmentMetadata>.Failure("Content type is required");
        }

        var metadata = new AttachmentMetadata(fileName, fileExtension, fileSizeInBytes, contentType);
        return Result<AttachmentMetadata>.Success(metadata);
    }

    public string GetFileSizeFormatted() {
        const int kilobyte = 1024;
        const int megabyte = kilobyte * 1024;

        if (FileSizeInBytes >= megabyte) {
            return $"{FileSizeInBytes / (double)megabyte:F2} MB";
        }
        if (FileSizeInBytes >= kilobyte) {
            return $"{FileSizeInBytes / (double)kilobyte:F2} KB";
        }
        return $"{FileSizeInBytes} bytes";
    }

    protected override IEnumerable<object?> GetEqualityComponents() {
        yield return FileName;
        yield return FileExtension;
        yield return FileSizeInBytes;
        yield return ContentType;
    }
}
