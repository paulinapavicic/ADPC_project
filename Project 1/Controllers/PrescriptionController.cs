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

      
       [HttpGet]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescription()
        {
            return await _context.Prescriptions.ToListAsync(); 
        }

        

        [HttpPost]
        public async Task<ActionResult> CreatePrescription([FromBody]  CreatePrescriptionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var prescription = new Prescription
            {
                Medicationname = dto.Medicationname, 
                Dosage = dto.Dosage,
                Startdate = DateTime.SpecifyKind(dto.Startdate, DateTimeKind.Utc),
                Enddate = DateTime.SpecifyKind(dto.Enddate, DateTimeKind.Utc),
                CheckupId = dto.CheckupId
            };

            

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();
            return Ok();

        }

    
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(int id, [FromBody] CreatePrescriptionDto dto)
        {
            

            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
                return NotFound();

            prescription.Medicationname = dto.Medicationname; 
            prescription.Dosage = dto.Dosage;
            prescription.Startdate = dto.Startdate;
            prescription.Enddate = dto.Enddate;

            await _context.SaveChangesAsync();
            return Ok();
        }

     
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
