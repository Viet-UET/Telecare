

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

        DbSet<User> Users { get; set; }
        DbSet<Patient> Patients { get; set; }
        DbSet<Doctor> Doctors { get; set; }
        DbSet<Hospital> Hospitals { get; set; }
        DbSet<MedicalSpecialty> MedicalSpecialties { get; set; }
        DbSet<Token> Tokens { get; set; }

        
    }
}