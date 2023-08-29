namespace Domain.Models.Application;

public class ExportFileResponseModel
{
    public byte[]? Bytes { get; set; }

    public ExportFileResponseModel(byte[]? bytes, string? mimeType, string? fileName)
    {
        Bytes = bytes;
        MimeType = mimeType;
        FileName = fileName;
    }

    public string? MimeType { get; set; }
    public string? FileName { get; set; }
}
