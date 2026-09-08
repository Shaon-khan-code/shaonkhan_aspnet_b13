using Amazon.S3;
using Amazon.S3.Model;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MaleFashion.Infrastructure.Services.S3;

public class S3FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly AwsOptions _options;
    private readonly ILogger<S3FileStorageService> _logger;

    public S3FileStorageService(IAmazonS3 s3Client, IOptions<AwsOptions> options, ILogger<S3FileStorageService> logger)
    {
        _s3Client = s3Client;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(
        Stream fileStream, string fileName, string contentType, string folder, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.BucketName))
        {
            throw new InvalidOperationException(
                "AWS S3 is not configured (Aws:BucketName missing). File upload cannot proceed.");
        }

        var sanitizedFileName = Path.GetFileName(fileName);
        var key = $"{folder.Trim('/')}/{Guid.NewGuid()}-{sanitizedFileName}";

        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType
        };

        try
        {
            await _s3Client.PutObjectAsync(request, cancellationToken);
            return key;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file to S3. Key: {Key}", key);
            throw;
        }
    }

    public async Task DeleteFileAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        try
        {
            await _s3Client.DeleteObjectAsync(_options.BucketName, key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file from S3. Key: {Key}", key);
            throw;
        }
    }

    public string GetFileUrl(string key)
    {
        if (!string.IsNullOrWhiteSpace(_options.CloudFrontUrl))
        {
            return $"{_options.CloudFrontUrl.TrimEnd('/')}/{key}";
        }

        return $"https://{_options.BucketName}.s3.{_options.Region}.amazonaws.com/{key}";
    }
}