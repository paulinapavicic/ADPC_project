using CsvHelper.TypeConversion;
using Microsoft.EntityFrameworkCore;
using Project_1.Models;

namespace Project_1.Data
{
    public class MedicalDbContext : DbContext
    {
        public MedicalDbContext(DbContextOptions<MedicalDbContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Checkup> Checkups { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           


            modelBuilder.Entity<Patient>().HasKey(p => p.PatientId);
            modelBuilder.Entity<MedicalRecord>().HasKey(m => m.RecordId);
            modelBuilder.Entity<Checkup>().HasKey(c => c.CheckupId);
            modelBuilder.Entity<Image>().HasKey(i => i.ImageId);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(m => m.PatientId);

            modelBuilder.Entity<Checkup>()
                .HasOne(c => c.Patient)
                .WithMany(p => p.Checkups)
                .HasForeignKey(c => c.PatientId);

            modelBuilder.Entity<Image>()
                .HasOne(i => i.Checkup)
                .WithMany(c => c.Images)
                .HasForeignKey(i => i.CheckupId);
        }
    }
}
