using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication4.Domain.Models;

namespace WebApplication4.Infrastructure.DB
{
    public class ApplicationDbContext : IdentityDbContext<Pharmacist>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PatientFeedback> PatientsFeedback { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
       
        public DbSet<PrescriptionUpload> PrescriptionsUpload { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            modelBuilder.Entity<PrescriptionUpload>().HasIndex(p => p.UploadedAt);
            modelBuilder.Entity<PatientFeedback>().HasIndex(p => p.CreatedAt);

            modelBuilder.Entity<Medicine>()
                .HasOne(m => m.Inventory)
                .WithOne(i => i.Medicine)
                .HasForeignKey<Inventory>(i => i.MedicineId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Medicine>()
                .Property(m => m.Price)
                .HasPrecision(18, 2);


            modelBuilder.Entity<Category>()
                .HasMany(c => c.Medicines)
                .WithOne(m => m.Category)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.Medicines)
                .WithOne(m => m.Supplier)
                .HasForeignKey(m => m.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.Medicines)
                .WithOne(o => o.Supplier)
                .HasForeignKey(o => o.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);


          
        }



    }
}
