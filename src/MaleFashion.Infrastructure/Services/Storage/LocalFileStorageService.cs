using System.Text;
using MaleFashion.Application.Contracts.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace MaleFashion.Infrastructure.Services.Storage;


public class LocalFileStorageService : IFileStorageService
{
    private const string RequestPath = "/uploads";

    private readonly string _rootPath;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(IWebHostEnvironment environment, ILogger<LocalFileStorageService> logger)
    {
        var webRoot = string.IsNullOrWhiteSpace(environment.WebRootPath)
            ? Path.Combine(environment.ContentRootPath, "wwwroot")
            : environment.WebRootPath;

        _rootPath = Path.GetFullPath(Path.Combine(webRoot, "uploads"));
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(
        Stream fileStream, string fileName, string contentType, string folder, CancellationToken cancellationToken = default)
    {
        if (fileStream.CanSeek)
        {
            fileStream.Position = 0;
        }

        if (!LooksLikeImage(fileStream))
        {
            throw new InvalidOperationException("The uploaded file is not a valid image.");
        }

        var safeName = Path.GetFileName(fileName);
        var key = $"{folder.Trim('/')}/{Guid.NewGuid():N}-{safeName}";
        var fullPath = ResolvePath(key);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var target = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await fileStream.CopyToAsync(target, cancellationToken);

        _logger.LogInformation("Saved image locally. Key: {Key}", key);
        return key;
    }

    public Task DeleteFileAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key) || !IsUploadedKey(key))
        {
            return Task.CompletedTask;
        }

        try
        {
            var path = ResolvePath(key);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not delete local file. Key: {Key}", key);
        }

        return Task.CompletedTask;
    }

    public string GetFileUrl(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return string.Empty;
        }

        key = key.Trim();

        if (Uri.TryCreate(key, UriKind.Absolute, out var absolute) &&
            (absolute.Scheme == Uri.UriSchemeHttp || absolute.Scheme == Uri.UriSchemeHttps))
        {
            return key;
        }

        
        if (key.StartsWith("img/", StringComparison.OrdinalIgnoreCase))
        {
            return "/storefront/" + key;
        }

        if (key.StartsWith("storefront/", StringComparison.OrdinalIgnoreCase))
        {
            return "/" + key;
        }

        if (key.StartsWith('/'))
        {
            return key;
        }

        var encoded = string.Join("/", key.Split('/', StringSplitOptions.RemoveEmptyEntries).Select(Uri.EscapeDataString));
        return $"{RequestPath}/{encoded}";
    }

   
    private static bool IsUploadedKey(string key)
        => !key.StartsWith("img/", StringComparison.OrdinalIgnoreCase)
           && !key.StartsWith("storefront/", StringComparison.OrdinalIgnoreCase)
           && !key.StartsWith('/')
           && !key.StartsWith("http", StringComparison.OrdinalIgnoreCase);

    
    private string ResolvePath(string key)
    {
        var relative = key.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
        var full = Path.GetFullPath(Path.Combine(_rootPath, relative));

        if (!full.StartsWith(_rootPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Invalid file path.");
        }

        return full;
    }

    
    private static bool LooksLikeImage(Stream stream)
    {
        if (!stream.CanSeek)
        {
            return true;
        }

        var header = new byte[12];
        var read = stream.Read(header, 0, header.Length);
        stream.Position = 0;

        if (read < 4)
        {
            return false;
        }

        var jpeg = header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
        var png = read >= 8 && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47
                  && header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A;
        var gif = header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38;
        var webp = read >= 12
                   && Encoding.ASCII.GetString(header, 0, 4) == "RIFF"
                   && Encoding.ASCII.GetString(header, 8, 4) == "WEBP";

        return jpeg || png || gif || webp;
    }
}
