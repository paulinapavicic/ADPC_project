namespace Project_1.Models
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }
        public string Medicationname { get; set; }
        public string Dosage { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }

      
        public int CheckupId { get; set; }

        public Checkup Checkup { get; set; }
    }
}
