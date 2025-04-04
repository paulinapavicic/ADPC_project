using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_1.Data;
using Project_1.Minio;
using Project_1.Models;

namespace Project_1.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImagesController : Controller
    {
        private readonly MedicalDbContext _context;
        private readonly ImageService _imageService;

        public ImagesController(MedicalDbContext context, ImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file, int checkupId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                // Initialize MinIO client
                var bucketName = "images";
                var minioHelper = new MinioClientHelper("localhost:9000", "admin", "admin123");

                // Ensure bucket exists
                await minioHelper.CreateBucketIfNotExistsAsync(bucketName);

                // Generate a unique object name for the image
                string objectName = $"{Guid.NewGuid()}-{file.FileName}";

                // Upload image to MinIO
                using (var stream = file.OpenReadStream())
                {
                    await minioHelper.UploadImageAsync(bucketName, objectName, stream, file.Length, file.ContentType);
                }

                // Generate a presigned URL for accessing the image
                string imageUrl = await minioHelper.GetPresignedUrlAsync(bucketName, objectName);

                // Save metadata in database
                var image = new Image
                {
                    CheckupId = checkupId,
                    ImageUrl = imageUrl
                };

                _context.Images.Add(image);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Image uploaded successfully", imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetImages()
        {
            var images = await _context.Images
                .Select(image => new
                {
                    image.ImageId,
                    image.CheckupId,
                    image.ImageUrl
                })
                .ToListAsync();

            return Ok(images);
        }

    }

}

