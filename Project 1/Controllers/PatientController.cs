using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_1.Data;
using Project_1.DTO;
using Project_1.Models;
using Project_1.Repository;
using System.Text;

namespace Project_1.Controllers
{
    [ApiController]
    [Route("api/patients")]
    public class PatientController : Controller
    {
        private readonly IRepository<Patient> _patientRepository;

        public PatientController(RepositoryFactory factory)
        {
            _patientRepository = factory.CreateRepository<Patient>();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            var patients = await _patientRepository.GetAll().ToListAsync();
            return Ok(patients);
        }



        [HttpPost]
        public async Task<ActionResult> AddPatient([FromBody] CreatePatientDto patientDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var patient = new Patient
            {
                PersonalIdentificationNumber = patientDTO.PersonalIdentificationNumber,
                Name = patientDTO.Name,
                Surname = patientDTO.Surname,
                DateOfBirth = DateTime.SpecifyKind(patientDTO.DateOfBirth, DateTimeKind.Utc),
                Sex = patientDTO.Sex
            };

            await _patientRepository.AddAsync(patient);
            await _patientRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePatient(int id, [FromBody] CreatePatientDto patientDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingPatient = await _patientRepository.GetByIdAsync(id);
            if (existingPatient == null) return NotFound();

            existingPatient.PersonalIdentificationNumber = patientDTO.PersonalIdentificationNumber;
            existingPatient.Name = patientDTO.Name;
            existingPatient.Surname = patientDTO.Surname;
            existingPatient.DateOfBirth = DateTime.SpecifyKind(patientDTO.DateOfBirth, DateTimeKind.Utc);
            existingPatient.Sex = patientDTO.Sex;

            _patientRepository.Update(existingPatient);
            await _patientRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePatient(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null) return NotFound();

            _patientRepository.Delete(patient);
            await _patientRepository.SaveChangesAsync();
            return Ok();
        }

     


        [HttpGet("export")]
        public async Task<IActionResult> ExportToCsv()
        {
            var patients = await _patientRepository.GetAll().ToListAsync();
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
