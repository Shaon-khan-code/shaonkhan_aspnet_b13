namespace MaleFashion.Application.Contracts.Services;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folder, CancellationToken cancellationToken = default);

    Task DeleteFileAsync(string key, CancellationToken cancellationToken = default);

    string GetFileUrl(string key);
}