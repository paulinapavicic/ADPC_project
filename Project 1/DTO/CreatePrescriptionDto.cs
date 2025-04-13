namespace Project_1.DTO
{
    public class CreatePrescriptionDto
    {
        public int PrescriptionId { get; set; } // Optional for GET responses
        public string Medicationname { get; set; }
        public string Dosage { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public int CheckupId { get; set; } // Foreign key to Checkup
    }
}
