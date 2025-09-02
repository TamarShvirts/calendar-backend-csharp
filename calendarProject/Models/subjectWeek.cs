using System.ComponentModel.DataAnnotations;

namespace calendarProject.Models
{
    public class subjectWeek
    {
        
            [Key]
            public int subId { get; set; }
            public string subName { get; set; }

            public DateTime stratDateWeek { get; set; }

            public DateTime lastDateWeek { get; set; }

        
    }
}
