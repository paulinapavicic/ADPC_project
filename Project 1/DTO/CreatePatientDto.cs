using System.ComponentModel.DataAnnotations;

namespace Project_1.DTO
{
    public class CreatePatientDto
    {
        [Required(ErrorMessage = "Personal Identification Number is required.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Personal Identification Number must be exactly 11 digits.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Personal Identification Number must contain only digits.")]
        public string PersonalIdentificationNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public char Sex { get; set; }
    }
}
