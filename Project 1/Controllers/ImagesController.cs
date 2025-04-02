using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_1.Data;
using Project_1.Models;

namespace Project_1.Controllers
{
    public class ImagesController : Controller
    {
        private readonly MedicalDbContext _context;

        public ImagesController(MedicalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetImages()
        {
            var images = await _context.Images.ToListAsync();
            return Ok(images);
        }


        [HttpGet("checkup/{checkupId}/images")]
        public async Task<ActionResult<IEnumerable<Image>>> GetImagesForCheckup(int checkupId)
     => await _context.Images.Where(i => i.CheckupId == checkupId).ToListAsync();


        [HttpPost]
        public async Task<ActionResult> AddImage([FromBody] Image image)
        {
            if (ModelState.IsValid)
            {
                _context.Images.Add(image);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteImage(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null) return NotFound();

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file, int checkupId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var filePath = Path.Combine("wwwroot/images", file.FileName); // Save to local folder

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var image = new Image
            {
                CheckupId = checkupId,
                ImageUrl = $"/images/{file.FileName}" // Store relative path in database
            };

            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            return Ok($"Image uploaded successfully: {file.FileName}");
        }

    }
}
