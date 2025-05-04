using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using System.Reflection; // needed for AutoMapper fix
using Microsoft.EntityFrameworkCore;
using studentsapi.Data;
using studentsapi.Configurations;
using studentsapi.Data.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models; // where your AutoMapper Profile(s) live

var builder = WebApplication.CreateBuilder(args);

// Setup Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("Log/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

// Add services to the container
builder.Services.AddControllers(options => options.ReturnHttpNotAcceptable = true)
    .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "Bearer",
                Name = "Authorization",
                In = ParameterLocation.Header
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddDbContext<CollegeDBContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddAutoMapper(typeof(AutoMapperConfig)); // Register AutoMapper with the assembly containing your profiles
    builder.Services.AddScoped(typeof(ICollegeRepository<>), typeof(CollegeRepository<>));
    builder.Services.AddScoped<IStudentRepository, StudentRepository>();
    // Add CORS policy
    builder.Services.AddCors(options =>
    {

        options.AddDefaultPolicy(
            policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        options.AddPolicy("AllowOnlyLocalhost", policy =>
        {
            policy.WithOrigins("http://localhost:5000")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
        options.AddPolicy("AllowSpecificOrigin", policy =>
        {
            policy.WithOrigins("http://google.com", "http://drive.google.com")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
        options.AddPolicy("AllowOnlyMicrosoft", policy =>
        {
            policy.WithOrigins("http://microsoft.com", "http://outlook.com", "http://onedrive.com")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });
    //Jwt authentication
    var key = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JWTSecret"));
    var localkey = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JWTSecretLocal"));


    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer("LoginforAppUSers", options =>

    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {

            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    }).AddJwtBearer("Local", options =>

    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {

            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(localkey)
        };
    });


    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();

