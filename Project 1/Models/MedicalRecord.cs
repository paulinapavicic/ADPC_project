using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project_1.Models
{
    public class MedicalRecord
    {
        public int RecordId { get; set; }

        [Required(ErrorMessage = "Patient ID is required.")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Disease name is required.")]
        [StringLength(200, ErrorMessage = "Cannot exceed 200 characters.")]
        public string DiseaseName { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        [DataType(DataType.Date)]

        public DateTime EndDate { get; set; }

        public virtual Patient Patient { get; set; }

        
    }
}