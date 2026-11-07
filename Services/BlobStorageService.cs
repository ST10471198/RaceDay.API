using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.API.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }

    public class BlobStorageService : IBlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobStorageService(IConfiguration configuration)
        {
            // Read from the new nested structure using colon ':'
            _connectionString = configuration["AzureStorage:ConnectionString"]!;

            // Read the container name
            _containerName = configuration["AzureStorage:ContainerName"] ?? "raceday-images";
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new Exception("Azure Storage connection string is missing!");
            }

            // 1. Create a client to talk to Azure Blob Storage
            var blobServiceClient = new BlobServiceClient(_connectionString);

            // 2. Get (or create) the container using the variable name
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // 3. Generate a unique file name to prevent overwriting
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = containerClient.GetBlobClient(fileName);

            // 4. Upload the file stream
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

            // 5. Return the public URL
            return blobClient.Uri.ToString();
        }
    }
}