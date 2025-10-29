Attendance Management API

This is the backend API for the Attendance Management System — built using ASP.NET Core Web API and Entity Framework Core.
It provides secure REST endpoints for handling user registration, authentication (with JWT), OTP verification, email notifications, and attendance management.

Tech Stack used for this projectas: 

.NET 8.0 / ASP.NET Core Web API
Entity Framework Core (Code-First)
SQL Server
JWT Authentication
SMTP Email Service for OTP & notifications

Features

🔐 Role-based Authentication (Admin, Student, SuperAdmin)
✉️ Email OTP Verification (students & admins must verify before registration)
👨‍🏫 Admin Management
SuperAdmin approves or denies admin registration
Admins can manage students (CRUD)
📅 Attendance Tracking
Admins mark attendance for students (Present/Absent)
Attendance records save with the Admin ID who marked it
📊 Student Reports
Students can view monthly attendance and percentage

Folder structure

AttendanceApi/
│
├── Controllers/
│   ├── AuthController.cs
│   ├── AdminController.cs
│   ├── AttendanceController.cs
│
├── Model/
│   ├── StudentModel.cs
│   ├── AdminModel.cs
│   ├── AttendanceModel.cs
│
├── Repository/
│   ├── Repository.cs
│   ├── IRepo.cs
│   ├── EmailService.cs
│
└── appsettings.json

Run the API
Open the project in Visual Studio.
Update your SQL connection string in appsettings.json:

"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ApiAttendance;Trusted_Connection=True;TrustServerCertificate=True;"
}
Run the project → Swagger UI will open.
Test endpoints directly from Swagger.


Key Learning

How to structure a clean layered API.
Integrating JWT authentication and email OTP.
How to use Entity Framework Core efficiently.
Clean role-based design for scalable architecture.

👩‍💻 Author
Abinaya
📧 [abinayameganthan05@gmail.com]
🌐 [www.linkedin.com/in/abinaya-meganathan-52093a216]
