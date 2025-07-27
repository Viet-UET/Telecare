using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;
using BCrypt.Net;
using DTOs;


namespace Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public AccountController(ApplicationDBContext context)
        {
            _context = context;
        }


        public async Task<string> IsUserRegistered(string email)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return "not registered";
            }
            return "registered";
        }

        public async Task<ActionResult<User>> CreateUser(string email, string password, Role role)
        {
            var existUser = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (existUser != null)
            {
                return Conflict("Email đã tồn tại");
            }
            else
            {
                var ep = email + password;
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(ep);
                var newUser = new User
                {
                    Email = email,
                    PasswordHash = passwordHash,
                    Role = role,
                };
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();
                return newUser;
            }
        }

        [HttpPost("create-patient")]
        public async Task<ActionResult> CreatePatient(CreatePatientDTO createPatientDTO)
        {

            var check = await IsUserRegistered(createPatientDTO.Email);
            if (check == "registered")
            {
                return Conflict("Email already registered");
            }
            else
            {
                var newUser = await CreateUser(createPatientDTO.Email, createPatientDTO.Password, Role.PATIENT);
                var newP = new Patient
                {
                    UserId = newUser.Value.UserId,
                    Name = createPatientDTO.Name,
                    Age = createPatientDTO.Age,
                    Sex = createPatientDTO.Sex,
                    Phone = createPatientDTO.Phone,
                    Address = createPatientDTO.Address
                };
                await _context.Patients.AddAsync(newP);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Patient created successfully" });

            }
        }





    }
}