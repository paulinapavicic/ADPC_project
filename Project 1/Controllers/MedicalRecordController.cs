using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_1.Data;
using Project_1.DTO;
using Project_1.Models;

namespace Project_1.Controllers
{
    [ApiController]
    [Route("api/medicalrecords")]
    public class MedicalRecordController : Controller
    {
        private readonly MedicalDbContext _context;

        public MedicalRecordController(MedicalDbContext context)
        {
            _context = context;
        }

        //all
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalRecord>>> GetAllMedicalRecords()
        {
            return await _context.MedicalRecords.ToListAsync();
        }




        //by id 
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalRecord>> GetMedicalRecordById(int id)
        {
            var record = await _context.MedicalRecords.FindAsync(id);
            if (record == null)
            {
                return NotFound($"Medical record with ID {id} not found.");
            }
            return Ok(record);
        }



        [HttpPost]
        public async Task<ActionResult> AddMedicalRecord([FromBody] CreateMedicalRecordDto recordDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var record = new MedicalRecord
            {
                PatientId = recordDTO.PatientId,
                DiseaseName = recordDTO.DiseaseName,
                StartDate = DateTime.SpecifyKind(recordDTO.StartDate, DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(recordDTO.EndDate , DateTimeKind.Utc),
            };

            _context.MedicalRecords.Add(record);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateMedicalRecord(int id, [FromBody] CreateMedicalRecordDto recordDTO)
        {
            var existingRecord = await _context.MedicalRecords.FindAsync(id);
            if (existingRecord == null) return NotFound();

            existingRecord.PatientId = recordDTO.PatientId;
            existingRecord.DiseaseName = recordDTO.DiseaseName;
            existingRecord.StartDate = recordDTO.StartDate;
            existingRecord.EndDate = recordDTO.EndDate;

            await _context.SaveChangesAsync();
            return Ok();
        }


       

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMedicalRecord(int id)
        {
            var record = await _context.MedicalRecords.FindAsync(id);
            if (record == null) return NotFound();

            _context.MedicalRecords.Remove(record);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
