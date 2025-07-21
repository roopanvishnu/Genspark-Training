using Azure.Storage.Blobs;
using Azure;
using System;

namespace TrainingVideoAPI.Services;

public class BlobService
{
    private readonly BlobContainerClient _containerClient;

    public BlobService(IConfiguration config)
    {
        var connStr = config["AzureBlobStorage:ConnectionString"];
        var container = config["AzureBlobStorage:ContainerName"];
        _containerClient = new BlobContainerClient(connStr, container);
        _containerClient.CreateIfNotExists();
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        var blobClient = _containerClient.GetBlobClient($"{Guid.NewGuid()}_{file.FileName}");
        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, true);
        return blobClient.Uri.ToString();
    }

    public async Task<bool> DeleteAsync(string blobUrl)
    {
        try
        {
            // Extract blob name from the URL
            var uri = new Uri(blobUrl);
            var blobName = Path.GetFileName(uri.LocalPath);

            var blobClient = _containerClient.GetBlobClient(blobName);
            var result = await blobClient.DeleteIfExistsAsync();

            return result.Value;
        }
        catch
        {
            return false;
        }
    }
}
