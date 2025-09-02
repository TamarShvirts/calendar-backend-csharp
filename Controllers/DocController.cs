using calendarProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;


namespace calendarProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocController : Controller
    {
        private readonly connectionDB _context;
        private readonly string _uploadPath;

        public DocController(connectionDB context, IConfiguration configuration)
        {
            _context = context;
            _uploadPath = configuration["UploadPath"];
        }

        [HttpPost("CreateByDate")]
        public async Task<IActionResult> CreateDocumentByDate([FromBody] DocumentDto dto)
        {
            try
            {
                var newDocument = new document
                {
                    docName = dto.DocName,
                    docDate = dto.DocDate,
                    docEdit= dto.docEdit,
                    docConfirmed= dto.docConfirmed
                    //docStatus = status.processing
                };


                Console.WriteLine(newDocument+"hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhyyy");
                _context.document.Add(newDocument);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(CreateDocumentByDate), new { id = newDocument.docId }, newDocument);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }


   //שליפת הקבצים להצגתם בטבלה

        [HttpGet("byDate")]
        public async Task<ActionResult<document>> GetdocumentsByDate([FromQuery] DateTime docDate)
        {
            try
            {
                Debug.WriteLine("ggggg"+docDate);

                var docs = await _context.document
                .Where(s => s.docDate == docDate)
                 .ToListAsync();


                //var subjects = await _context.SubjectWeek
                //    .Where(s => s.StartDateWeek.Date == startDate.Date)
                //    .ToListAsync();


                //var subject = await _context.SubjectWeek
                //    .Include(s => s.RelatedEntity)
                //    .FirstOrDefaultAsync(s => s.StartDateWeek.Date == startDate.Date);


                if (docs == null)
                {
                    return NotFound($"No subject found starting on {docDate.ToShortDateString()}");
                }

                return Ok(docs) ;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while fetching docs: {ex.Message}");
                //_logger.LogError(ex, $"Error occurred while fetching subject for date {startDate}");
                return StatusCode(500, "An error occurred while fetching the data.");
            }
        }



 
        //מחיקת הקובץ מהדטה בייס ומהשרת
        [HttpDelete("deleteFile/{docId}")]
        public async Task<IActionResult> DeleteDocument(int docId)
        {
            try
            {
                // מחיקת המידע מהדטה בייס
                await DeleteDocumentInfoFromDatabase(docId);

                // מציאת הקובץ בשרת ומחיקתו
                string[] files = Directory.GetFiles(_uploadPath, $"{docId}.*");
                if (files.Length > 0)
                {
                    System.IO.File.Delete(files[0]);
                }

                return Ok(new { Message = "Document deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        private async Task<ActionResult> DeleteDocumentInfoFromDatabase(int docId)
        {
            try
            {
                var docToDelete = _context.document
                    .Where(s => s.docId == docId)
                    .ToList();

                if (docToDelete.Count == 0)
                {
                    return NotFound($"No doc found starting on {docId}");
                }

                _context.document.RemoveRange(docToDelete);
                await _context.SaveChangesAsync();

                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while deleting doc: {ex.Message}");
                //_logger.LogError(ex, $"Error occurred while deleting subjects for date {startDate}");
                return StatusCode(500, "An error occurred while deleting the data.");
            }
        }

        //עדכון סטטוס הקובץ
        [HttpPost("updateDocStatus")]
        public async Task<ActionResult> UpdateDocumentStatus([FromBody] DocumentStatusUpdateModel model)
        {

            try
            {
                var documentToUpdate = await _context.document
                    .FirstOrDefaultAsync(s => s.docId == model.docId);


                if (documentToUpdate == null)
                {
                    return NotFound($"No document found with ID {model.docId}");
                }
                else
                {
                   

                    documentToUpdate.docEdit = model.docEdit;
                    documentToUpdate.docConfirmed = model.docConfirmed;
                    await _context.SaveChangesAsync();


                    return Ok(documentToUpdate); // 200 OK with the updated subject
                }

                }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the document status.");
            }

        }

        //שליפת הקובץ מהשרת לפי ID
        [HttpGet("getFile/{docId}")]
        public async Task<IActionResult> GetDocument(int docId)
        {
            try
            {
                // מציאת פרטי המסמך בדטה בייס
                var document = await _context.document
                    .FirstOrDefaultAsync(d => d.docId == docId);

                if (document == null)
                    return NotFound($"Document with ID {docId} not found");

                // בניית נתיב הקובץ
                string fileExtension = Path.GetExtension(document.docName);
                string fileName = $"{docId}{fileExtension}";
                string filePath = Path.Combine(_uploadPath, fileName);

                // בדיקה שהקובץ קיים
                if (!System.IO.File.Exists(filePath))
                    return NotFound($"File not found for document {docId}");

                // קריאת סוג MIME של הקובץ
                string contentType = GetContentType(fileExtension);

                // החזרת הקובץ
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return File(fileStream, contentType, document.docName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        private string GetContentType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".pdf":
                    return "application/pdf";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                // הוסיפי סוגי קבצים נוספים לפי הצורך
                default:
                    return "application/octet-stream";
            }
        }


        //מחזיר מערך של כמות קבצים לא ערוכים או/ו לא מבוקרים לפי תאריך

        [HttpGet("CuontStatus")]

        public async Task<ActionResult<document>> getCuontStatusDocByDate([FromQuery] DateTime docDate)
        {
            try
            {
                Debug.WriteLine("ggggg" + docDate);
                var filteredDocuments = _context.document
                .Where(d => d.docDate <= docDate)
                .ToList();

               var result= new DocumentStatusResult
                {
                    FalseEditAndConfirmedCount = filteredDocuments.Count(d => !d.docEdit && !d.docConfirmed),
                    FalseEditOnlyCount = filteredDocuments.Count(d => !d.docEdit && d.docConfirmed),
                    FalseConfirmedOnlyCount = filteredDocuments.Count(d => d.docEdit && !d.docConfirmed)
                };
                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }



    }
    public class DocumentDto
    {
        public string? DocName { get; set; }
        public DateTime DocDate { get; set; }

        public Boolean docEdit { get; set; }
        public Boolean docConfirmed { get; set; }
    }

    public class DocumentStatusUpdateModel
    {
        public int docId { get; set; }
        public Boolean docEdit { get; set; }
        public Boolean docConfirmed { get; set; }
    }

    public class DocumentStatusResult
    {
        public int FalseEditAndConfirmedCount { get; set; }
        public int FalseEditOnlyCount { get; set; }
        public int FalseConfirmedOnlyCount { get; set; }
    }
}

