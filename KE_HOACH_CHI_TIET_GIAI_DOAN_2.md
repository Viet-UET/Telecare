# 📋 KẾ HOẠCH CHI TIẾT GIAI ĐOẠN 2: THIẾT KẾ DATABASE INCREMENTAL

## 🎯 **TỔNG QUAN GIAI ĐOẠN 2 (Tuần 2 - 5 sub-phases)**

### **Timeline:** 5 ngày × 5 phases = 25 ngày (≈ 5 tuần thực tế với testing)

---

## 📅 **GIAI ĐOẠN 2A: CORE AUTHENTICATION (Ngày 1-5)**

### **Mục tiêu:** Thiết lập foundation cho authentication system

### **Entities cần implement:**
```csharp
- User (core user data)
- Token (JWT token management)
```

### **Chi tiết implementation:**

#### **Day 1: Project Structure Setup**
```
MomCare/
├── MomCare.Domain/
│   ├── Entities/
│   │   ├── User.cs
│   │   └── Token.cs
│   └── Enums/
│       └── UserRole.cs
├── MomCare.Infrastructure/
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   └── Configurations/
│       ├── UserConfiguration.cs
│       └── TokenConfiguration.cs
```

#### **Day 2: User Entity**
```csharp
// MomCare.Domain/Entities/User.cs
public class User
{
    public long UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? GoogleId { get; set; }
    public string? Img { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();
}

// MomCare.Domain/Enums/UserRole.cs
public enum UserRole
{
    ADMIN = 1,
    PATIENT = 2,
    DOCTOR = 3,
    HOSPITAL = 4
}
```

#### **Day 3: Token Entity & Configuration**
```csharp
// MomCare.Domain/Entities/Token.cs
public class Token
{
    public long TokenId { get; set; }
    public string TokenValue { get; set; } = string.Empty;
    public long UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
}

// MomCare.Infrastructure/Configurations/UserConfiguration.cs
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.UserId);
        
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(128);
            
        builder.HasIndex(e => e.Email)
            .IsUnique();
            
        builder.Property(e => e.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);
            
        builder.Property(e => e.Role)
            .IsRequired()
            .HasConversion<string>();
            
        builder.Property(e => e.GoogleId)
            .HasMaxLength(128);
            
        builder.Property(e => e.Img)
            .HasMaxLength(500);
    }
}
```

#### **Day 4: DbContext Setup & Migration**
```csharp
// MomCare.Infrastructure/Data/ApplicationDbContext.cs
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Token> Tokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
```

**Commands:**
```bash
# Tạo migration đầu tiên
Add-Migration Phase2A_InitialUserAndToken
Update-Database

# Verify database
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo'
```

#### **Day 5: Testing & Validation**
```csharp
// Test basic CRUD operations
public class Phase2ATests
{
    [Test]
    public async Task CanCreateUser()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetInMemoryOptions());
        
        var user = new User
        {
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
            Role = UserRole.PATIENT
        };
        
        // Act
        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        // Assert
        var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
        Assert.IsNotNull(savedUser);
        Assert.AreEqual(UserRole.PATIENT, savedUser.Role);
    }
}
```

**✅ Deliverables Phase 2A:**
- [x] User và Token entities
- [x] Database configuration
- [x] Migration tạo tables
- [x] Basic CRUD tests pass

---

## 📅 **GIAI ĐOẠN 2B: USER PROFILES (Ngày 6-10)**

### **Mục tiêu:** Implement user profile entities với relationships

### **Entities cần implement:**
```csharp
- Patient
- Doctor  
- Hospital
- MedicalSpecialty
```

#### **Day 6: Patient Entity**
```csharp
// MomCare.Domain/Entities/Patient.cs
public class Patient
{
    public long PatientId { get; set; }
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? Age { get; set; }
    public Sex? Sex { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
}

// MomCare.Domain/Enums/Sex.cs
public enum Sex
{
    MALE = 1,
    FEMALE = 2,
    NOT_MENTION = 3
}
```

#### **Day 7: Doctor Entity & MedicalSpecialty**
```csharp
// MomCare.Domain/Entities/MedicalSpecialty.cs
public class MedicalSpecialty
{
    public long SpecialtyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Navigation properties
    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}

// MomCare.Domain/Entities/Doctor.cs
public class Doctor
{
    public long DoctorId { get; set; }
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long? SpecialtyId { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? Phone { get; set; }
    public string? Description { get; set; }
    public decimal? ConsultationFee { get; set; }
    public bool IsVerified { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual MedicalSpecialty? Specialty { get; set; }
}
```

