using SkiaSharp;

namespace ImageResize.Services;

public record ResizeResult(string FileName, bool Success, string? ErrorMessage = null);

public class ImageResizeService
{
    private static readonly string[] SupportedExtensions = [".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".tiff", ".tif"];

    public string ResolveOutputFolder(string sourceFolder)
    {
        string datePart = DateTime.Today.ToString("yyyyMMdd");
        string baseName = $"resized_{datePart}";
        string candidate = Path.Combine(sourceFolder, baseName);

        if (!Directory.Exists(candidate))
            return candidate;

        int index = 1;
        while (true)
        {
            candidate = Path.Combine(sourceFolder, $"{baseName}_{index}");
            if (!Directory.Exists(candidate))
                return candidate;
            index++;
        }
    }

    public IEnumerable<string> GetImageFiles(string folder) =>
        Directory.EnumerateFiles(folder)
                 .Where(f => SupportedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()));

    public async Task<List<ResizeResult>> ResizeImagesAsync(
        string sourceFolder,
        string outputFolder,
        int targetWidth,
        int targetHeight,
        bool keepAspectRatio = true,
        IProgress<string>? progress = null)
    {
        var results = new List<ResizeResult>();

        Directory.CreateDirectory(outputFolder);

        var files = GetImageFiles(sourceFolder).ToList();

        if (files.Count == 0)
        {
            return results;
        }

        foreach (var filePath in files)
        {
            string fileName = Path.GetFileName(filePath);
            progress?.Report(fileName);

            try
            {
                await Task.Run(() => ResizeSingleImage(filePath, outputFolder, targetWidth, targetHeight, keepAspectRatio));
                results.Add(new ResizeResult(fileName, true));
            }
            catch (Exception ex)
            {
                results.Add(new ResizeResult(fileName, false, ex.Message));
            }
        }

        return results;
    }

    private static void ResizeSingleImage(string sourcePath, string outputFolder, int targetWidth, int targetHeight, bool keepAspectRatio)
    {
        using var original = SKBitmap.Decode(sourcePath)
            ?? throw new InvalidOperationException("Unable to decode image.");

        (int newWidth, int newHeight) = keepAspectRatio
            ? CalculateDimensions(original.Width, original.Height, targetWidth, targetHeight)
            : (targetWidth, targetHeight);

        using var resized = original.Resize(new SKImageInfo(newWidth, newHeight), SKSamplingOptions.Default);
        if (resized is null)
            throw new InvalidOperationException("Resize operation failed.");

        using var image = SKImage.FromBitmap(resized);

        string ext = Path.GetExtension(sourcePath).ToLowerInvariant();
        SKEncodedImageFormat format = ext switch
        {
            ".png" => SKEncodedImageFormat.Png,
            ".gif" => SKEncodedImageFormat.Gif,
            ".webp" => SKEncodedImageFormat.Webp,
            _ => SKEncodedImageFormat.Jpeg
        };

        using var data = image.Encode(format, 95);
        string outputPath = Path.Combine(outputFolder, Path.GetFileName(sourcePath));
        using var stream = File.OpenWrite(outputPath);
        data.SaveTo(stream);
    }

    private static (int width, int height) CalculateDimensions(int srcWidth, int srcHeight, int targetWidth, int targetHeight)
    {
        float ratioX = (float)targetWidth / srcWidth;
        float ratioY = (float)targetHeight / srcHeight;
        float ratio = Math.Min(ratioX, ratioY);

        return ((int)Math.Round(srcWidth * ratio), (int)Math.Round(srcHeight * ratio));
    }
}
