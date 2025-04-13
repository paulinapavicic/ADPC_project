using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project_1.Models
{
    public class Checkup
    {
        public int CheckupId { get; set; }

        [Required(ErrorMessage = "Patient ID is required.")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Procedure code is required.")]
        [StringLength(10, ErrorMessage = "Cannot exceed 10 characters.")]
        public string ProcedureCode { get; set; }

        [Required(ErrorMessage = "Checkup date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime CheckupDate { get; set; }

        public Patient Patient { get; set; }
        public ICollection<Image> Images { get; set; }

        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
