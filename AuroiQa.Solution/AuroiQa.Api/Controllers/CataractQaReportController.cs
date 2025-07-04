using AuroiQa.Domain.Data;
using AuroiQa.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuroiQa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CataractQaReportController : ControllerBase
    {
        private readonly EmrContext _context;

        public CataractQaReportController(EmrContext context)
        {
            _context = context;
        }

        // GET: api/CataractQaReport
        // Get all records with optional filtering and pagination
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientCataractQa>>> GetAllRecords(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? patientName = null,
            [FromQuery] string? mrNo = null,
            [FromQuery] DateTime? surgeryDateFrom = null,
            [FromQuery] DateTime? surgeryDateTo = null,
            [FromQuery] string? eye = null,
            [FromQuery] string? surgeonType = null)
        {
            try
            {
                var query = _context.PatientCataractData.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(patientName))
                {
                    query = query.Where(x => x.PATIENT_NAME != null && x.PATIENT_NAME.Contains(patientName));
                }

                if (!string.IsNullOrEmpty(mrNo))
                {
                    query = query.Where(x => x.Mr_no != null && x.Mr_no.Contains(mrNo));
                }

                if (!string.IsNullOrEmpty(eye))
                {
                    query = query.Where(x => x.Eye != null && x.Eye.Contains(eye));
                }

                if (!string.IsNullOrEmpty(surgeonType))
                {
                    query = query.Where(x => x.SurgeonType != null && x.SurgeonType.Contains(surgeonType));
                }

                if (surgeryDateFrom.HasValue)
                {
                    query = query.Where(x => x.SurgeryDate >= surgeryDateFrom.Value);
                }

                if (surgeryDateTo.HasValue)
                {
                    query = query.Where(x => x.SurgeryDate <= surgeryDateTo.Value);
                }

                // Get total count before pagination
                var totalCount = await query.CountAsync();

                // Apply pagination
                var records = await query
                    .OrderByDescending(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Add pagination headers
                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                Response.Headers.Add("X-Page", page.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());
                Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)totalCount / pageSize)).ToString());

                return Ok(new { data = records, totalCount, page, pageSize });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving records", error = ex.Message });
            }
        }

        // GET: api/CataractQaReport/{id}
        // Get single record by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientCataractQa>> GetRecord(long id)
        {
            try
            {
                var record = await _context.PatientCataractData.FindAsync(id);

                if (record == null)
                {
                    return NotFound(new { message = $"Record with ID {id} not found" });
                }

                return Ok(record);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving record", error = ex.Message });
            }
        }

        // POST: api/CataractQaReport
        // Create new record
        [HttpPost]
        public async Task<ActionResult<PatientCataractQa>> CreateRecord(PatientCataractQa record)
        {
            try
            {
                // Set InsertedDate if not provided
                if (!record.InsertedDate.HasValue)
                {
                    record.InsertedDate = DateTime.Now;
                }

                _context.PatientCataractData.Add(record);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetRecord), new { id = record.Id },
                    new { message = "Record created successfully", data = record });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating record", error = ex.Message });
            }
        }

        // PUT: api/CataractQaReport/{id}
        // Update existing record
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecord(long id, PatientCataractQa record)
        {
            if (id != record.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            try
            {
                var existingRecord = await _context.PatientCataractData.FindAsync(id);
                if (existingRecord == null)
                {
                    return NotFound(new { message = $"Record with ID {id} not found" });
                }

                // Update all properties
                _context.Entry(existingRecord).CurrentValues.SetValues(record);

                await _context.SaveChangesAsync();

                return Ok(new { message = "Record updated successfully", data = existingRecord });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecordExists(id))
                {
                    return NotFound(new { message = $"Record with ID {id} not found" });
                }
                else
                {
                    return Conflict(new { message = "Record was modified by another user" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating record", error = ex.Message });
            }
        }

        // DELETE: api/CataractQaReport/{id}
        // Delete record
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecord(long id)
        {
            try
            {
                var record = await _context.PatientCataractData.FindAsync(id);
                if (record == null)
                {
                    return NotFound(new { message = $"Record with ID {id} not found" });
                }

                _context.PatientCataractData.Remove(record);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Record deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting record", error = ex.Message });
            }
        }

        // GET: api/CataractQaReport/search
        // Advanced search functionality
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PatientCataractQa>>> SearchRecords(
            [FromQuery] string searchTerm = "",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.PatientCataractData.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(x =>
                        (x.PATIENT_NAME != null && x.PATIENT_NAME.ToLower().Contains(searchTerm)) ||
                        (x.Mr_no != null && x.Mr_no.ToLower().Contains(searchTerm)) ||
                        (x.Firstname != null && x.Firstname.ToLower().Contains(searchTerm)) ||
                        (x.Eye != null && x.Eye.ToLower().Contains(searchTerm)) ||
                        (x.SurgeryType != null && x.SurgeryType.ToLower().Contains(searchTerm))
                    );
                }

                var totalCount = await query.CountAsync();
                var records = await query
                    .OrderByDescending(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new { data = records, totalCount, page, pageSize, searchTerm });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error searching records", error = ex.Message });
            }
        }

        // GET: api/CataractQaReport/patient/{mrNo}
        // Get all records for a specific patient by MR number
        [HttpGet("patient/{mrNo}")]
        public async Task<ActionResult<IEnumerable<PatientCataractQa>>> GetPatientRecords(string mrNo)
        {
            try
            {
                var records = await _context.PatientCataractData
                    .Where(x => x.Mr_no == mrNo)
                    .OrderByDescending(x => x.SurgeryDate)
                    .ToListAsync();

                if (!records.Any())
                {
                    return NotFound(new { message = $"No records found for patient with MR number {mrNo}" });
                }

                return Ok(new { data = records, count = records.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving patient records", error = ex.Message });
            }
        }

        // GET: api/CataractQaReport/stats
        // Get basic statistics
        [HttpGet("stats")]
        public async Task<ActionResult> GetStatistics()
        {
            try
            {
                var totalRecords = await _context.PatientCataractData.CountAsync();
                var totalPatients = await _context.PatientCataractData
                    .Where(x => x.Mr_no != null)
                    .Select(x => x.Mr_no)
                    .Distinct()
                    .CountAsync();

                var surgeryTypeStats = await _context.PatientCataractData
                    .Where(x => x.SurgeryType != null)
                    .GroupBy(x => x.SurgeryType)
                    .Select(g => new { SurgeryType = g.Key, Count = g.Count() })
                    .ToListAsync();

                var eyeStats = await _context.PatientCataractData
                    .Where(x => x.Eye != null)
                    .GroupBy(x => x.Eye)
                    .Select(g => new { Eye = g.Key, Count = g.Count() })
                    .ToListAsync();

                return Ok(new
                {
                    totalRecords,
                    totalPatients,
                    surgeryTypeStats,
                    eyeStats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving statistics", error = ex.Message });
            }
        }

        // Helper method to check if record exists
        private bool RecordExists(long id)
        {
            return _context.PatientCataractData.Any(e => e.Id == id);
        }
    }
}
