using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel.Args;

namespace Project_1.Minio
{
    public class MinioClientHelper
    {
        private readonly IMinioClient _minioClient;

        public MinioClientHelper(string endpoint, string accessKey, string secretKey)
        {
            _minioClient = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL(false)  // Set to true if using HTTPS
                .Build();
        }

        public IMinioClient GetClient() => _minioClient;

        // Add these methods to your MinioClientHelper class

        public async Task CreateBucketIfNotExistsAsync(string bucketName)
        {
            var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
            bool found = await _minioClient.BucketExistsAsync(bucketExistsArgs);
            if (!found)
            {
                var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(makeBucketArgs);
            }
        }

        public async Task UploadImageAsync(string bucketName, string objectName, Stream data, long size, string contentType)
        {
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(data)
                .WithObjectSize(size)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putObjectArgs);
        }

        public async Task<string> GetPresignedUrlAsync(string bucketName, string objectName, int expirySeconds = 3600)
        {
            var presignedGetObjectArgs = new PresignedGetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithExpiry(expirySeconds);

            return await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);
        }

       

    }
}

