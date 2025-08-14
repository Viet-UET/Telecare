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
using Microsoft.VisualBasic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System.Xml.Schema;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.AspNetCore.StaticFiles;


namespace Controllers

{
    // [ApiController]
    // [Route("[controller]")]
    public class AccountController : ControllerBase
    {

        private readonly IWebHostEnvironment _env;

        private const string JWT_SECRET_KEY = "hoangsaongocxamhathanhmocaibatajmanasdaueirawbiefonokfnenjdeckn";

        private readonly ApplicationDBContext _context;
        public AccountController(ApplicationDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        private async Task<string> IsUserRegistered(string email)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return "not registered";
            }
            return "registered";
        }

        private async Task<ActionResult<User>> CreateUser(string email, string password, Role role)
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

            // Validate required fields
            if (string.IsNullOrEmpty(createPatientDTO.Email) ||
                string.IsNullOrEmpty(createPatientDTO.Password) ||
                string.IsNullOrEmpty(createPatientDTO.Name) ||
                string.IsNullOrEmpty(createPatientDTO.Phone) ||
                string.IsNullOrEmpty(createPatientDTO.Address))
            {
                return BadRequest("Required fields cannot be empty");
            }


            var check = await IsUserRegistered(createPatientDTO.Email);
            if (check == "registered")
            {
                return Conflict("Email already registered");
            }
            else
            {
                var newUserResult = await CreateUser(createPatientDTO.Email, createPatientDTO.Password, Role.PATIENT);
                if (newUserResult.Value == null)
                {
                    return BadRequest("Failed to create user");
                }
                var newP = new Patient
                {
                    UserId = newUserResult.Value.UserId,
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


        [HttpPost("create-hospital")]
        public async Task<ActionResult> CreateHospital(CreateHospitalDTO createHospitalDTO)
        {

            // Validate input
            if (string.IsNullOrEmpty(createHospitalDTO.Email) ||
                string.IsNullOrEmpty(createHospitalDTO.Password) ||
                string.IsNullOrEmpty(createHospitalDTO.Name) ||
                string.IsNullOrEmpty(createHospitalDTO.Address))
            {
                return BadRequest("Required fields cannot be empty");
            }

            var check = await IsUserRegistered(createHospitalDTO.Email);
            if (check == "registered")
            {
                return Conflict("Email already registered");
            }
            else
            {
                var newUserResult = await CreateUser(createHospitalDTO.Email, createHospitalDTO.Password, Role.HOSPITAL);
                if (newUserResult.Value == null)
                {
                    return BadRequest("Failed to create user");
                }
                var newH = new Hospital
                {
                    UserId = newUserResult.Value.UserId,
                    Name = createHospitalDTO.Name,
                    Address = createHospitalDTO.Address,
                    WorkingTime = createHospitalDTO.WorkingTime
                };
                await _context.Hospitals.AddAsync(newH);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Hospital created successfully" });

            }
        }


        [HttpPost("create_medicalspecialty")]
        public async Task<ActionResult<string>> CreateMedicalSpecialty(CreateMedicalSpecialtyDTO createMedicalSpecialtyDTO)
        {
            // Check if the medical specialty already exists by comparing both English and Vietnamese names
            var existingSpecialty = await _context.MedicalSpecialties
                .FirstOrDefaultAsync(ms =>
                    ms.EnglishName.ToLower() == createMedicalSpecialtyDTO.EnglishName.ToLower() ||
                    ms.VietnameseName.ToLower() == createMedicalSpecialtyDTO.VietnameseName.ToLower()
                );
            if (existingSpecialty != null)
            {
                return BadRequest("Medical specialty already exists");
            }

            if (createMedicalSpecialtyDTO.createrRole == Role.PATIENT || createMedicalSpecialtyDTO.createrRole == Role.DOCTOR)
            {
                return Forbid("You do not have permission to create a medical specialty");
            }

            var newMedSpe = new MedicalSpecialty
            {
                EnglishName = createMedicalSpecialtyDTO.EnglishName,
                VietnameseName = createMedicalSpecialtyDTO.VietnameseName,
            };

            await _context.MedicalSpecialties.AddAsync(newMedSpe);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Medical specialty created successfully" });


        }


        [HttpPost("create_doctor")]
        public async Task<ActionResult<string>> CreateDoctor(CreateDoctorDTO createDoctorDTO)
        {
            // Validate the creator's role to ensure they have the necessary permissions
            // Verify that the specified hospital exists in the system
            // Confirm that the specified medical specialty exists in the system
            // Ensure that the doctor is being created for the correct hospital
            // Proceed to create the doctor: check for duplicate email, create a user with the "doctor" role, hash the password, and create a new doctor object linked to the user

            if (createDoctorDTO.CreatorRole == Role.PATIENT || createDoctorDTO.CreatorRole == Role.DOCTOR)
            {
                return Forbid("You do not have permission to create a doctor");
            }

            var existingHospital = await _context.Hospitals.FirstOrDefaultAsync(h => h.HospitalId == createDoctorDTO.HospitalId);
            if (existingHospital == null)
            {
                return BadRequest("Hospital does not exist");
            }

            if (createDoctorDTO.CreatorRole != Role.ADMIN && createDoctorDTO.CreatorId != createDoctorDTO.HospitalId)
            {
                return Forbid("You do not have permission to assign a doctor to this hospital");
            }

            var checker = await IsUserRegistered(createDoctorDTO.Email);
            if (checker == "registered")
            {
                return Conflict("Email already registered");
            }



            var newUserResult = await CreateUser(createDoctorDTO.Email, createDoctorDTO.Password, Role.DOCTOR);
            if (newUserResult.Value == null)
            {
                return BadRequest("Failed to create user");
            }

            var newDoctor = new Doctor
            {
                UserId = newUserResult.Value.UserId,
                Name = createDoctorDTO.Name,
                Age = createDoctorDTO.Age,
                Sex = createDoctorDTO.Sex,
                Phone = createDoctorDTO.Phone,
                MedicalSpecialtyId = createDoctorDTO.MedicalSpecialtyId,
                HospitalId = createDoctorDTO.HospitalId,
                Degree = createDoctorDTO.Degree,
                ConsultingPriceViaMessage = createDoctorDTO.ConsultingPriceViaMessage,
                ConsultingPriceViaCall = createDoctorDTO.ConsultingPriceViaCall
            };

            await _context.Doctors.AddAsync(newDoctor);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Doctor created successfully" });
        }



        private async Task<Token> CreateToken(long userId, bool expires)
        {
            var delta = TimeSpan.FromMinutes(15);
            DateTime? expiration;
            if (expires)
            {
                expiration = DateTime.UtcNow.Add(delta);
            }
            else
            {
                expiration = null;
            }
            Console.WriteLine(expiration);

            var expirationValue = expiration.HasValue
                ? ((DateTimeOffset)expiration.Value).ToUnixTimeSeconds().ToString()
                : "";

            var claims = new[]
                {
                    new Claim("UserId", userId.ToString()),
                    new Claim("Expires", expires.ToString()),
                    new Claim("Expiration", expirationValue)
                };

            //tạo khóa bí mật 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT_SECRET_KEY));

            //tạo credentials 
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiration,
                signingCredentials: credential
            );

            //chuyển token từ kiểu JwtSecurityToken sang string 
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);

            var newT = new Token
            {
                UserId = userId,
                TokenValue = tokenString,
                Expires = expires
            };

            await _context.Tokens.AddAsync(newT);
            await _context.SaveChangesAsync();
            return newT;
        }



