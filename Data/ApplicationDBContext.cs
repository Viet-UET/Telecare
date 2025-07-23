

using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {

        }

        // Khai báo các bảng
        DbSet<User> Users { get; set; }
        DbSet<Patient> Patients { get; set; }
        DbSet<Doctor> Doctors { get; set; }
        DbSet<Hospital> Hospitals { get; set; }
        DbSet<MedicalSpecialty> MedicalSpecialties { get; set; }
        DbSet<Token> Tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
                {
                    entity.ToTable("User");
                    entity.HasKey(e => e.UserId);
                    entity.Property(e => e.UserId).ValueGeneratedOnAdd();
                    entity.Property(e => e.Email).IsRequired().HasMaxLength(128);
                    entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(128);
                    entity.Property(e => e.GoogleId).HasMaxLength(128).IsRequired(false);
                    entity.Property(e => e.Img).HasMaxLength(256).IsRequired(false);

                    // Enum to string conversion for Role
                    entity.Property(e => e.Role)
                        .IsRequired()
                        .HasConversion<string>()
                        .HasMaxLength(255);

                    // Unique index for Email
                    entity.HasIndex(e => e.Email, "User_index_0").IsUnique();

                    // Relationships
                    entity.HasOne(u => u.Patient).WithOne(p => p.User).HasForeignKey<Patient>(p => p.UserId);
                    entity.HasOne(u => u.Hospital).WithOne(h => h.User).HasForeignKey<Hospital>(h => h.UserId);
                    entity.HasOne(u => u.Doctor).WithOne(d => d.User).HasForeignKey<Doctor>(d => d.UserId);
                });



            // Patient Configuration
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("Patient");
                entity.HasKey(e => e.PatientId);
                entity.Property(e => e.PatientId).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20).IsRequired(false);
                entity.Property(e => e.Address).HasColumnType("TEXT").IsRequired(false);

                // Enum to string conversion for Sex
                entity.Property(e => e.Sex)
                      .IsRequired(false) // Default is null
                      .HasConversion<string>()
                      .HasMaxLength(255);
            });




            // Hospital Configuration
            modelBuilder.Entity<Hospital>(entity =>
            {
                entity.ToTable("Hospital");
                entity.HasKey(e => e.HospitalId);
                entity.Property(e => e.HospitalId).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Address).HasColumnType("TEXT").IsRequired(false);
                entity.Property(e => e.WorkingTime).HasColumnType("TEXT").IsRequired(false);
            });





            // MedicalSpecialty Configuration
            modelBuilder.Entity<MedicalSpecialty>(entity =>
            {
                entity.ToTable("MedicalSpecialty");
                entity.HasKey(e => e.MedicalSpecialtyId);
                entity.Property(e => e.MedicalSpecialtyId).ValueGeneratedOnAdd();
                entity.Property(e => e.EnglishName).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.VietnameseName).HasMaxLength(100).IsRequired(false);
            });





            // Doctor Configuration
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.ToTable("Doctor");
                entity.HasKey(e => e.DoctorId);
                entity.Property(e => e.DoctorId).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20).IsRequired(false);
                entity.Property(e => e.Degree).HasMaxLength(300).IsRequired(false);

                // Default values
                entity.Property(e => e.ConsultingPriceViaMessage).HasDefaultValue(0.0);
                entity.Property(e => e.ConsultingPriceViaCall).HasDefaultValue(0.0);

                // Enum to string conversion for Sex
                entity.Property(e => e.Sex)
                      .IsRequired(false) // Default is null
                      .HasConversion<string>()
                      .HasMaxLength(255);

                // Relationships
                entity.HasOne(d => d.MedicalSpecialty)
                      .WithMany(ms => ms.Doctors)
                      .HasForeignKey(d => d.MedicalSpecialtyId)
                      .OnDelete(DeleteBehavior.Cascade); // ON DELETE CASCADE

                entity.HasOne(d => d.Hospital)
                      .WithMany(h => h.Doctors)
                      .HasForeignKey(d => d.HospitalId)
                      .OnDelete(DeleteBehavior.Cascade); // ON DELETE CASCADE

                // Relationship with User already configured via User entity, but cascade delete can be set here too
                entity.HasOne(d => d.User)
                      .WithOne(u => u.Doctor)
                      .HasForeignKey<Doctor>(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // ON DELETE CASCADE
            });





            // Token Configuration
            modelBuilder.Entity<Token>(entity =>
            {
                entity.ToTable("Token");
                entity.HasKey(e => e.TokenValue); // Primary Key
                entity.Property(e => e.TokenValue).HasColumnName("token").HasMaxLength(300);

                // Relationship
                entity.HasOne(t => t.User)
                      .WithMany(u => u.Tokens)
                      .HasForeignKey(t => t.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });


        }
    }
}