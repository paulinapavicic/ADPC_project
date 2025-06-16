using System.ComponentModel.DataAnnotations;

namespace Project_1.Models
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }

        [Required(ErrorMessage = "Medication name is required.")]
        [StringLength(200, ErrorMessage = "Cannot exceed 200 characters.")]
        public string Medicationname { get; set; }
        public string Dosage { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        [DataType(DataType.Date)]
        public DateTime Startdate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        [DataType(DataType.Date)]
        public DateTime Enddate { get; set; }

      
        public int CheckupId { get; set; }

        public virtual Checkup Checkup { get; set; }
    }
}
