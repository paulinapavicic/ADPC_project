namespace Project_1.DTO
{
    public class CreateMedicalRecordDto
    {
        public int PatientId { get; set; }
        public string DiseaseName { get; set; } 
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } 
    }
}
