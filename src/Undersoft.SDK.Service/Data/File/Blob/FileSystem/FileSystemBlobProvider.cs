using System;
using System.IO;
using System.Threading.Tasks;
using Polly;

namespace Undersoft.SDK.Service.Data.File.Blob.FileSystem;

public class FileSystemBlobProvider : BlobProviderBase
{
    protected IBlobFilePathCalculator FilePathCalculator { get; }

    public FileSystemBlobProvider(IBlobFilePathCalculator filePathCalculator)
    {
        FilePathCalculator = filePathCalculator;
    }

    public override async Task SaveAsync(BlobProviderSaveArgs args)
    {
        var filePath = FilePathCalculator.Calculate(args);

        if (!args.OverrideExisting && await ExistsAsync(filePath))
        {
            throw new BlobAlreadyExistsException($"Saving BLOB '{args.BlobName}' does already exists in the container '{args.ContainerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
        }

        var dirname = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dirname))
            Directory.CreateDirectory(dirname);

        var fileMode = args.OverrideExisting
            ? FileMode.Create
            : FileMode.CreateNew;

        await Policy.Handle<IOException>()
            .WaitAndRetryAsync(2, retryCount => TimeSpan.FromSeconds(retryCount))
            .ExecuteAsync(async () =>
            {
                using (var fileStream = System.IO.File.Open(filePath, fileMode, FileAccess.Write))
                {
                    await args.BlobStream.CopyToAsync(
                        fileStream,
                        args.CancellationToken
                    );

                    await fileStream.FlushAsync();
                }
            });
    }

    public override Task<bool> DeleteAsync(BlobProviderArgs args)
    {
        var filePath = FilePathCalculator.Calculate(args);
        return Task.Run<bool>(() =>
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                return true;
            }
            return false;
        });
    }

    public override Task<bool> ExistsAsync(BlobProviderArgs args)
    {
        var filePath = FilePathCalculator.Calculate(args);
        return ExistsAsync(filePath);
    }

    public override async Task<Stream> GetOrNullAsync(BlobProviderArgs args)
    {
        var filePath = FilePathCalculator.Calculate(args);

        if (!System.IO.File.Exists(filePath))
        {
            return null;
        }

        return await Policy.Handle<IOException>()
            .WaitAndRetryAsync(2, retryCount => TimeSpan.FromSeconds(retryCount))
            .ExecuteAsync(async () =>
            {
                using (var fileStream = System.IO.File.OpenRead(filePath))
                {
                    return await TryCopyToMemoryStreamAsync(fileStream, args.CancellationToken);
                }
            });
    }

    protected virtual Task<bool> ExistsAsync(string filePath)
    {
        return Task.FromResult(System.IO.File.Exists(filePath));
    }
}