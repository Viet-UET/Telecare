using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;

public static class DbSeeder
{
    // Mặc định: 5 BV, 20 BS/BV => 100 bác sĩ; 50 bệnh nhân
    public static async Task SeedAsync(
        ApplicationDBContext db,
        int nHospitals = 5,
        int doctorsPerHospital = 20,
        int nPatients = 50)
    {
        // Nếu đã có user thì coi như đã seed -> bỏ qua
        if (await db.Users.AnyAsync()) return;

        // 1) Chuyên khoa mẫu
        var specialties = new[]
        {
            new MedicalSpecialty { EnglishName = "Cardiology",     VietnameseName = "Tim mạch" },
            new MedicalSpecialty { EnglishName = "Dermatology",    VietnameseName = "Da liễu" },
            new MedicalSpecialty { EnglishName = "Pediatrics",     VietnameseName = "Nhi khoa" },
            new MedicalSpecialty { EnglishName = "Neurology",      VietnameseName = "Thần kinh" },
            new MedicalSpecialty { EnglishName = "Orthopedics",    VietnameseName = "Chấn thương chỉnh hình" },
            new MedicalSpecialty { EnglishName = "Oncology",       VietnameseName = "Ung bướu" },
        };
        await db.MedicalSpecialties.AddRangeAsync(specialties);
        await db.SaveChangesAsync();

        var rnd = new Random(42);
        var fakerVi = new Faker("vi");

        // 2) Tạo bệnh viện + user (Role.HOSPITAL)
        var hospitals = new List<Hospital>();
        for (int i = 0; i < nHospitals; i++)
        {
            var email = $"hospital{i + 1}@example.com";
            var password = "Password123!"; // đăng nhập test
            var hash = BCrypt.Net.BCrypt.HashPassword(email + password); // ĐÚNG theo logic AccountController

            var user = new User
            {
                Email = email,
                PasswordHash = hash,
                Role = Role.HOSPITAL
            };

            var hospital = new Hospital
            {
                User = user,
                Name = $"Bệnh viện {i + 1}",
                Address = fakerVi.Address.FullAddress(),
                WorkingTime = "T2–T7: 08:00–17:00",
                Point = Math.Round(rnd.NextDouble() * 5, 2)
            };
            hospitals.Add(hospital);
        }
        await db.Hospitals.AddRangeAsync(hospitals);
        await db.SaveChangesAsync();

        // 3) Tạo bác sĩ (Role.DOCTOR)
        var doctors = new List<Doctor>();
        foreach (var h in hospitals)
        {
            for (int j = 0; j < doctorsPerHospital; j++)
            {
                var fullName = fakerVi.Name.FullName();
                var email = $"dr_{h.HospitalId}_{j + 1}@example.com";
                var password = "Password123!";
                var hash = BCrypt.Net.BCrypt.HashPassword(email + password);

                var user = new User
                {
                    Email = email,
                    PasswordHash = hash,
                    Role = Role.DOCTOR
                };

                var spec = specialties[rnd.Next(specialties.Length)];

                var doc = new Doctor
                {
                    User = user,
                    Hospital = h,
                    MedicalSpecialty = spec,
                    Name = fullName,
                    Age = rnd.Next(28, 60),
                    Sex = (Sex)rnd.Next(0, 3), // MALE/FEMALE/NOT_MENTION
                    Phone = fakerVi.Phone.PhoneNumber("0#########"),
                    Degree = "BS.CKI",
                    ConsultingPriceViaMessage = rnd.Next(50_000, 150_000),
                    ConsultingPriceViaCall = rnd.Next(150_000, 500_000),
                    Point = Math.Round(3 + rnd.NextDouble() * 2, 2) // 3.00–5.00
                };

                doctors.Add(doc);
            }
        }
        await db.Doctors.AddRangeAsync(doctors);
        await db.SaveChangesAsync();

        // 4) Tạo bệnh nhân (Role.PATIENT)
        var patients = new List<Patient>();
        for (int i = 0; i < nPatients; i++)
        {
            var fullName = fakerVi.Name.FullName();
            var email = $"pt_{i + 1}@example.com";
            var password = "Password123!";
            var hash = BCrypt.Net.BCrypt.HashPassword(email + password);

            var user = new User
            {
                Email = email,
                PasswordHash = hash,
                Role = Role.PATIENT
            };

            var p = new Patient
            {
                User = user,
                Name = fullName,
                Age = rnd.Next(1, 90),
                Sex = (Sex)rnd.Next(0, 3),
                Phone = fakerVi.Phone.PhoneNumber("0#########"),
                Address = fakerVi.Address.FullAddress()
            };
            patients.Add(p);
        }
        await db.Patients.AddRangeAsync(patients);
        await db.SaveChangesAsync();
    }
}
