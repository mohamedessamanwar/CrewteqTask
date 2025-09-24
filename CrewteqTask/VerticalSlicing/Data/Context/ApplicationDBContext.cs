using Microsoft.EntityFrameworkCore;


namespace CrewteqTask.VerticalSlicing.Data.Context
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public DbSet<Entities.Employee> Employees { get; set; }
    }
}
