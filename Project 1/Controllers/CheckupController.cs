using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_1.Data;
using Project_1.DTO;
using Project_1.Models;

namespace Project_1.Controllers
{
    [ApiController]
    [Route("api/checkups")]
    public class CheckupController : Controller
    {
        private readonly MedicalDbContext _context;

        public CheckupController(MedicalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Checkup>>> GetAllCheckups()
        {
            return await _context.Checkups.ToListAsync();
        }


        [HttpGet("patient/{patientId}/checkups")]
        public async Task<ActionResult<IEnumerable<Checkup>>> GetCheckupsForPatient(int patientId)
    => await _context.Checkups.Where(c => c.PatientId == patientId).ToListAsync();

        [HttpPost]
        public async Task<ActionResult> AddCheckup([FromBody] CreateCheckupDto checkupDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var checkup = new Checkup
            {
                PatientId = checkupDTO.PatientId,
                ProcedureCode = checkupDTO.ProcedureCode,
                CheckupDate = DateTime.SpecifyKind(checkupDTO.CheckupDate, DateTimeKind.Utc),
            };

            _context.Checkups.Add(checkup);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCheckup(int id, [FromBody] CreateCheckupDto checkupDTO)
        {
            var existingCheckup = await _context.Checkups.FindAsync(id);
            if (existingCheckup == null) return NotFound();

            existingCheckup.PatientId = checkupDTO.PatientId;
            existingCheckup.ProcedureCode = checkupDTO.ProcedureCode;
            existingCheckup.CheckupDate = DateTime.SpecifyKind(checkupDTO.CheckupDate, DateTimeKind.Utc);

            await _context.SaveChangesAsync();
            return Ok();
        }

        //lazy loading
        [HttpGet("{id}/with-details")]
        public async Task<IActionResult> GetCheckupWithDetails(int id)
        {
            var checkup = await _context.Checkups.FindAsync(id);
            if (checkup == null) return NotFound();

           
            var images = checkup.Images?.ToList();
            var prescriptions = checkup.Prescriptions?.ToList();
            var patient = checkup.Patient;

            
            return Ok(new
            {
                checkup.CheckupId,
                checkup.ProcedureCode,
                checkup.CheckupDate,
                Images = images?.Select(img => new { img.ImageId, img.ImageUrl }),
                Prescriptions = prescriptions?.Select(p => new { p.PrescriptionId }),
                Patient = patient != null ? new { patient.PatientId, patient.Name } : null
            });
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCheckup(int id)
        {
            var checkup = await _context.Checkups.FindAsync(id);
            if (checkup == null) return NotFound();

            _context.Checkups.Remove(checkup);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
