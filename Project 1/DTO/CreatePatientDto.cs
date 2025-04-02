namespace Project_1.DTO
{
    public class CreatePatientDto
    {
        public string PersonalIdentificationNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public char Sex { get; set; }
    }
}
