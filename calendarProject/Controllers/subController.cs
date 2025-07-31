using calendarProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;




namespace calendarProject.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class subController : ControllerBase
    {

        private readonly connectionDB _context;



        public subController(connectionDB context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<subjectWeek>>> GetSub()
        {
            var subjects = await _context.subjectWeek.ToListAsync();
            return Ok(subjects);
        }


        // שליפה לפי תאריך
        [HttpGet("byDate")]

        public async Task<ActionResult<subjectWeek>> GetSubjectByStartDate([FromQuery] DateTime stratDateWeek)
        {
            try
            {
                var subject = await _context.subjectWeek
                    .FirstOrDefaultAsync(s => s.stratDateWeek.Date == stratDateWeek.Date);


                //var subjects = await _context.SubjectWeek
                //    .Where(s => s.StartDateWeek.Date == startDate.Date)
                //    .ToListAsync();


                //var subject = await _context.SubjectWeek
                //    .Include(s => s.RelatedEntity)
                //    .FirstOrDefaultAsync(s => s.StartDateWeek.Date == startDate.Date);


                //if (subject == null)
                //{
                //    return NotFound($"No subject found starting on {stratDateWeek.ToShortDateString()}");
                //}

                return Ok(subject);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while fetching subjects: {ex.Message}");
                //_logger.LogError(ex, $"Error occurred while fetching subject for date {startDate}");
                return StatusCode(500, "An error occurred while fetching the data.");
            }
        }

        //הכנסת נושא חדש לטבלה
        [HttpPost]
        public async Task<ActionResult<subjectWeek>> Createsub([FromBody] subjectWeek subjectWeek)
        {
            _context.subjectWeek.Add(subjectWeek);
            await _context.SaveChangesAsync();



            return CreatedAtAction("GetUser", new { id = subjectWeek.subId }, subjectWeek);
        }

        //מחיקת נתון מהטבלה לפי תאריך
        [HttpDelete("DeletebyDate")]
        public async Task<ActionResult> DeleteSubjectByStartDate([FromQuery] DateTime startDate)
        {
            try
            {
                var subjectsToDelete = _context.subjectWeek
                    .Where(s => s.stratDateWeek.Date == startDate.Date)
                    .ToList();

                if (subjectsToDelete.Count == 0)
                {
                    return NotFound($"No subjects found starting on {startDate.ToShortDateString()}");
                }

                _context.subjectWeek.RemoveRange(subjectsToDelete);
                await _context.SaveChangesAsync();

                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while deleting subjects: {ex.Message}");
                //_logger.LogError(ex, $"Error occurred while deleting subjects for date {startDate}");
                return StatusCode(500, "An error occurred while deleting the data.");
            }
        }

        //עדכון נושא שבועי לפי תאריך
        [HttpPost("UpdatebyDate")] 
        public async Task<ActionResult> UpdateSubjectByStartDate([FromBody] subjectWeek subjectWeek)
        {

            try
            {
                var subjectToUpdate = await _context.subjectWeek
                    .FirstOrDefaultAsync(s => s.stratDateWeek.Date == subjectWeek.stratDateWeek);

                if (subjectToUpdate == null)
                {
                    _context.subjectWeek.Add(subjectWeek);
                    await _context.SaveChangesAsync();

                    return Ok(subjectWeek); // 200 OK with the newly added subject
                }
                else
                {
                    subjectToUpdate.subName = subjectWeek.subName;
                    await _context.SaveChangesAsync();

                    return Ok(subjectToUpdate); // 200 OK with the updated subject
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while updating subjects: {ex.Message}");
                //_logger.LogError(ex, $"Error occurred while updating subjects for date {startDate}");
                return StatusCode(500, "An error occurred while updating the data.");
            }

        }



    }
}








   



 

