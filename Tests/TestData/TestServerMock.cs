using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using studentsapi.Configurations;
using studentsapi.Data;
using studentsapi.Data.Repository;

namespace studentsapi.Tests.TestData
{
    public class TestServerMock
    {
        public static HttpClient CreateClientWithCorsPolicy(string policyName, bool authorized)
        {
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddCors(options =>
                    {
                        options.AddPolicy("AllowOnlyLocalhost", policy =>
                        {
                            policy.WithOrigins("http://localhost:5000")
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();
                        });

                        options.AddPolicy("AllowAll", policy =>
                        {
                            policy.AllowAnyOrigin()
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();
                        });
                    });

                    services.AddAutoMapper(typeof(AutoMapperConfig));
                    services.AddScoped(typeof(ICollegeRepository<>), typeof(CollegeRepository<>));
                    services.AddScoped<IStudentRepository, StudentRepository>();
                    services.AddDbContext<CollegeDBContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));

                    services.AddControllers();

                    if (authorized)
                    {
                        services.AddAuthentication("Test")
                                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Local", null);

                        services.AddAuthorization(options =>
                        {
                            options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
                        });
                    }
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseCors(policyName);

                    if (authorized)
                    {
                        app.UseAuthentication();
                        app.UseAuthorization();
                    }

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });

                    using var scope = app.ApplicationServices.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<CollegeDBContext>();

                    dbContext.Departments.RemoveRange(dbContext.Departments);
                    dbContext.Students.RemoveRange(dbContext.Students);

                    dbContext.Departments.AddRange(
                        new Department { Id = 1, DepartmentName = "IT", Description = "Info Tech" },
                        new Department { Id = 2, DepartmentName = "Math", Description = "Math Dept" }
                    );

                    dbContext.Students.AddRange(
                        new Student { Id = 1, Name = "John", Email = "john@example.com", Address = "Street 123", DepartmentId = 1 },
                        new Student { Id = 2, Name = "Anna", Email = "anna@example.com", Address = "Avenue 456", DepartmentId = 2 }
                    );

                    dbContext.SaveChanges();
                });

            var server = new TestServer(builder);
            var client = server.CreateClient();

            return client;
        }
    }
}
