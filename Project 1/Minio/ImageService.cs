namespace Project_1.Minio
{
    public class ImageService
    {
        private readonly MinioClientHelper _minioHelper;
        private readonly string _bucketName = "images";

        public ImageService(string endpoint, string accessKey, string secretKey)
        {
            _minioHelper = new MinioClientHelper(endpoint, accessKey, secretKey);
        }

        public async Task InitializeBucketAsync()
        {
            await _minioHelper.CreateBucketIfNotExistsAsync(_bucketName);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            // Generate a unique object name
            string objectName = $"{Guid.NewGuid()}-{file.FileName}";

            // Ensure bucket exists
            await InitializeBucketAsync();

            // Upload file to MinIO
            using (var stream = file.OpenReadStream())
            {
                await _minioHelper.UploadImageAsync(
                    _bucketName,
                    objectName,
                    stream,
                    file.Length,
                    file.ContentType);
            }

            // Return the object name
            return objectName;
        }

        public async Task<string> GetImageUrlAsync(string objectName)
        {
            return await _minioHelper.GetPresignedUrlAsync(_bucketName, objectName);
        }
    }
}

