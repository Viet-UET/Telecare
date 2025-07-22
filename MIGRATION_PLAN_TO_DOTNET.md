# KẾ HOẠCH MIGRATION TỪ FASTAPI + MYSQL SANG ASP.NET CORE + SQL SERVER - DỰ ÁN TELECARE

## TỔNG QUAN DỰ ÁN HIỆN TẠI

### Công nghệ hiện tại:
- **Backend**: FastAPI (Python)
- **Database**: MySQL 
- **ORM**: SQLAlchemy
- **Authentication**: JWT tokens
- **File Storage**: Local file system

### Chức năng chính:
1. **Quản lý người dùng**: Patient, Doctor, Hospital, Admin
2. **Authentication/Authorization**: JWT-based với role-based access control
3. **Đặt lịch hẹn**: Call appointment, Hospital appointment  
4. **Tin nhắn**: Messaging system giữa doctor-patient
5. **Đánh giá**: Comment/rating cho doctor và hospital
6. **Upload file**: User avatar, attachments
7. **Báo cáo y tế**: Medical records, prescriptions
8. **Thanh toán**: Payment, invoice, transaction

## KẾ HOẠCH MIGRATION

### GIAI ĐOẠN 1: THIẾT LẬP DỰ ÁN ASP.NET CORE (Tuần 1)

#### 1.1 Tạo Single Project Structure (Kiểu cổ điển)
```
TelecareAPI/
├── Controllers/                  # API Controllers
├── Models/                      # Entity models  
├── Data/                        # DbContext
├── Services/                    # Business logic services
├── DTOs/                        # Data Transfer Objects
├── Helpers/                     # Utilities, JWT helper
├── Middleware/                  # Custom middleware
├── Uploads/                     # File storage folder
├── appsettings.json
└── Program.cs
```

#### 1.2 Cài đặt NuGet Packages
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
<PackageReference Include="Swashbuckle.AspNetCore" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" />
<PackageReference Include="BCrypt.Net-Next" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.NewtonsoftJson" />
```

### GIAI ĐOẠN 2: THIẾT KẾ DATABASE VÀ MODELS (Tuần 2)

#### 2.1 Entity Models (Models folder)
```csharp
// Models/User.cs
public class User
{
    public long UserId { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } // ADMIN, PATIENT, DOCTOR, HOSPITAL
    public string GoogleId { get; set; }
    public string Img { get; set; }
    
    // Navigation properties
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
    public Hospital Hospital { get; set; }
    public List<Token> Tokens { get; set; }
}

// Models/Patient.cs
public class Patient
{
    public long PatientId { get; set; }
    public long UserId { get; set; }
    public string Name { get; set; }
    public int? Age { get; set; }
    public string Sex { get; set; } // MALE, FEMALE, NOT_MENTION
    public string Phone { get; set; }
    public string Address { get; set; }
    
    public User User { get; set; }
    public List<CallAppointment> CallAppointments { get; set; }
    public List<HospitalAppointment> HospitalAppointments { get; set; }
}

// Models/Doctor.cs - tương tự cho tất cả entities khác
```

#### 2.2 DbContext (Data folder)
```csharp
// Data/ApplicationDbContext.cs
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Hospital> Hospitals { get; set; }
    public DbSet<MedicalSpecialty> MedicalSpecialties { get; set; }
    public DbSet<CallAppointment> CallAppointments { get; set; }
    public DbSet<HospitalAppointment> HospitalAppointments { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<DoctorComment> DoctorComments { get; set; }
    public DbSet<HospitalComment> HospitalComments { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    public DbSet<MedicalHistory> MedicalHistories { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Token> Tokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships trực tiếp ở đây
        modelBuilder.Entity<User>()
            .HasOne(u => u.Patient)
            .WithOne(p => p.User)
            .HasForeignKey<Patient>(p => p.UserId);
            
        // Cấu hình tất cả relationships...
    }
}
```

### GIAI ĐOẠN 3: BUSINESS LOGIC SERVICES (Tuần 3-4)

#### 3.1 Services (Services folder)
```csharp
// Services/UserService.cs
public class UserService
{
    private readonly ApplicationDbContext _context;
    
    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<User> CreatePatientAsync(CreatePatientDto dto)
    {
        // Logic trực tiếp, không cần repository pattern
        var user = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "PATIENT"
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var patient = new Patient
        {
            UserId = user.UserId,
            Name = dto.Name,
            Age = dto.Age,
            Sex = dto.Sex,
            Phone = dto.Phone,
            Address = dto.Address
        };
        
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        
        return user;
    }
    
    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");
            
        var token = JwtHelper.GenerateToken(user);
        
        // Lưu token vào database
        _context.Tokens.Add(new Token { UserId = user.UserId, TokenValue = token });
        await _context.SaveChangesAsync();
        
        return new LoginResponseDto { Token = token, User = user };
    }
}

