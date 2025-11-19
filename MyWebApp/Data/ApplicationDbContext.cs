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
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<Review> Reviews { get; set; }

        // Эти строки нужны, чтобы программа знала о новых таблицах
        public DbSet<Employee> Employees { get; set; }
        public DbSet<MedicalExam> MedicalExams { get; set; }
    }
}