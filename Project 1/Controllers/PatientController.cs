using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_1.Data;
using Project_1.DTO;
using Project_1.Models;
using System.Text;

namespace Project_1.Controllers
{
    [ApiController]
    [Route("api/patients")]
    public class PatientController : Controller
    {
        private readonly MedicalDbContext _context;

        public PatientController(MedicalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            return await _context.Patients.ToListAsync(); 
        }



        [HttpPost]
        public async Task<ActionResult> AddPatient([FromBody] CreatePatientDto patientDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

          
            var patient = new Patient
            {
                PersonalIdentificationNumber = patientDTO.PersonalIdentificationNumber,
                Name = patientDTO.Name,
                Surname = patientDTO.Surname,
                DateOfBirth = DateTime.SpecifyKind(patientDTO.DateOfBirth, DateTimeKind.Utc),
                Sex = patientDTO.Sex
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePatient(int id, [FromBody] CreatePatientDto patientDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPatient = await _context.Patients.FindAsync(id);
            if (existingPatient == null) return NotFound();

            // Update fields from DTO
            existingPatient.PersonalIdentificationNumber = patientDTO.PersonalIdentificationNumber;
            existingPatient.Name = patientDTO.Name;
            existingPatient.Surname = patientDTO.Surname;
            existingPatient.DateOfBirth = patientDTO.DateOfBirth;
            existingPatient.Sex = patientDTO.Sex;

            await _context.SaveChangesAsync();
            return Ok();
        }



        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportToCsv()
        {
            var patients = await _context.Patients.ToListAsync();
            var csv = new StringBuilder();
            csv.AppendLine("PersonalIdentificationNumber,Name,Surname,DateOfBirth,Sex");

            foreach (var patient in patients)
            {
                csv.AppendLine($"{patient.PersonalIdentificationNumber},{patient.Name},{patient.Surname},{patient.DateOfBirth},{patient.Sex}");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "patients.csv");
        }


    }
}
