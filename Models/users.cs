using System.ComponentModel.DataAnnotations;

namespace calendarProject.Models
{
    public class users
    {

        [Key]
        public int userNameId { get; set; }
        public string? userName { get; set; }
        public string? password { get; set; }
    }
}