        [HttpPost("login")]
        public async Task<ActionResult<object>> Login(LoginDTO loginDTO)
        {
            var loginUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
            if (loginUser == null)
            {
                return Unauthorized("Email không tồn tại");
            }

            var p = loginDTO.Email + loginDTO.Password;

            if (!BCrypt.Net.BCrypt.Verify(p, loginUser.PasswordHash))
            {
                return Unauthorized("wrong password");
            }

            //đăng nhập thành công thì tạo token
            var token = await CreateToken(loginUser.UserId, true);
            return Ok(new
            {
                TokenValue = token.TokenValue,
                UserId = loginUser.UserId,
                Role = loginUser.Role
            });
        }

        [HttpGet("logout")]
        public async Task<ActionResult<string>> Logout(string tokenValue)
        {
            var currentToken = await _context.Tokens.FirstOrDefaultAsync(t => t.TokenValue == tokenValue);
            if (currentToken != null)
            {
                _context.Tokens.Remove(currentToken);
                await _context.SaveChangesAsync();
                return Ok("Logged out successfully");
            }
            else
            {
                return BadRequest("Invalid token");
            }

        }

        [HttpPost("change-password")]
        public async Task<ActionResult<string>> ChangePassword(string email, string newPassword)
        {
            var check = await IsUserRegistered(email);
            if (check == "not registered")
            {
                return BadRequest("Email is not registered");
            }
            else
            {
                var p = email + newPassword;
                var newHashPassword = BCrypt.Net.BCrypt.HashPassword(p);
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (currentUser == null)
                {
                    return BadRequest("User not found");
                }
                currentUser.PasswordHash = newHashPassword;
                await _context.SaveChangesAsync();
                return Ok("Password changed successfully");
            }
        }

