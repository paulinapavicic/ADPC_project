namespace Project_1.DTO
{
    public class CreateCheckupDto
    {
        public int PatientId { get; set; } 
        public string ProcedureCode { get; set; }
        public DateTime CheckupDate { get; set; } 
    }
}
