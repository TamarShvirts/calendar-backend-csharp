using Microsoft.AspNetCore.Mvc;
using calendarProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace calendarProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class commentController : Controller
    {

        private readonly connectionDB _context;

        public commentController(connectionDB context)
        {
            _context = context;
        }


        /*   [HttpGet("byDate")]
           public async Task<ActionResult<IEnumerable<subjectWeek>>> GetSub()
           {

           }
        */

        [HttpGet]
        public async Task<ActionResult<IEnumerable<comment>>> Getcomment()
        {
            var comments = await _context.comment.ToListAsync();
            return Ok(comments);
        }

        //הכנסת הערה לטבלה

        [HttpPost("UpdateComment")]
        public async Task<IActionResult> Createcomment([FromBody] comment comment)
        {
            _context.comment.Add(comment);
            await _context.SaveChangesAsync();



            return Ok(comment);
        }




        [HttpGet("getCommentsByDate")]
        public async Task<ActionResult<comment>> getCommentsByDate([FromQuery] DateTime CommentsDate)
        {
            try
            {

                var comments = await _context.comment
                .Where(s => s.dateByCalendar == CommentsDate)
                 .ToListAsync();


                if (comments == null)
                {
                    return NotFound($"No subject found starting on {CommentsDate.ToShortDateString()}");
                }

                return Ok(comments);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while fetching comments: {ex.Message}");
                //_logger.LogError(ex, $"Error occurred while fetching subject for date {startDate}");
                return StatusCode(500, "An error occurred while fetching the data.");
            }
        }



        //מחיקת הערה מהדטה בייס
        [HttpDelete("deleteComment/{commentId}")]
        public async Task<IActionResult> deleteComment(int commentId)
        {
            try
            {
                
                var commentToDelete = _context.comment
                   .Where(s => s.commentId == commentId)
                   .ToList();

                if (commentToDelete.Count == 0)
                {
                    return NotFound($"No doc found starting on {commentId}");
                }

                _context.comment.RemoveRange(commentToDelete);
                await _context.SaveChangesAsync();

                return NoContent(); // 204 No Content
                return Ok(new { Message = "comment deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }



           


    }
}