        [HttpGet("verify_token_user")]
        public async Task<ActionResult<string>> VerifyTokenUser(string token)
        {

            try
            {

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(JWT_SECRET_KEY);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var tokenInDb = await _context.Tokens.FirstOrDefaultAsync(t => t.TokenValue == token);

                if (tokenInDb == null)
                {
                    return Unauthorized("Token is not exist");
                }
                return Ok("ok");

            }
            catch (SecurityTokenExpiredException)
            {
                return Unauthorized("Token has expired");
            }
            catch (SecurityTokenException)
            {
                return Unauthorized("Invalid token");
            }
        }

        [HttpGet("user/{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            if (await IsUserRegistered(email) == "not registered")
            {
                return NotFound("User not found");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return Ok(user);
        }

        [HttpGet("user/role/{role}")]
        public async Task<ActionResult<List<User>>> GetUserByRole(Role role)
        {
            var users = await _context.Users.Where(u => u.Role == role).ToListAsync();
            return Ok(users);
        }



        //---------img----------//
        [HttpPost("user/up_img")]
        public async Task<ActionResult<object>> UploadImage(string email, IFormFile image)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");
            if (image is null || image.Length == 0)
                return BadRequest("Image is required.");

            var user = await _context.Users.AsTracking().FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound("User not found.");
            }


            var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            if (!allowed.Contains(ext))
                return BadRequest("Unsupported file type.");


            var fileName = $"{user.UserId}{ext}";


            var dir = Path.Combine(_env.ContentRootPath, "image", "user");
            Directory.CreateDirectory(dir);
            var fullPath = Path.Combine(dir, fileName);
            using (var stream = System.IO.File.Create(fullPath))
            {
                await image.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("image", "user", fileName).Replace('\\', '/');
            user.Img = relativePath;
            await _context.SaveChangesAsync();

            return Ok(new { fileName = fileName, file_path = relativePath });


        }

        [HttpGet("user/get_img")]
        public async Task<ActionResult<object>> GetImage([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("email is required");

            var relPath = await _context.Users
                .AsNoTracking()
                .Where(u => u.Email == email)
                .Select(u => u.Img)                 // ví dụ: "image/user/9.jpg"
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(relPath))
                return NotFound();

            var rel = relPath.TrimStart('/', '\\');
            var absPath = Path.GetFullPath(Path.Combine(_env.ContentRootPath, rel));

            if (!System.IO.File.Exists(absPath))
                return NotFound();

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(absPath, out var contentType))
                contentType = "application/octet-stream";

            return PhysicalFile(absPath, contentType, enableRangeProcessing: true);
        }




    }







}
