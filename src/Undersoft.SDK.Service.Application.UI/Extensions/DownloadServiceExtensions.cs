using System.IO.Compression;

namespace Undersoft.SDK.Service.Application.Components;

public static class DownloadServiceExtensions
{
    public static Task DownloadFromStreamAsync(this DownloadService download, string downloadFileName, Stream stream) => download.DownloadFromStreamAsync(new DownloadOption() { FileName = downloadFileName, FileStream = stream });

    public static Task DownloadFromByteArrayAsync(this DownloadService download, string downloadFileName, byte[] data) => download.DownloadFromStreamAsync(new DownloadOption() { FileName = downloadFileName, FileStream = new MemoryStream(data) });

    public static async Task DownloadFolderAsync(this DownloadService download, string downloadFileName, string folder)
    {
        if (!Directory.Exists(folder))
        {
            throw new DirectoryNotFoundException($"Couldn't be not found {folder}");
        }

        var directoryName = folder.TrimEnd('\\', '\r', '\n');
        var destZipFile = $"{directoryName}.zip";
        ZipFile.CreateFromDirectory(folder, destZipFile);

        using var stream = new FileStream(destZipFile, FileMode.Open);
        await download.DownloadFromStreamAsync(new DownloadOption() { FileName = downloadFileName, FileStream = stream });
    }

    public static Task DownloadFromUrlAsync(this DownloadService download, string downloadFileName, string url) => download.DownloadFromUrlAsync(new DownloadOption() { FileName = downloadFileName, Url = url });

    public static async Task DownloadFromFileAsync(this DownloadService download, string fileName, string physicalFilePath)
    {
        if (File.Exists(physicalFilePath))
        {
            using var stream = File.OpenRead(physicalFilePath);
            await download.DownloadFromStreamAsync(fileName, stream);
        }
    }
}
