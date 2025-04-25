using Minio.DataModel.Args;
using Minio.DataModel.Tags;
using Minio;
using Project_2.Models.Constraints;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minio.ApiEndpoints;
using System.Reactive.Linq;

namespace Project_2.Services.Storage
{
    public class MinioService
    {
        private readonly IMinioClient _minioClient; // Use IMinioClient interface

        public MinioService()
        {
            _minioClient = new MinioClient()
                .WithEndpoint(Constraints.MinioEndpoint)
                .WithCredentials(Constraints.MinioAccessKey, Constraints.MinioSecretKey)
                .WithSSL(false)
                .Build();
        }


        public async Task InitializeBucketAsync()
        {
            var bucketExists = await _minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(Constraints.MinioBucketName));

            if (!bucketExists)
            {
                await _minioClient.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(Constraints.MinioBucketName));
            }
        }

        public async Task UploadObjectAsync(string bucketName, string objectName, Stream data,
            Dictionary<string, string> tags = null)
        {
            await InitializeBucketAsync();

            // Reset stream position
            if (data.CanSeek)
                data.Position = 0;

            // Create args for upload
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(data)
                .WithObjectSize(data.Length)
                .WithContentType("text/tab-separated-values");

            // Add tags if provided
            if (tags != null && tags.Count > 0)
                putObjectArgs = putObjectArgs.WithTagging(new Tagging(tags, true));

            await _minioClient.PutObjectAsync(putObjectArgs);
        }

        public async Task<Stream> DownloadObjectAsync(string bucketName, string objectName)
        {
            var memoryStream = new MemoryStream();

            var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream =>
                {
                    stream.CopyTo(memoryStream);
                });

            await _minioClient.GetObjectAsync(args);

            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<List<string>> ListObjectsAsync(string bucketName, string prefix = null)
        {
            var result = new List<string>();

            var listArgs = new ListObjectsArgs()
                .WithBucket(bucketName);

            if (!string.IsNullOrEmpty(prefix))
                listArgs = listArgs.WithPrefix(prefix);

            // Method 1: Using ToList() on the Observable
            var items = await _minioClient.ListObjectsAsync(listArgs)
                .ToList() // This converts IObservable to an Observable<List<Item>>
                .FirstAsync(); // Get the first (and only) list emitted

            foreach (var item in items)
            {
                result.Add(item.Key);
            }

            return result;
        }
    }
}
