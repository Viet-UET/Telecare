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
    // Mặc định: 8 BV, 25 BS/BV => 200 bác sĩ; 1000 bệnh nhân; 2000 cuộc hẹn gọi; 1200 hẹn BV
    public static async Task SeedAsync(
        ApplicationDBContext db,
        int nHospitals = 8,
        int doctorsPerHospital = 25,
        int nPatients = 1000,
        int nCallAppointments = 2000,
        int nHospitalAppointments = 1200)
    {
        // Nếu DB đã có User thì coi như đã seed trước đó -> bỏ qua
        if (await db.Users.AnyAsync()) return;

        var rnd = new Random(42);
        var fakerVi = new Faker("vi");
        var faker = new Faker();

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

        // 2) Tạo bệnh viện + user (Role.HOSPITAL)
        var hospitals = new List<Hospital>();
        for (int i = 0; i < nHospitals; i++)
        {
            var email = $"hospital{i + 1}@example.com";
            var password = "Password123!";
            var hash = BCrypt.Net.BCrypt.HashPassword(email + password); // theo logic AccountController

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
                    Sex = (Sex)rnd.Next(0, 3),
                    Phone = fakerVi.Phone.PhoneNumber("0#########"),
                    Degree = faker.PickRandom(new[] { "BS.CK I", "BS.CK II", "ThS.BS", "BS" }),
                    ConsultingPriceViaMessage = rnd.Next(50_000, 150_000),
                    ConsultingPriceViaCall = rnd.Next(150_000, 500_000),
                    Point = Math.Round(3 + rnd.NextDouble() * 2, 2)
                };

                doctors.Add(doc);
            }
        }
        await db.Doctors.AddRangeAsync(doctors);
        await db.SaveChangesAsync();

        // 4) Tạo bệnh nhân (Role.PATIENT) ~ 1000
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

        // Helper tạo thời gian ngẫu nhiên quanh hiện tại (UTC) ±45 ngày
        DateTime RandomTimeUtc()
        {
            // 60% tương lai, 40% quá khứ
            bool future = rnd.NextDouble() < 0.6;
            int minutes = rnd.Next(30, 60 * 24 * 45); // 30' đến 45 ngày
            return future ? DateTime.UtcNow.AddMinutes(minutes) : DateTime.UtcNow.AddMinutes(-minutes);
        }

        // 5) Tạo 2000 lịch hẹn gọi (CallAppointment)
        // Lưu ý: Không set State để dùng giá trị mặc định của enum (đã cấu hình conversion trong DbContext)
        var callAppointments = new List<CallAppointment>(nCallAppointments);
        for (int i = 0; i < nCallAppointments; i++)
        {
            var p = patients[rnd.Next(patients.Count)];
            var d = doctors[rnd.Next(doctors.Count)];

            var appt = new CallAppointment
            {
                Patient = p,
                Doctor = d,
                Time = RandomTimeUtc()
            };
            callAppointments.Add(appt);
        }
        await db.CallAppointments.AddRangeAsync(callAppointments);
        await db.SaveChangesAsync();

        // 6) Tạo 1200 lịch hẹn tại bệnh viện (HospitalAppointment)
        var hospitalAppointments = new List<HospitalAppointment>(nHospitalAppointments);
        for (int i = 0; i < nHospitalAppointments; i++)
        {
            var p = patients[rnd.Next(patients.Count)];
            var h = hospitals[rnd.Next(hospitals.Count)];

            var appt = new HospitalAppointment
            {
                Patient = p,
                Hospital = h,
                Time = RandomTimeUtc()
            };
            hospitalAppointments.Add(appt);
        }
        await db.hospitalAppointments.AddRangeAsync(hospitalAppointments); // DbSet: hospitalAppointments
        await db.SaveChangesAsync();
    }
}
