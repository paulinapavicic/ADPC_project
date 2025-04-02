using System.ComponentModel.DataAnnotations;

namespace Project_1.Models
{
    public class Image
    {
        public int ImageId { get; set; }

        [Required(ErrorMessage = "Checkup ID is required.")]
        public int CheckupId { get; set; }

        [Required(ErrorMessage = "Image URL is required.")]
        [Url(ErrorMessage = "Invalid URL format.")]
        public string ImageUrl { get; set; }

        public Checkup Checkup { get; set; }
    }
}
