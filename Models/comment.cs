using System.ComponentModel.DataAnnotations;

namespace calendarProject.Models
{
    public class comment
    {
        [Key]
        public int commentId { get; set; }
        public string? comments { get; set; }
        public DateTime commentDate { get; set; }
        public DateTime dateByCalendar { get; set; }
        public string? userName { get; set; }


    }
}



