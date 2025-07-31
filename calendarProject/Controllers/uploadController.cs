using calendarProject.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace calendarProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class uploadController : Controller
    {

        private readonly string _connectionString;
        private readonly string _uploadPath;


        public uploadController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _uploadPath = configuration["UploadPath"];
        }



        [HttpPost("upFile")]
        public async Task<IActionResult> UploadDocument(IFormFile file, [FromForm] DocumentDto Dto)

           
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File is empty");

                // שמירת המידע בדטה בייס וקבלת ה-docId
                int docId = await SaveDocumentInfoToDatabase(Dto.DocName!, Dto.DocDate, Dto.docEdit, Dto.docConfirmed);


                // שמירת הקובץ בשרת עם ה-docId כשם הקובץ
                string fileExtension = Path.GetExtension(file.FileName);
                string fileName = $"{docId}{fileExtension}";
                string filePath = Path.Combine(_uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // עדכון נתיב הקובץ בדטה בייס
                //await UpdateFilePathInDatabase(docId, filePath);

                return Ok(new { DocId = docId, Message = "Document uploaded successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        private async Task<int> SaveDocumentInfoToDatabase(string docName, DateTime docDate, bool docEdit, bool docConfirmed)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = @"
                INSERT INTO document (docName, docDate, docEdit, docConfirmed)
                VALUES (@docName, @docDate, @docEdit, @docConfirmed);
                SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@docName", docName);
                    command.Parameters.AddWithValue("@docDate", docDate);
                    command.Parameters.AddWithValue("@docEdit", docEdit);
                    command.Parameters.AddWithValue("@docConfirmed",docConfirmed);

                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        //private async Task UpdateFilePathInDatabase(int docId, string filePath)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();

        //        string sql = "UPDATE document SET FilePath = @FilePath WHERE docId = @DocId";

        //        using (var command = new SqlCommand(sql, connection))
        //        {
        //            command.Parameters.AddWithValue("@FilePath", filePath);
        //            command.Parameters.AddWithValue("@DocId", docId);

        //            await command.ExecuteNonQueryAsync();
        //        }
        //    }
        //}


        //private readonly IWebHostEnvironment _environment;

        //public uploadController(IWebHostEnvironment environment)
        //{
        //    _environment = environment;
        //}

        //[HttpPost]
        //public async Task<IActionResult> Upload(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("File is empty");

        //    try
        //    {
        //        string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
        //        if (!Directory.Exists(uploadsFolder))
        //            Directory.CreateDirectory(uploadsFolder);

        //        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        //        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(fileStream);
        //        }

        //        return Ok(new { fileName = uniqueFileName });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex}");
        //    }
        //}


    }


    //public class DocumentDto
    //{
    //    public string DocName { get; set; }
    //    public DateTime DocDate { get; set; }

    //    public bool docEdit { get; set; }
    //    public bool docConfirmed { get; set; }
    //}

}
