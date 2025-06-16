using System.ComponentModel.DataAnnotations;

namespace Project_1.Models
{
    public class Patient
    {

        
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Personal Identification Number is required.")]
        [StringLength(20, ErrorMessage = "Cannot exceed 20 characters.")]
        public string PersonalIdentificationNumber { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        [StringLength(100, ErrorMessage = "Cannot exceed 100 characters.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Sex is required.")]
        [RegularExpression("^[MF]$", ErrorMessage = "Sex must be 'Male' or 'Female'.")]
        public char Sex { get; set; }

        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; }
        public virtual ICollection<Checkup> Checkups { get; set; }
    }
}
