using System.ComponentModel.DataAnnotations;

namespace calendarProject.Models
{
    
    public class document
    {
        [Key]
        public int docId { get; set; }
        public string? docName { get; set; }

        public DateTime docDate { get; set; }

        public Boolean docEdit { get; set; }
        public Boolean docConfirmed { get; set; }
    }

}
