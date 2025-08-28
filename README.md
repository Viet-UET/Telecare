# 🏥 Telecare - Healthcare Management System

A comprehensive healthcare management system built with ASP.NET Core 8.0, providing telemedicine services, appointment management, and healthcare facility administration.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [API Documentation](#api-documentation)
- [Database Schema](#database-schema)
- [Contributing](#contributing)
- [License](#license)

## 🎯 Overview

Telecare is a modern healthcare management system designed to facilitate communication between patients, doctors, and hospitals. The system supports telemedicine appointments, secure messaging, and comprehensive healthcare facility management.

## ✨ Features

###  Authentication & Authorization
- JWT-based authentication system
- Role-based access control (Patient, Doctor, Admin)
- Secure password hashing with BCrypt

###  User Management
- Patient registration and profile management
- Doctor profiles with medical specialties
- Hospital and medical facility management
- User role management

###  Appointment System
- Call appointments for telemedicine
- Hospital appointments for in-person visits
- Appointment state tracking
- Scheduling and calendar management

###  Communication
- Secure messaging system between users
- Conversation management
- File and image attachments
- Real-time communication support

###  Healthcare Management
- Medical specialty categorization
- Hospital and clinic management
- Doctor-patient relationship management
- Comment and review system

##  Technology Stack

- **Backend Framework**: ASP.NET Core 8.0
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT Bearer Tokens
- **API Documentation**: Swagger/OpenAPI
- **Password Hashing**: BCrypt.Net-Next
- **Object Mapping**: AutoMapper
- **Data Seeding**: Bogus for test data

## 📁 Project Structure

```
Telecare/
├── Controllers/          # API Controllers
├── Models/              # Entity Models
│   └── Enums/          # Enumeration Types
├── DTOs/               # Data Transfer Objects
├── Data/               # Database Context
├── Services/           # Business Logic Services
├── Helpers/            # Utility Classes
├── Migrations/         # Database Migrations
├── attachment/         # File Attachments
└── image/              # User Images
```

##  Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/telecare.git
   cd telecare
   ```

2. **Configure the database connection**
   - Update `appsettings.json` with your SQL Server connection string
   - Default uses LocalDB: `Server=(localdb)\MSSQLLocalDB;Database=telecare;Trusted_Connection=True;TrustServerCertificate=True;`

3. **Install dependencies**
   ```bash
   dotnet restore
   ```

4. **Run database migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access the application**
   - API: `https://localhost:5001`
   - Swagger UI: `https://localhost:5001/swagger`

### Environment Setup

The application automatically:
- Runs database migrations on startup
- Seeds initial data using `DbSeeder`
- Configures Swagger for development

##  API Documentation

### 🔐 Authentication & Account Management
- `POST /Account/create-patient` - Create new patient account
- `POST /Account/create-hospital` - Create new hospital account
- `POST /Account/create_medicalspecialty` - Create new medical specialty
- `POST /Account/create_doctor` - Create new doctor account
- `POST /Account/login` - User login and get JWT token
- `GET /Account/logout` - User logout
- `POST /Account/change-password` - Change user password
- `GET /Account/verify_token_user` - Verify JWT token validity
- `GET /Account/user/{email}` - Get user by email
- `GET /Account/user/role/{role}` - Get users by role
- `POST /Account/user/up_img` - Upload user profile image
- `GET /Account/user/get_img` - Get user profile image

### 👨‍⚕️ Doctor Management
- `PUT /Doctor/update-doctor` - Update doctor information
- `GET /Doctor/list-doctor` - Get list of all doctors
- `GET /Doctor/list-doctor/name` - Get doctors sorted by name
- `GET /Doctor/doctorbyuserid/{userId}` - Get doctor by user ID
- `GET /Doctor/doctor-in-hospital/{hospitalId}` - Get doctors in specific hospital
- `GET /Doctor/doctor/top/{page}` - Get top doctors with pagination

### 🏥 Hospital Management
- `PUT /Hospital/{id}` - Update hospital information
- `GET /Hospital/list-hospital` - Get list of all hospitals
- `GET /Hospital/list-hospital/name` - Get hospitals sorted by name
- `GET /Hospital/hospitalbyuserid/{userId}` - Get hospital by user ID

### 📅 Appointment Management
- `POST /Appointment/make-call-appointment` - Create new telemedicine appointment
- `POST /Appointment/change-time-call-appointment` - Change telemedicine appointment time
- `POST /Appointment/change-state-call-appointment` - Change telemedicine appointment state
- `POST /Appointment/make-hospital-appointment` - Create new hospital appointment
- `POST /Appointment/change-time-hospital-appointment` - Change hospital appointment time
- `POST /Appointment/change-state-hospital-appointment` - Change hospital appointment state
- `GET /Appointment/users/{userId}` - Get appointments for specific user
- `GET /Appointment/users/quantity` - Get total number of user appointments
- `GET /Appointment/doctors/quantity` - Get total number of doctor appointments
- `GET /Appointment/hospitals/quantity` - Get total number of hospital appointments

### 💬 Communication & Messaging
- `GET /Conversation/Conversation/{conversationId}/Mess` - Get messages in conversation
- `GET /Conversation/Conversation/{conversationId}/Att` - Get attachments in conversation
- `POST /Conversation/Conversation` - Create new conversation
- `POST /Conversation/Send_Message` - Send message in conversation
- `POST /Conversation/Send_Attachment` - Send attachment in conversation
- `GET /Conversation/Get_Attachment` - Get conversation attachment

### 💭 Comment & Feedback System
- `POST /Comment/add-doctor-comment` - Add comment for doctor
- `POST /Comment/add-hospital-comment` - Add comment for hospital
- `GET /Comment/get-doctor-comment/{doctorId}` - Get comments for specific doctor
- `GET /Comment/get-hospital-comment/{hospitalId}` - Get comments for specific hospital

### 🏥 Medical Specialty Management
- `GET /MedicalSpecialty/list-medicalspecialty` - Get list of all medical specialties
- `GET /MedicalSpecialty/medicalspecialty/{medicalspecialtyId}` - Get medical specialty by ID
- `GET /MedicalSpecialty/medicalspecialty/vi/{vi}` - Get medical specialty by Vietnamese name
- `GET /MedicalSpecialty/medicalspecialty/en/{en}` - Get medical specialty by English name

## 🗄️ Database Schema

### Core Entities
- **User**: Base user information and authentication
- **Patient**: Patient-specific data and medical history
- **Doctor**: Doctor profiles and specialties
- **Hospital**: Healthcare facility information
- **MedicalSpecialty**: Medical specialization categories

### Appointment System
- **CallAppointment**: Telemedicine appointments
- **HospitalAppointment**: In-person hospital visits
- **AppointmentState**: Appointment status tracking

### Communication System
- **Conversation**: Chat sessions between users
- **Message**: Individual messages within conversations
- **Attachment**: File and image attachments

### Feedback System
- **DoctorComment**: Patient feedback for doctors
- **HospitalComment**: Patient feedback for hospitals

## 🔧 Development

### Adding New Features
1. Create model in `Models/` directory
2. Add DTOs in `DTOs/` directory
3. Create controller in `Controllers/` directory
4. Add database migration: `dotnet ef migrations add FeatureName`
5. Update database: `dotnet ef database update`

### Code Style
- Follow C# naming conventions
- Use async/await for database operations
- Implement proper error handling
- Add XML documentation for public APIs

