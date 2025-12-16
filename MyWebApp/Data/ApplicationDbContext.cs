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

            modelBuilder.Entity<EavEntity>()
                .HasMany(e => e.Attributes)
                .WithOne(a => a.EavEntity)
                .HasForeignKey(a => a.EavEntityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EavAttribute>()
                .HasOne(a => a.LinkedEntity)
                .WithMany()
                .HasForeignKey(a => a.LinkedEntityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EavEntity>()
                .HasMany(e => e.Records)
                .WithOne(r => r.EavEntity)
                .HasForeignKey(r => r.EavEntityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EavRecord>()
                .HasMany(r => r.Values)
                .WithOne(v => v.EavRecord)
                .HasForeignKey(v => v.EavRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EavValue>()
                .HasOne(v => v.EavAttribute)
                .WithMany()
                .HasForeignKey(v => v.EavAttributeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EavValue>()
                .HasOne(v => v.LinkedRecord)
                .WithMany()
                .HasForeignKey(v => v.LinkedRecordId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}