// Services/AppointmentService.cs
public class AppointmentService
{
    private readonly ApplicationDbContext _context;
    
    public AppointmentService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<CallAppointment> MakeCallAppointmentAsync(CreateCallAppointmentDto dto)
    {
        // Business logic trực tiếp
    }
}
```

#### 3.2 DTOs (DTOs folder)
```csharp
// DTOs/UserDtos.cs
public class CreatePatientDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Sex { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
}

public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginResponseDto
{
    public string Token { get; set; }
    public User User { get; set; }
}
```

### GIAI ĐOẠN 4: API CONTROLLERS (Tuần 5-6)

#### 4.1 Controllers trực tiếp sử dụng Services
```csharp
// Controllers/AuthController.cs
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    
    public AuthController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var result = await _userService.LoginAsync(dto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromQuery] string token)
    {
        var result = await _userService.LogoutAsync(token);
        return Ok(result);
    }
}

// Controllers/PatientsController.cs
[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly UserService _userService;
    
    public PatientsController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDto dto)
    {
        var result = await _userService.CreatePatientAsync(dto);
        return Ok(result);
    }
}
```

### GIAI ĐOẠN 5: JWT VÀ AUTHENTICATION (Tuần 7)

#### 5.1 JWT Helper (Helpers folder)
```csharp
// Helpers/JwtHelper.cs
public static class JwtHelper
{
    public static string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim("UserId", user.UserId.ToString()),
            new Claim("Email", user.Email),
            new Claim("Role", user.Role)
        };
        
        var token = new JwtSecurityToken(
            issuer: "TelecareAPI",
            audience: "TelecareClients", 
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials);
            
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public static bool ValidateToken(string token, long userId, ApplicationDbContext context)
    {
        // Validate token logic
    }
}
```

#### 5.2 Authentication Middleware
```csharp
// Middleware/JwtMiddleware.cs
public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApplicationDbContext _context;
    
    public JwtMiddleware(RequestDelegate next, ApplicationDbContext context)
    {
        _next = next;
        _context = context;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        
        if (token != null)
        {
            // Validate và attach user vào context
        }
        
        await _next(context);
    }
}
```

### GIAI ĐOẠN 6: FILE HANDLING (Tuần 8)

#### 6.1 File Service đơn giản
```csharp
// Services/FileService.cs
public class FileService
{
    private readonly string _uploadPath = "Uploads";
    
    public async Task<string> UploadUserImageAsync(string email, IFormFile file)
    {
        var fileName = $"{email}_{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(_uploadPath, "users", fileName);
        
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        
        return filePath;
    }
}

