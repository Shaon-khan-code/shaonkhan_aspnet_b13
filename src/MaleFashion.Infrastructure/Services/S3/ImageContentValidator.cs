using SixLabors.ImageSharp;

namespace MaleFashion.Infrastructure.Services.S3;

public static class ImageContentValidator
{
    public static bool IsValidImage(Stream stream)
    {
        try
        {
            var info = Image.Identify(stream);
            return info != null;
        }
        catch
        {
            return false;
        }
        finally
        {
            stream.Position = 0;
        }
    }
}

