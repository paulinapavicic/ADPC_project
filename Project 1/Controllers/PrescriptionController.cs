using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_1.Data;
using Project_1.DTO;
using Project_1.Models;

namespace Project_1.Controllers
{
    [ApiController]
    [Route("api/prescriptions")]
    public class PrescriptionController:Controller
    {

        private readonly MedicalDbContext _context;

        public PrescriptionController(MedicalDbContext context)
        {
            _context = context;
        }

        // GET: api/prescriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CreatePrescriptionDto>>> GetPrescriptions()
        {
            var prescriptions = await _context.Prescriptions
                .Select(p => new CreatePrescriptionDto
                {
                    PrescriptionId = p.PrescriptionId,
                    Medicationname = p.Medicationname,
                    Dosage = p.Dosage,
                    Startdate = p.Startdate,
                    Enddate = p.Enddate,
                    CheckupId = p.CheckupId
                })
                .ToListAsync();

            return Ok(prescriptions);
        }

        // GET: api/prescriptions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CreatePrescriptionDto>> GetPrescription(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);

            if (prescription == null)
                return NotFound();

            var dto = new CreatePrescriptionDto
            {
                PrescriptionId = prescription.PrescriptionId,
                Medicationname = prescription.Medicationname,
                Dosage = prescription.Dosage,
                Startdate = prescription.Startdate,
                Enddate = prescription.Enddate,
                CheckupId = prescription.CheckupId
            };

            return Ok(dto);
        }

        // POST: api/prescriptions
        [HttpPost]
        public async Task<ActionResult<Prescription>> CreatePrescription(CreatePrescriptionDto dto)
        {
            // Validate Checkup ID
            var checkupExists = await _context.Checkups.AnyAsync(c => c.CheckupId == dto.CheckupId);
            if (!checkupExists)
                return BadRequest("Invalid Checkup ID.");

            // Create new Prescription entity
            var prescription = new Prescription
            {
                Medicationname = dto.Medicationname.ToUpper(), // Ensure uppercase storage
                Dosage = dto.Dosage,
                Startdate = dto.Startdate,
                Enddate = dto.Enddate,
                CheckupId = dto.CheckupId
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPrescription), new { id = prescription.PrescriptionId }, prescription);
        }

        // PUT: api/prescriptions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(int id, CreatePrescriptionDto dto)
        {
            if (id != dto.PrescriptionId)
                return BadRequest();

            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
                return NotFound();

            // Update fields
            prescription.Medicationname = dto.Medicationname.ToUpper(); // Ensure uppercase storage
            prescription.Dosage = dto.Dosage;
            prescription.Startdate = dto.Startdate;
            prescription.Enddate = dto.Enddate;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Prescriptions.Any(p => p.PrescriptionId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/prescriptions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);

            if (prescription == null)
                return NotFound();

            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

        
}
