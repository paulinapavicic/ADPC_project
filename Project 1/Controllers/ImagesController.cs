using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_1.Data;
using Project_1.DTO;
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
        public async Task<IActionResult> UploadImage([FromForm] ImageUploadDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {


                
                var checkup = await _context.Checkups
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CheckupId == dto.CheckupId);

                if (checkup == null)
                {
                    return NotFound("Checkup not found.");
                }

                Console.WriteLine($"Checkup ID: {dto.CheckupId}, Procedure Code: {checkup.ProcedureCode}");




                var allowedProcedureCodes = new List<string>
        {
            "X-RAY", "CT", "MR", "ULTRA", "MAMMO",
            "ECG", "ECHO", "DERM", "EYE", "DENTA"
        };

              
                if (!allowedProcedureCodes.Contains(checkup.ProcedureCode.ToUpper()))
                {
                    return BadRequest("Image upload not allowed for this procedure code.");
                }
              
                // Initialize MinIO client
                var bucketName = "images";
                var minioHelper = new MinioClientHelper("localhost:9000", "admin", "admin123");

                
                await minioHelper.CreateBucketIfNotExistsAsync(bucketName);

               
                string objectName = $"{Guid.NewGuid()}-{dto.File.FileName}";

                // Upload image to MinIO
                using (var stream = dto.File.OpenReadStream())
                {
                    await minioHelper.UploadImageAsync(bucketName, objectName, stream, dto.File.Length, dto.File.ContentType);
                }

               
                string imageUrl = await minioHelper.GetPresignedUrlAsync(bucketName, objectName);

              
                var image = new Image
                {
                    CheckupId = dto.CheckupId,
                    ImageUrl = $"http://localhost:9000/{bucketName}/{objectName}"
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
            var images = await _context.Images.ToListAsync();
            return Ok(images);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteImage(int id)
        {
            var images = await _context.Images.FindAsync(id);
            if (images == null) return NotFound();

            _context.Images.Remove(images);
            await _context.SaveChangesAsync();
            return Ok();
        }


    }

}