#### **Day 8: Hospital Entity**
```csharp
// MomCare.Domain/Entities/Hospital.cs
public class Hospital
{
    public long HospitalId { get; set; }
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Description { get; set; }
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public bool IsVerified { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
}
```

#### **Day 9: Entity Configurations & Relationships**
```csharp
// MomCare.Infrastructure/Configurations/PatientConfiguration.cs
public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(e => e.PatientId);
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.Phone)
            .HasMaxLength(20);
            
        builder.Property(e => e.Address)
            .HasMaxLength(500);
            
        builder.Property(e => e.Sex)
            .HasConversion<string>();
            
        // One-to-One relationship with User
        builder.HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<Patient>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

// Similar configurations for Doctor, Hospital, MedicalSpecialty
```

#### **Day 10: Migration & Testing**
```bash
# Create migration
Add-Migration Phase2B_AddUserProfiles
Update-Database
```

**Testing scenarios:**
```csharp
[Test]
public async Task CanCreatePatientWithUser()
{
    // Test User -> Patient relationship
}

[Test] 
public async Task CanCreateDoctorWithSpecialty()
{
    // Test Doctor -> MedicalSpecialty relationship
}

[Test]
public async Task CascadeDeleteWorksCorrectly()
{
    // Test khi xóa User thì Patient/Doctor/Hospital cũng bị xóa
}
```

**✅ Deliverables Phase 2B:**
- [x] Patient, Doctor, Hospital, MedicalSpecialty entities
- [x] One-to-One relationships với User
- [x] Foreign key constraints
- [x] Migration successful
- [x] Relationship tests pass

---

## 📅 **GIAI ĐOẠN 2C: APPOINTMENT SYSTEM (Ngày 11-15)**

### **Entities cần implement:**
```csharp
- CallAppointment
- HospitalAppointment  
- AppointmentState (enum)
```

#### **Day 11: Appointment Base & Enums**
```csharp
// MomCare.Domain/Enums/AppointmentState.cs
public enum AppointmentState
{
    PENDING = 1,
    CONFIRMED = 2,
    IN_PROGRESS = 3,
    COMPLETED = 4,
    CANCELLED = 5,
    RESCHEDULED = 6
}

// MomCare.Domain/Entities/CallAppointment.cs
public class CallAppointment
{
    public long CallAppointmentId { get; set; }
    public long PatientId { get; set; }
    public long DoctorId { get; set; }
    public DateTime AppointmentTime { get; set; }
    public AppointmentState State { get; set; } = AppointmentState.PENDING;
    public string? Notes { get; set; }
    public decimal? Fee { get; set; }
    public string? CancelReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
}
```

#### **Day 12: Hospital Appointment**
```csharp
// MomCare.Domain/Entities/HospitalAppointment.cs
public class HospitalAppointment
{
    public long HospitalAppointmentId { get; set; }
    public long PatientId { get; set; }
    public long HospitalId { get; set; }
    public long? DoctorId { get; set; } // Optional - có thể chọn doctor specific
    public DateTime AppointmentTime { get; set; }
    public AppointmentState State { get; set; } = AppointmentState.PENDING;
    public string? Department { get; set; }
    public string? Notes { get; set; }
    public decimal? Fee { get; set; }
    public string? CancelReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Hospital Hospital { get; set; } = null!;
    public virtual Doctor? Doctor { get; set; }
}
```

#### **Day 13-14: Entity Configurations**
```csharp
// MomCare.Infrastructure/Configurations/CallAppointmentConfiguration.cs
public class CallAppointmentConfiguration : IEntityTypeConfiguration<CallAppointment>
{
    public void Configure(EntityTypeBuilder<CallAppointment> builder)
    {
        builder.HasKey(e => e.CallAppointmentId);
        
        builder.Property(e => e.State)
            .IsRequired()
            .HasConversion<string>();
            
        builder.Property(e => e.Fee)
            .HasColumnType("decimal(10,2)");
            
        builder.Property(e => e.Notes)
            .HasMaxLength(1000);
            
        builder.Property(e => e.CancelReason)
            .HasMaxLength(500);
            
        // Relationships
        builder.HasOne(ca => ca.Patient)
            .WithMany()
            .HasForeignKey(ca => ca.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(ca => ca.Doctor)
            .WithMany()
            .HasForeignKey(ca => ca.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Indexes for performance
        builder.HasIndex(e => e.AppointmentTime);
        builder.HasIndex(e => e.State);
        builder.HasIndex(e => new { e.PatientId, e.AppointmentTime });
    }
}
```

