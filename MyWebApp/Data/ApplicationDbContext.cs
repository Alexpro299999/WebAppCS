using Microsoft.EntityFrameworkCore;
using MyWebApp.Models;

namespace MyWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<MedicalExam> MedicalExams { get; set; }

        public DbSet<EavEntity> EavEntities { get; set; }
        public DbSet<EavAttribute> EavAttributes { get; set; }
        public DbSet<EavRecord> EavRecords { get; set; }
        public DbSet<EavValue> EavValues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}