// Controllers/FilesController.cs
[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly FileService _fileService;
    
    [HttpPost("user/upload")]
    public async Task<IActionResult> UploadUserImage([FromQuery] string email, IFormFile image)
    {
        var result = await _fileService.UploadUserImageAsync(email, image);
        return Ok(result);
    }
}
```

### GIAI ĐOẠN 7: PROGRAM.CS SETUP (Tuần 9)

#### 7.1 Configuration đơn giản
```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<FileService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key")),
            ValidateIssuer = true,
            ValidIssuer = "TelecareAPI",
            ValidateAudience = true,
            ValidAudience = "TelecareClients"
        };
    });

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
```

## MAPPING ENDPOINTS (Giữ nguyên như cũ)

### User Management
| FastAPI Endpoint | ASP.NET Core Endpoint | Controller | Method |
|------------------|----------------------|------------|---------|
| `POST /create_patient/` | `POST /api/patients/create` | PatientsController | CreatePatient |
| `POST /create_doctor/` | `POST /api/doctors/create` | DoctorsController | CreateDoctor |
| `POST /create_hospital/` | `POST /api/hospitals/create` | HospitalsController | CreateHospital |
| `GET /login/` | `POST /api/auth/login` | AuthController | Login |
| `GET /logout` | `POST /api/auth/logout` | AuthController | Logout |
| `GET /check/` | `GET /api/auth/check` | AuthController | VerifyToken |

### Appointments
| FastAPI Endpoint | ASP.NET Core Endpoint | Controller | Method |
|------------------|----------------------|------------|---------|
| `POST /make_call_appointment/` | `POST /api/appointments/call` | AppointmentsController | MakeCallAppointment |
| `POST /make_hospital_appointment/` | `POST /api/appointments/hospital` | AppointmentsController | MakeHospitalAppointment |
| `POST /change_time_call_appointment/` | `PUT /api/appointments/call/{id}/time` | AppointmentsController | ChangeCallAppointmentTime |

### Messages
| FastAPI Endpoint | ASP.NET Core Endpoint | Controller | Method |
|------------------|----------------------|------------|---------|
| `GET /conversation/` | `GET /api/messages/conversation` | MessagesController | GetConversation |
| `POST /send_mess/` | `POST /api/messages/send` | MessagesController | SendMessage |
| `POST /send_att/` | `POST /api/messages/attachment` | MessagesController | SendAttachment |

## DATABASE MIGRATION

### Từ MySQL sang SQL Server
1. **Data Types Mapping**:
   - `BIGINT AUTO_INCREMENT` → `BIGINT IDENTITY(1,1)`
   - `VARCHAR(n)` → `NVARCHAR(n)`
   - `TEXT` → `NVARCHAR(MAX)`
   - `ENUM` → `NVARCHAR(n) CHECK (column IN (...))`
   - `FLOAT` → `REAL` hoặc `FLOAT`

2. **Migration Commands**:
```bash
# Tạo migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

## TIMELINE TỔNG THỂ (Đơn giản hóa)

| Tuần | Công việc chính | Deliverables |
|------|----------------|--------------|
| 1 | Project setup, cấu trúc folder | Single project structure |
| 2 | Models, DbContext | Entity models, EF setup |
| 3-4 | Services (business logic) | All service classes |
| 5-6 | Controllers | All API endpoints |
| 7 | Authentication | JWT implementation |
| 8 | File handling | Upload/download features |
| 9 | Testing & fixes | Bug fixes, testing |

## LƯU Ý QUAN TRỌNG

1. **Cấu trúc đơn giản**: Chỉ 1 project, không phân chia phức tạp
2. **Direct DbContext**: Services trực tiếp sử dụng DbContext, không cần Repository pattern
3. **Minimal dependencies**: Chỉ cài những package thực sự cần thiết
4. **Traditional MVC**: Theo pattern MVC cổ điển của ASP.NET Core
5. **Giữ nguyên API behavior**: Response format và business logic y hệt như hiện tại

## CHECKLIST HOÀN THÀNH

- [ ] Single project structure setup
- [ ] All Entity models created
- [ ] DbContext configured  
- [ ] All Services implemented
- [ ] All Controllers implemented
- [ ] JWT Authentication working
- [ ] File upload/download working
- [ ] All endpoints tested
- [ ] Database migration completed
- [ ] Error handling implemented

Cấu trúc này đơn giản hơn nhiều, dễ hiểu và maintain hơn so với clean architecture. 