#### **Day 15: Migration & Complex Testing**
```bash
Add-Migration Phase2C_AddAppointments
Update-Database
```

**Advanced Testing:**
```csharp
[Test]
public async Task CanCreateAppointmentWithValidReferences()
{
    // Test tạo appointment với Patient và Doctor hợp lệ
}

[Test]
public async Task PreventDeletePatientWithActiveAppointments()
{
    // Test Restrict delete behavior
}

[Test]
public async Task CanQueryAppointmentsByDateRange()
{
    // Test performance với date indexing
}
```

**✅ Deliverables Phase 2C:**
- [x] CallAppointment và HospitalAppointment entities
- [x] Complex relationships với multiple entities
- [x] Business constraints (Restrict deletes)
- [x] Performance indexes
- [x] Advanced testing scenarios

---

## 📅 **GIAI ĐOẠN 2D: MESSAGING SYSTEM (Ngày 16-20)**

### **Entities cần implement:**
```csharp
- Conversation
- Message
- Attachment
```

#### **Day 16: Conversation Entity**
```csharp
// MomCare.Domain/Entities/Conversation.cs
public class Conversation
{
    public long ConversationId { get; set; }
    public long PatientId { get; set; }
    public long DoctorId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastMessageAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
```

#### **Day 17: Message Entity**
```csharp
// MomCare.Domain/Entities/Message.cs
public class Message
{
    public long MessageId { get; set; }
    public long ConversationId { get; set; }
    public long SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    
    // Navigation properties
    public virtual Conversation Conversation { get; set; } = null!;
    public virtual User Sender { get; set; } = null!;
    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
```

#### **Day 18: Attachment Entity**
```csharp
// MomCare.Domain/Entities/Attachment.cs
public class Attachment
{
    public long AttachmentId { get; set; }
    public long MessageId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Message Message { get; set; } = null!;
}
```

#### **Day 19: Complex Configurations**
```csharp
// MomCare.Infrastructure/Configurations/ConversationConfiguration.cs
public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(e => e.ConversationId);
        
        // Unique constraint: One conversation per Patient-Doctor pair
        builder.HasIndex(e => new { e.PatientId, e.DoctorId })
            .IsUnique();
            
        // Relationships
        builder.HasOne(c => c.Patient)
            .WithMany()
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(c => c.Doctor)
            .WithMany()
            .HasForeignKey(c => c.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Performance indexes
        builder.HasIndex(e => e.LastMessageAt);
    }
}
```

#### **Day 20: Migration & Real-world Testing**
```csharp
[Test]
public async Task UniqueConversationConstraintWorks()
{
    // Test không thể tạo 2 conversation cho cùng 1 cặp Patient-Doctor
}

[Test]
public async Task MessageOrderingByTimestamp()
{
    // Test query messages theo thứ tự thời gian
}

[Test]
public async Task CascadeDeleteAttachmentsWithMessage()
{
    // Test xóa message thì attachments cũng bị xóa
}
```

**✅ Deliverables Phase 2D:**
- [x] Messaging system entities
- [x] Unique business constraints
- [x] Cascading delete strategies
- [x] Performance optimization
- [x] Real-world scenario testing

---

## 📅 **GIAI ĐOẠN 2E: REVIEWS & MEDICAL RECORDS (Ngày 21-25)**

### **Entities cần implement:**
```csharp
- DoctorComment
- HospitalComment
- MedicalRecord
- Prescription
- Invoice, Payment, Transaction
```

#### **Day 21: Rating System**
```csharp
// MomCare.Domain/Entities/DoctorComment.cs
public class DoctorComment
{
    public long CommentId { get; set; }
    public long PatientId { get; set; }
    public long DoctorId { get; set; }
    public int Rating { get; set; } // 1-5 stars
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
}

// MomCare.Domain/Entities/HospitalComment.cs
public class HospitalComment
{
    public long CommentId { get; set; }
    public long PatientId { get; set; }
    public long HospitalId { get; set; }
    public int Rating { get; set; } // 1-5 stars
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Hospital Hospital { get; set; } = null!;
}
```

#### **Day 22: Medical Records**
```csharp
// MomCare.Domain/Entities/MedicalRecord.cs
public class MedicalRecord
{
    public long RecordId { get; set; }
    public long PatientId { get; set; }
    public long DoctorId { get; set; }
    public string? Diagnosis { get; set; }
    public string? Symptoms { get; set; }
    public string? Treatment { get; set; }
    public string? Notes { get; set; }
    public DateTime RecordDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}

// MomCare.Domain/Entities/Prescription.cs
public class Prescription
{
    public long PrescriptionId { get; set; }
    public long RecordId { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int Duration { get; set; } // in days
    public string? Instructions { get; set; }
    
    // Navigation properties
    public virtual MedicalRecord MedicalRecord { get; set; } = null!;
}
```

