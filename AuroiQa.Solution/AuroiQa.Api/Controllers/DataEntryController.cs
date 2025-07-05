using AuroiQa.Domain.Data;
using AuroiQa.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;

namespace AuroiQa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataEntryController : ControllerBase
    {
        private readonly EmrContext _context;
        private readonly ILogger<DataEntryController> _logger;

        public DataEntryController(EmrContext context, ILogger<DataEntryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/DataEntry/test-emr-connection
        [HttpPost("test-emr-connection")]
        public async Task<ActionResult> TestEmrConnection([FromBody] EmrConnectionRequest request)
        {
            try
            {
                using (var connection = new SqlConnection(request.ConnectionString))
                {
                    await connection.OpenAsync();
                    return Ok(new { success = true, message = "Connection successful!" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EMR connection test failed");
                return Ok(new { success = false, message = ex.Message });
            }
        }

        // POST: api/DataEntry/sync-from-emr
        [HttpPost("sync-from-emr")]
        public async Task<ActionResult> SyncFromEmr([FromBody] EmrSyncRequest request)
        {
            try
            {
                var importedCount = 0;
                var errors = new List<string>();

                using (var emrConnection = new SqlConnection(request.ConnectionString))
                {
                    await emrConnection.OpenAsync();

                    if (request.DataType.ToLower() == "cataract")
                    {
                        importedCount = await SyncCataractData(emrConnection, errors);
                    }
                    else if (request.DataType.ToLower() == "glaucoma")
                    {
                        importedCount = await SyncGlaucomaData(emrConnection, errors);
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "Invalid data type" });
                    }
                }

                return Ok(new
                {
                    success = true,
                    recordsImported = importedCount,
                    message = $"Successfully imported {importedCount} records",
                    errors = errors.Count > 0 ? errors : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EMR sync failed");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/DataEntry/download-template/{dataType}
        [HttpGet("download-template/{dataType}")]
        public async Task<ActionResult> DownloadTemplate(string dataType)
        {
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add($"{dataType.ToUpper()} Data");

                    if (dataType.ToLower() == "cataract")
                    {
                        CreateCataractTemplate(worksheet);
                    }
                    else if (dataType.ToLower() == "glaucoma")
                    {
                        CreateGlaucomaTemplate(worksheet);
                    }
                    else
                    {
                        return BadRequest("Invalid data type");
                    }

                    var stream = new MemoryStream();
                    await package.SaveAsAsync(stream);
                    stream.Position = 0;

                    var fileName = $"{dataType}_template.xlsx";
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Template download failed");
                return StatusCode(500, new { message = "Error generating template", error = ex.Message });
            }
        }

        // POST: api/DataEntry/upload-bulk-data
        [HttpPost("upload-bulk-data")]
        public async Task<ActionResult> UploadBulkData([FromForm] IFormFile file, [FromForm] string dataType)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file provided" });
                }

                if (!IsExcelFile(file))
                {
                    return BadRequest(new { message = "Please provide a valid Excel file" });
                }

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                var result = new BulkUploadResult();
                var errors = new List<BulkUploadError>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];

                        if (dataType.ToLower() == "cataract")
                        {
                            result = await ProcessCataractUpload(worksheet, errors);
                        }
                        else if (dataType.ToLower() == "glaucoma")
                        {
                            result = await ProcessGlaucomaUpload(worksheet, errors);
                        }
                        else
                        {
                            return BadRequest(new { message = "Invalid data type" });
                        }
                    }
                }

                result.Errors = errors;
                result.ErrorCount = errors.Count;

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk upload failed");
                return StatusCode(500, new { message = "Upload failed", error = ex.Message });
            }
        }

        #region Private Helper Methods

        private async Task<int> SyncCataractData(SqlConnection emrConnection, List<string> errors)
        {
            var importedCount = 0;

            // This is a sample query - adjust based on your EMR database schema
            var query = @"
                SELECT 
                    PatientName, MRNumber, SurgeryDate, LensName, ModelName, 
                    IOLPower, Eye, SurgeryType, Age, Sex, SurgeonType,
                    Anesthesia, PreBCVA, PreUCVA, FollowupBCVA, FollowupUCVA
                FROM CataractSurgeryRecords 
                WHERE SurgeryDate >= DATEADD(day, -30, GETDATE())";

            using (var command = new SqlCommand(query, emrConnection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    try
                    {
                        var record = new PatientCataractQa
                        {
                            PATIENT_NAME = GetSafeString(reader, "PatientName"),
                            Mr_no = GetSafeString(reader, "MRNumber"),
                            SurgeryDate = GetSafeDateTime(reader, "SurgeryDate"),
                            Lensname = GetSafeString(reader, "LensName"),
                            Modelname = GetSafeString(reader, "ModelName"),
                            IolPower = GetSafeString(reader, "IOLPower"),
                            Eye = GetSafeString(reader, "Eye"),
                            SurgeryType = GetSafeString(reader, "SurgeryType"),
                            Age = GetSafeInt(reader, "Age"),
                            SEX = GetSafeString(reader, "Sex"),
                            SurgeonType = GetSafeString(reader, "SurgeonType"),
                            Anesthesia = GetSafeString(reader, "Anesthesia"),
                            Pre_BCVA = GetSafeString(reader, "PreBCVA"),
                            Pre_UCVA = GetSafeString(reader, "PreUCVA"),
                            FollowupBCVA = GetSafeString(reader, "FollowupBCVA"),
                            FollowupUCVA = GetSafeString(reader, "FollowupUCVA"),
                            InsertedDate = DateTime.Now
                        };

                        // Check for duplicate
                        var exists = await _context.PatientCataractData
                            .AnyAsync(x => x.Mr_no == record.Mr_no && x.SurgeryDate == record.SurgeryDate);

                        if (!exists)
                        {
                            _context.PatientCataractData.Add(record);
                            importedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error processing record for {GetSafeString(reader, "PatientName")}: {ex.Message}");
                    }
                }
            }

            await _context.SaveChangesAsync();
            return importedCount;
        }

        private async Task<int> SyncGlaucomaData(SqlConnection emrConnection, List<string> errors)
        {
            var importedCount = 0;

            // This is a sample query - adjust based on your EMR database schema
            var query = @"
                SELECT 
                    PatientName, MRNumber, SurgeryDate, EyePart, SurgeryType, 
                    Age, Sex, SurgeonType, PreOpIOP, PostOpIOP,
                    Anesthesia, PreBCVA, PreUCVA, FollowupBCVA, FollowupUCVA
                FROM GlaucomaSurgeryRecords 
                WHERE SurgeryDate >= DATEADD(day, -30, GETDATE())";

            using (var command = new SqlCommand(query, emrConnection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    try
                    {
                        var record = new PatientGlaucomaQa
                        {
                            Patient_name = GetSafeString(reader, "PatientName"),
                            Mr_no = GetSafeString(reader, "MRNumber"),
                            Surgery_date = GetSafeDateTime(reader, "SurgeryDate"),
                            Eye_part = GetSafeString(reader, "EyePart"),
                            Surgery_type = GetSafeString(reader, "SurgeryType"),
                            Age = GetSafeInt(reader, "Age"),
                            Sex = GetSafeString(reader, "Sex"),
                            Surgeon_type = GetSafeString(reader, "SurgeonType"),
                            Pre_operative_iop = GetSafeInt(reader, "PreOpIOP"),
                            Post_operative_iop = GetSafeInt(reader, "PostOpIOP"),
                            Anesthesia = GetSafeString(reader, "Anesthesia"),
                            Pre_bcva = GetSafeString(reader, "PreBCVA"),
                            Pre_ucva = GetSafeString(reader, "PreUCVA"),
                            Followup_bcva = GetSafeString(reader, "FollowupBCVA"),
                            Followup_ucva = GetSafeString(reader, "FollowupUCVA"),
                            Is_active = true
                        };

                        // Check for duplicate
                        var exists = await _context.PatientGlaucomaData
                            .AnyAsync(x => x.Mr_no == record.Mr_no && x.Surgery_date == record.Surgery_date);

                        if (!exists)
                        {
                            _context.PatientGlaucomaData.Add(record);
                            importedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error processing record for {GetSafeString(reader, "PatientName")}: {ex.Message}");
                    }
                }
            }

            await _context.SaveChangesAsync();
            return importedCount;
        }

        private void CreateCataractTemplate(ExcelWorksheet worksheet)
        {
            var headers = new[]
            {
                "Patient Name*", "MR Number*", "Surgery Date", "Age", "Sex", "Eye",
                "Surgery Type*", "Surgeon Type", "Lens Name", "Model Name", "IOL Power",
                "Anesthesia", "Pre-op BCVA", "Pre-op UCVA", "Follow-up BCVA", "Follow-up UCVA"
            };

            // Set headers
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            // Add sample data
            worksheet.Cells[2, 1].Value = "John Doe";
            worksheet.Cells[2, 2].Value = "MR001";
            worksheet.Cells[2, 3].Value = DateTime.Now.ToString("yyyy-MM-dd");
            worksheet.Cells[2, 4].Value = 65;
            worksheet.Cells[2, 5].Value = "M";
            worksheet.Cells[2, 6].Value = "Right";
            worksheet.Cells[2, 7].Value = "Phacoemulsification";
            worksheet.Cells[2, 8].Value = "Consultant";

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private void CreateGlaucomaTemplate(ExcelWorksheet worksheet)
        {
            var headers = new[]
            {
                "Patient Name*", "MR Number*", "Surgery Date", "Age", "Sex", "Eye Part",
                "Surgery Type*", "Surgeon Type", "Pre-operative IOP", "Post-operative IOP",
                "Anesthesia", "Pre-op BCVA", "Pre-op UCVA", "Follow-up BCVA", "Follow-up UCVA", "Active"
            };

            // Set headers
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
            }

            // Add sample data
            worksheet.Cells[2, 1].Value = "Jane Smith";
            worksheet.Cells[2, 2].Value = "MR002";
            worksheet.Cells[2, 3].Value = DateTime.Now.ToString("yyyy-MM-dd");
            worksheet.Cells[2, 4].Value = 70;
            worksheet.Cells[2, 5].Value = "F";
            worksheet.Cells[2, 6].Value = "Left";
            worksheet.Cells[2, 7].Value = "Trabeculectomy";
            worksheet.Cells[2, 8].Value = "Senior Resident";
            worksheet.Cells[2, 9].Value = 28;
            worksheet.Cells[2, 10].Value = 16;
            worksheet.Cells[2, 16].Value = "TRUE";

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private async Task<BulkUploadResult> ProcessCataractUpload(ExcelWorksheet worksheet, List<BulkUploadError> errors)
        {
            var result = new BulkUploadResult();
            var totalRows = worksheet.Dimension?.Rows ?? 0;

            if (totalRows <= 1) // Only header row
            {
                return result;
            }

            result.TotalRows = totalRows - 1; // Exclude header

            for (int row = 2; row <= totalRows; row++)
            {
                try
                {
                    var record = new PatientCataractQa
                    {
                        PATIENT_NAME = GetCellValue(worksheet, row, 1),
                        Mr_no = GetCellValue(worksheet, row, 2),
                        SurgeryDate = ParseDate(GetCellValue(worksheet, row, 3)),
                        Age = ParseInt(GetCellValue(worksheet, row, 4)),
                        SEX = GetCellValue(worksheet, row, 5),
                        Eye = GetCellValue(worksheet, row, 6),
                        SurgeryType = GetCellValue(worksheet, row, 7),
                        SurgeonType = GetCellValue(worksheet, row, 8),
                        Lensname = GetCellValue(worksheet, row, 9),
                        Modelname = GetCellValue(worksheet, row, 10),
                        IolPower = GetCellValue(worksheet, row, 11),
                        Anesthesia = GetCellValue(worksheet, row, 12),
                        Pre_BCVA = GetCellValue(worksheet, row, 13),
                        Pre_UCVA = GetCellValue(worksheet, row, 14),
                        FollowupBCVA = GetCellValue(worksheet, row, 15),
                        FollowupUCVA = GetCellValue(worksheet, row, 16),
                        InsertedDate = DateTime.Now
                    };

                    // Validate required fields
                    if (string.IsNullOrWhiteSpace(record.PATIENT_NAME))
                    {
                        errors.Add(new BulkUploadError { Row = row, Message = "Patient Name is required" });
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(record.Mr_no))
                    {
                        errors.Add(new BulkUploadError { Row = row, Message = "MR Number is required" });
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(record.SurgeryType))
                    {
                        errors.Add(new BulkUploadError { Row = row, Message = "Surgery Type is required" });
                        continue;
                    }

                    // Check for duplicate
                    var exists = await _context.PatientCataractData
                        .AnyAsync(x => x.Mr_no == record.Mr_no && x.SurgeryDate == record.SurgeryDate);

                    if (exists)
                    {
                        errors.Add(new BulkUploadError { Row = row, Message = "Duplicate record found" });
                        continue;
                    }

                    _context.PatientCataractData.Add(record);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    errors.Add(new BulkUploadError { Row = row, Message = ex.Message });
                }
            }

            await _context.SaveChangesAsync();
            return result;
        }

        private async Task<BulkUploadResult> ProcessGlaucomaUpload(ExcelWorksheet worksheet, List<BulkUploadError> errors)
        {
            var result = new BulkUploadResult();
            var totalRows = worksheet.Dimension?.Rows ?? 0;

            if (totalRows <= 1) // Only header row
            {
                return result;
            }

            result.TotalRows = totalRows - 1; // Exclude header

            for (int row = 2; row <= totalRows; row++)
            {
                try
                {
                    var record = new PatientGlaucomaQa
                    {
                        Patient_name = GetCellValue(worksheet, row, 1),
                        Mr_no = GetCellValue(worksheet, row, 2),
                        Surgery_date = ParseDate(GetCellValue(worksheet, row, 3)),
                        Age = ParseInt(GetCellValue(worksheet, row, 4)),
                        Sex = GetCellValue(worksheet, row, 5),
                        Eye_part = GetCellValue(worksheet, row, 6),
                        Surgery_type = GetCellValue(worksheet, row, 7),
                        Surgeon_type = GetCellValue(worksheet, row, 8),
                        Pre_operative_iop = ParseInt(GetCellValue(worksheet, row, 9)),
                        Post_operative_iop = ParseInt(GetCellValue(worksheet, row, 10)),
                        Anesthesia = GetCellValue(worksheet, row, 11),
                        Pre_bcva = GetCellValue(worksheet, row, 12),
                        Pre_ucva = GetCellValue(worksheet, row, 13),
                        Followup_bcva = GetCellValue(worksheet, row, 14),
                        Followup_ucva = GetCellValue(worksheet, row, 15),
                        Is_active = ParseBool(GetCellValue(worksheet, row, 16)) ?? true
                    };

                    // Validate required fields
                    if (string.IsNullOrWhiteSpace(record.Patient_name))
                    {
                        errors.Add(new BulkUploadError { Row = row, Message = "Patient Name is required" });
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(record.Mr_no))
                    {
                        errors.Add(new BulkUploadError { Row = row, Message = "MR Number is required" });
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(record.Surgery_type))
                    {
                        errors.Add(new BulkUploadError { Row = row, Message = "Surgery Type is required" });
                        continue;
                    }

                    // Check for duplicate
                    var exists = await _context.PatientGlaucomaData
                        .AnyAsync(x => x.Mr_no == record.Mr_no && x.Surgery_date == record.Surgery_date);

                    if (exists)
                    {
                        errors.Add(new BulkUploadError { Row = row, Message = "Duplicate record found" });
                        continue;
                    }

                    _context.PatientGlaucomaData.Add(record);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    errors.Add(new BulkUploadError { Row = row, Message = ex.Message });
                }
            }

            await _context.SaveChangesAsync();
            return result;
        }

        // Safe data reader methods
        private string? GetSafeString(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        private int? GetSafeInt(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
        }

        private DateTime? GetSafeDateTime(SqlDataReader reader, string columnName)
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
        }

        private string GetCellValue(ExcelWorksheet worksheet, int row, int col)
        {
            return worksheet.Cells[row, col]?.Value?.ToString()?.Trim() ?? string.Empty;
        }

        private DateTime? ParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            if (DateTime.TryParse(value, out var date))
                return date;

            if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return date;

            return null;
        }

        private int? ParseInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return int.TryParse(value, out var result) ? result : null;
        }

        private bool? ParseBool(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            value = value.ToLower();
            if (value == "true" || value == "1" || value == "yes" || value == "y")
                return true;
            if (value == "false" || value == "0" || value == "no" || value == "n")
                return false;

            return null;
        }

        private bool IsExcelFile(IFormFile file)
        {
            var allowedTypes = new[]
            {
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "application/vnd.ms-excel"
            };

            return allowedTypes.Contains(file.ContentType);
        }

        #endregion
    }

    #region Request/Response Models

    public class EmrConnectionRequest
    {
        public string ConnectionString { get; set; } = string.Empty;
    }

    public class EmrSyncRequest
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
    }

    public class BulkUploadResult
    {
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public int TotalRows { get; set; }
        public List<BulkUploadError>? Errors { get; set; }
    }

    public class BulkUploadError
    {
        public int Row { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    #endregion
}