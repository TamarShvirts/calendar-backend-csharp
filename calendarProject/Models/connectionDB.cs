
using Microsoft.EntityFrameworkCore;

namespace calendarProject.Models
{
    public class connectionDB : DbContext
    {



        public connectionDB(DbContextOptions<connectionDB> options)
            : base(options)
        { }
        public DbSet<document> document { get; set; }
        public DbSet<subjectWeek> subjectWeek { get; set; }
        public DbSet<comment> comment { get; set; }
        public DbSet<users> users { get; set; }


    }
}
