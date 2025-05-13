using System.ComponentModel.DataAnnotations;

namespace Project_1.DTO
{
    public class ImageUploadDto
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public int CheckupId { get; set; }
    }
}
