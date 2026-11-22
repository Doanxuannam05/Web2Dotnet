using DemoAPINET.Data;
using DemoAPINET.DTOs;
using DemoAPINET.Middleware;
using DemoAPINET.Repositories;
using DemoAPINET.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Lưu trữ DataProtection keys vào thư mục nội bộ
var keysDir = Path.Combine(builder.Environment.ContentRootPath, "temp_keys");
if (!Directory.Exists(keysDir)) Directory.CreateDirectory(keysDir);
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(keysDir));

// 1. Cấu hình Controllers
builder.Services.AddControllers();

// 2. Cấu hình DbContext kết nối SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
    });
    options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// 3. Đăng ký Options Pattern cho FileSettings (Buổi 10)
builder.Services.Configure<FileSettingsOptions>(
    builder.Configuration.GetSection("FileSettings"));

// 4. Cấu hình Dependency Injection (DI)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IFileService, FileService>();

// Đăng ký DI cho Hệ thống Quản lý ngày nghỉ nhân sự (HR Leave Management)
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ILeaveTypeRepository, LeaveTypeRepository>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
builder.Services.AddScoped<ILeaveService, LeaveService>();

// 5. Cấu hình CORS (Buổi 13 - Slide 10 & 21)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Chỉ cho phép domain này của Frontend
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 6. Cấu hình Swagger nâng cao có hỗ trợ Bearer Token (Buổi 13 - Slide 5-6)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProjectAPIdemo", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập JWT token (Swagger sẽ tự thêm tiền tố Bearer).",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

// 7. Cấu hình JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey)),
        RoleClaimType = ClaimTypes.Role
    };
});

var app = builder.Build();

// Tự động khởi tạo dữ liệu mẫu (Admin, HR, Nhân viên, Loại nghỉ, Phòng ban)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DataSeeder.SeedAsync(dbContext);
}

// ==========================================
// 8. Cấu hình Request Pipeline (Thứ tự Middleware)
// ==========================================

// Buổi 12: Exception Middleware đăng ký đầu tiên để hứng toàn bộ ngoại lệ trong pipeline
app.UseMiddleware<GlobalExceptionMiddleware>();

// Buổi 12: Request Timing Middleware đo thời gian thực thi (milliseconds) của mọi request
app.UseMiddleware<RequestTimingMiddleware>();

// Swagger UI (Development & Testing)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Phục vụ trang mặc định index.html và file tĩnh trong wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

// Buổi 13: CORS Middleware - đặt sau static files / routing và trước Authentication / Authorization
app.UseCors("AllowFrontend");

// Authentication đứng TRƯỚC Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