#### **Day 23: Payment System**
```csharp
// MomCare.Domain/Enums/InvoiceStatus.cs
public enum InvoiceStatus
{
    PENDING = 1,
    PAID = 2,
    CANCELLED = 3,
    REFUNDED = 4
}

// MomCare.Domain/Entities/Invoice.cs
public class Invoice
{
    public long InvoiceId { get; set; }
    public long PatientId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public InvoiceStatus Status { get; set; } = InvoiceStatus.PENDING;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
    
    // Navigation properties  
    public virtual Patient Patient { get; set; } = null!;
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

// MomCare.Domain/Entities/Payment.cs
public class Payment
{
    public long PaymentId { get; set; }
    public long InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Invoice Invoice { get; set; } = null!;
}

// MomCare.Domain/Entities/Transaction.cs
public class Transaction
{
    public long TransactionId { get; set; }
    public long UserId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty; // CREDIT, DEBIT
    public string Description { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
}
```

#### **Day 24: Final Migration & Integration Testing**
```bash
Add-Migration Phase2E_AddReviewsAndMedicalRecords
Update-Database

# Verify complete schema
SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = 'dbo'
ORDER BY TABLE_NAME, ORDINAL_POSITION
```

#### **Day 25: Comprehensive Testing**
```csharp
[Test]
public async Task CompleteWorkflowTest()
{
    // Test complete user journey:
    // 1. Create Patient & Doctor
    // 2. Make appointment
    // 3. Have conversation
    // 4. Create medical record
    // 5. Generate invoice
    // 6. Leave review
}

[Test]
public async Task DatabaseConstraintsWork()
{
    // Test all business rules and constraints
}

[Test]
public async Task PerformanceTestWithLargeDataset()
{
    // Test với 1000+ records để verify indexes
}
```

**✅ Final Deliverables Phase 2:**
- [x] Complete database schema với 15+ entities
- [x] All relationships properly configured
- [x] Business constraints implemented  
- [x] Performance indexes in place
- [x] Comprehensive test coverage
- [x] Migration scripts ready
- [x] Documentation hoàn chỉnh

---

## 🎯 **CHECKLIST HOÀN THÀNH GIAI ĐOẠN 2**

### **Technical Deliverables:**
- [ ] 15+ Entity classes hoàn chỉnh
- [ ] Entity configurations với Fluent API
- [ ] 5 migration files successful
- [ ] DbContext setup hoàn chỉnh
- [ ] Unit tests cho tất cả entities
- [ ] Integration tests cho complex scenarios
- [ ] Performance benchmarks

### **Learning Outcomes:**
- [ ] Hiểu sâu Entity Framework Core
- [ ] Thành thạo Code-First migrations
- [ ] Nắm vững relationship configurations
- [ ] Kinh nghiệm database constraint design
- [ ] Kỹ năng performance tuning với indexes
- [ ] Testing strategies cho data layer

### **Documentation:**
- [ ] Entity Relationship Diagram
- [ ] Database schema documentation
- [ ] Migration guide
- [ ] Testing documentation
- [ ] Performance optimization notes

**Estimation: 25 ngày công việc thực tế = ~5 tuần calendar time với testing và learning**

---

## 📚 **TÀI LIỆU THAM KHẢO**

### **Entity Framework Core:**
- [EF Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Code First Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Fluent API Configuration](https://docs.microsoft.com/en-us/ef/core/modeling/entity-properties)

### **SQL Server:**
- [SQL Server Data Types](https://docs.microsoft.com/en-us/sql/t-sql/data-types/data-types-transact-sql)
- [Index Design Guidelines](https://docs.microsoft.com/en-us/sql/relational-databases/indexes/clustered-and-nonclustered-indexes-described)

### **Testing:**
- [EF Core Testing](https://docs.microsoft.com/en-us/ef/core/testing/)
- [Unit Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

---

## 🔧 **COMMANDS REFERENCE**

### **Migration Commands:**
```bash
# Add new migration
Add-Migration <MigrationName>

# Update database
Update-Database

# Remove last migration
Remove-Migration

# Script migration to SQL
Script-Migration

# Check migration status
Get-Migration
```

### **Database Commands:**
```bash
# Drop database
Drop-Database

# Ensure database created
Ensure-Created

# Generate database script
Script-DbContext
``` 