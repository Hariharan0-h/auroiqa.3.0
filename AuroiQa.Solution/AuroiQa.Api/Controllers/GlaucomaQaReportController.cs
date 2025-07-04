using AuroiQa.Domain.Data;
using AuroiQa.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuroiQa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GlaucomaQaReportController : ControllerBase
    {
        private readonly EmrContext _context;

        public GlaucomaQaReportController(EmrContext context)
        {
            _context = context;
        }

        // GET: api/GlaucomaQaReport
        // Get all records with optional filtering and pagination
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientGlaucomaQa>>> GetAllRecords(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? patientName = null,
            [FromQuery] string? mrNo = null,
            [FromQuery] DateTime? surgeryDateFrom = null,
            [FromQuery] DateTime? surgeryDateTo = null,
            [FromQuery] string? eyePart = null,
            [FromQuery] string? surgeonType = null,
            [FromQuery] string? surgeryType = null,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var query = _context.PatientGlaucomaData.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(patientName))
                {
                    query = query.Where(x => x.Patient_name != null && x.Patient_name.Contains(patientName));
                }

                if (!string.IsNullOrEmpty(mrNo))
                {
                    query = query.Where(x => x.Mr_no != null && x.Mr_no.Contains(mrNo));
                }

                if (!string.IsNullOrEmpty(eyePart))
                {
                    query = query.Where(x => x.Eye_part != null && x.Eye_part.Contains(eyePart));
                }

                if (!string.IsNullOrEmpty(surgeonType))
                {
                    query = query.Where(x => x.Surgeon_type != null && x.Surgeon_type.Contains(surgeonType));
                }

                if (!string.IsNullOrEmpty(surgeryType))
                {
                    query = query.Where(x => x.Surgery_type != null && x.Surgery_type.Contains(surgeryType));
                }

                if (isActive.HasValue)
                {
                    query = query.Where(x => x.Is_active == isActive.Value);
                }

                if (surgeryDateFrom.HasValue)
                {
                    query = query.Where(x => x.Surgery_date >= surgeryDateFrom.Value);
                }

                if (surgeryDateTo.HasValue)
                {
                    query = query.Where(x => x.Surgery_date <= surgeryDateTo.Value);
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

        // GET: api/GlaucomaQaReport/{id}
        // Get single record by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientGlaucomaQa>> GetRecord(long id)
        {
            try
            {
                var record = await _context.PatientGlaucomaData.FindAsync(id);

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

        // POST: api/GlaucomaQaReport
        // Create new record
        [HttpPost]
        public async Task<ActionResult<PatientGlaucomaQa>> CreateRecord(PatientGlaucomaQa record)
        {
            try
            {
                // Set Is_active to true by default if not specified
                if (!record.Is_active.HasValue)
                {
                    record.Is_active = true;
                }

                _context.PatientGlaucomaData.Add(record);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetRecord), new { id = record.Id },
                    new { message = "Record created successfully", data = record });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating record", error = ex.Message });
            }
        }

        // PUT: api/GlaucomaQaReport/{id}
        // Update existing record
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecord(long id, PatientGlaucomaQa record)
        {
            if (id != record.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            try
            {
                var existingRecord = await _context.PatientGlaucomaData.FindAsync(id);
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

        // DELETE: api/GlaucomaQaReport/{id}
        // Delete record (or mark as inactive)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecord(long id, [FromQuery] bool hardDelete = false)
        {
            try
            {
                var record = await _context.PatientGlaucomaData.FindAsync(id);
                if (record == null)
                {
                    return NotFound(new { message = $"Record with ID {id} not found" });
                }

                if (hardDelete)
                {
                    // Permanently delete the record
                    _context.PatientGlaucomaData.Remove(record);
                }
                else
                {
                    // Soft delete - mark as inactive
                    record.Is_active = false;
                    _context.PatientGlaucomaData.Update(record);
                }

                await _context.SaveChangesAsync();

                var message = hardDelete ? "Record deleted permanently" : "Record marked as inactive";
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting record", error = ex.Message });
            }
        }

        // GET: api/GlaucomaQaReport/search
        // Advanced search functionality
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PatientGlaucomaQa>>> SearchRecords(
            [FromQuery] string searchTerm = "",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.PatientGlaucomaData.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(x =>
                        (x.Patient_name != null && x.Patient_name.ToLower().Contains(searchTerm)) ||
                        (x.Mr_no != null && x.Mr_no.ToLower().Contains(searchTerm)) ||
                        (x.Doctor_first_name != null && x.Doctor_first_name.ToLower().Contains(searchTerm)) ||
                        (x.Eye_part != null && x.Eye_part.ToLower().Contains(searchTerm)) ||
                        (x.Surgery_type != null && x.Surgery_type.ToLower().Contains(searchTerm)) ||
                        (x.Surgeon_type != null && x.Surgeon_type.ToLower().Contains(searchTerm))
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

        // GET: api/GlaucomaQaReport/patient/{mrNo}
        // Get all records for a specific patient by MR number
        [HttpGet("patient/{mrNo}")]
        public async Task<ActionResult<IEnumerable<PatientGlaucomaQa>>> GetPatientRecords(string mrNo)
        {
            try
            {
                var records = await _context.PatientGlaucomaData
                    .Where(x => x.Mr_no == mrNo)
                    .OrderByDescending(x => x.Surgery_date)
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

        // GET: api/GlaucomaQaReport/stats
        // Get basic statistics
        [HttpGet("stats")]
        public async Task<ActionResult> GetStatistics()
        {
            try
            {
                var totalRecords = await _context.PatientGlaucomaData.CountAsync();
                var activeRecords = await _context.PatientGlaucomaData.CountAsync(x => x.Is_active == true);
                var totalPatients = await _context.PatientGlaucomaData
                    .Where(x => x.Mr_no != null)
                    .Select(x => x.Mr_no)
                    .Distinct()
                    .CountAsync();

                var surgeryTypeStats = await _context.PatientGlaucomaData
                    .Where(x => x.Surgery_type != null)
                    .GroupBy(x => x.Surgery_type)
                    .Select(g => new { SurgeryType = g.Key, Count = g.Count() })
                    .ToListAsync();

                var eyePartStats = await _context.PatientGlaucomaData
                    .Where(x => x.Eye_part != null)
                    .GroupBy(x => x.Eye_part)
                    .Select(g => new { EyePart = g.Key, Count = g.Count() })
                    .ToListAsync();

                var surgeonTypeStats = await _context.PatientGlaucomaData
                    .Where(x => x.Surgeon_type != null)
                    .GroupBy(x => x.Surgeon_type)
                    .Select(g => new { SurgeonType = g.Key, Count = g.Count() })
                    .ToListAsync();

                var complicationStats = await _context.PatientGlaucomaData
                    .Where(x => !string.IsNullOrEmpty(x.Postop_complications))
                    .CountAsync();

                return Ok(new
                {
                    totalRecords,
                    activeRecords,
                    inactiveRecords = totalRecords - activeRecords,
                    totalPatients,
                    surgeryTypeStats,
                    eyePartStats,
                    surgeonTypeStats,
                    recordsWithComplications = complicationStats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving statistics", error = ex.Message });
            }
        }

        // GET: api/GlaucomaQaReport/iop-analysis
        // Get IOP analysis data
        [HttpGet("iop-analysis")]
        public async Task<ActionResult> GetIOPAnalysis()
        {
            try
            {
                var iopData = await _context.PatientGlaucomaData
                    .Where(x => x.Pre_operative_iop.HasValue && x.Post_operative_iop.HasValue)
                    .Select(x => new {
                        x.Id,
                        x.Patient_name,
                        x.Mr_no,
                        x.Surgery_date,
                        PreOpIOP = x.Pre_operative_iop,
                        PostOpIOP = x.Post_operative_iop,
                        IOPReduction = x.Pre_operative_iop - x.Post_operative_iop
                    })
                    .ToListAsync();

                var avgPreOpIOP = iopData.Average(x => x.PreOpIOP ?? 0);
                var avgPostOpIOP = iopData.Average(x => x.PostOpIOP ?? 0);
                var avgIOPReduction = iopData.Average(x => x.IOPReduction ?? 0);

                return Ok(new
                {
                    totalRecordsWithIOP = iopData.Count,
                    averagePreOperativeIOP = Math.Round(avgPreOpIOP, 2),
                    averagePostOperativeIOP = Math.Round(avgPostOpIOP, 2),
                    averageIOPReduction = Math.Round(avgIOPReduction, 2),
                    iopData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving IOP analysis", error = ex.Message });
            }
        }

        // GET: api/GlaucomaQaReport/complications
        // Get complications analysis
        [HttpGet("complications")]
        public async Task<ActionResult> GetComplicationsAnalysis()
        {
            try
            {
                var complicationTypes = await _context.PatientGlaucomaData
                    .Where(x => !string.IsNullOrEmpty(x.Postop_complications))
                    .GroupBy(x => x.Postop_complications)
                    .Select(g => new { ComplicationType = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync();

                var intraOpComplications = await _context.PatientGlaucomaData
                    .Where(x => !string.IsNullOrEmpty(x.Intraoperative_complication))
                    .GroupBy(x => x.Intraoperative_complication)
                    .Select(g => new { ComplicationType = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync();

                var anesthesiaComplications = await _context.PatientGlaucomaData
                    .Where(x => !string.IsNullOrEmpty(x.Anesthesia_complication))
                    .GroupBy(x => x.Anesthesia_complication)
                    .Select(g => new { ComplicationType = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync();

                return Ok(new
                {
                    postOperativeComplications = complicationTypes,
                    intraOperativeComplications = intraOpComplications,
                    anesthesiaComplications = anesthesiaComplications
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving complications analysis", error = ex.Message });
            }
        }

        // Helper method to check if record exists
        private bool RecordExists(long id)
        {
            return _context.PatientGlaucomaData.Any(e => e.Id == id);
        }
    }
}
