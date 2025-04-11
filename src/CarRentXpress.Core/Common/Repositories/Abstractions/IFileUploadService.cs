namespace CarRentXpress.Core.Repositories;

public interface IFileUploadService
{
    Task<string> UploadFileAsync(Stream fileStream, string destinationPath);
}