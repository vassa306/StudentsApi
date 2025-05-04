using System.Net;
using System.Text;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NUnit.Framework;
using studentsapi.DTO;
using studentsapi.Model;
using studentsapi.Tests.TestData;

namespace studentsapi.Tests
{
    public class CorsTest
    {
        private TestServer _server;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            // Fix for CS0029 and CS1002: Correctly initialize _client instead of _server
            _client = TestServerMock.CreateClientWithCorsPolicy("AllowAll", true);
        }

        [Test]
        public async Task Should_Add_AccessControlAllowOrigin_Header()
        {
            var request = new HttpRequestMessage(HttpMethod.Options, "/api/Students/All");

            request.Headers.Add("Origin", "*");
            request.Headers.Add("Access-Control-Request-Method", "GET");

            var response = await _client.SendAsync(request);

            Console.WriteLine("Headers:");
            foreach (var header in response.Headers)
            {
                Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            Assert.That(response.Headers.Contains("Access-Control-Allow-Origin"), Is.True, "CORS header missing");

            var origin = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
            Assert.That(origin, Is.EqualTo("*"));
        }

        [Test]
        public async Task Should_Allow_CORS_For_Get_Method()
        {
            
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Options, "/api/Students/All");
            request.Headers.Add("Origin", "*");
            request.Headers.Add("Access-Control-Request-Method", "GET");
            // Act
            var response = await _client.SendAsync(request);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.That(response.Headers.Contains("Access-Control-Allow-Origin"), Is.True, "CORS header missing");
        }

        [Test]
        public async Task GetAllStudents_Should_Return_OK_And_StudentList()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/Students/All");
            request.Headers.Add("Authorization", "Bearer admin-token");
            // Arrange


            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected 200 OK");

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.That(responseBody, Is.Not.Null.And.Not.Empty, "Expected response body to contain data");

            var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);
            Assert.That(apiResponse, Is.Not.Null);
            Assert.That(apiResponse.Status, Is.True);

            var students = JsonConvert.DeserializeObject<List<StudentDto>>(apiResponse.Data.ToString());
            Assert.That(students, Is.Not.Null.And.Not.Empty, "Expected at least one student");
        }

        [Test]
        public async Task Should_Allow_CORS_For_Post_Method()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Options, "/api/Students/create");
            request.Headers.Add("Origin", "*");
            request.Headers.Add("Access-Control-Request-Method", "POST");
            // Act
            var response = await _client.SendAsync(request);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.That(response.Headers.Contains("Access-Control-Allow-Origin"), Is.True, "CORS header missing");
        }

        [Test]
        public async Task Get_should_be_Unauthorized()
        {
            _client = TestServerMock.CreateClientWithCorsPolicy("AllowOnlyLocalhost", true);
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/Students/All");
            request.Headers.Add("Origin", "http://localhost:5000");
            request.Headers.Add("Access-Control-Request-Method", "GET");
            // Act
            var response = await _client.SendAsync(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), "Expected 401 Unauthorized");
        }
        [Test]
        public async Task Get_should_be_403()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/Students/All");
            request.Headers.Add("Origin", "http://localhost:5000");
            request.Headers.Add("Access-Control-Request-Method", "GET");
            request.Headers.Add("Authorization", "Bearer manager-token");
            // Act
            var response = await _client.SendAsync(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden), "Expected 403 forbidden");
        }




        [Test]
        public async Task PostStudent_Should_Return_Created_And_CORS_Header()
        {

            
            _client = TestServerMock.CreateClientWithCorsPolicy("AllowAll", true);
            // Arrange
            var student = new
            {
                name = "TestUser",
                email = "testuser@example.com",
                address = "123 Test Street"
            };

            var json = JsonConvert.SerializeObject(student);

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Students/create")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Origin", "*");
            request.Headers.Add("Authorization", "Bearer admin-token");
            var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(response.Headers.Contains("Access-Control-Allow-Origin"), Is.True, "CORS header missing");

            var origin = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
            Assert.That(origin, Is.EqualTo("*"));
        }


        [Test]
        public async Task Should_Allow_Only_Specific_Origins()
        {
            _client = TestServerMock.CreateClientWithCorsPolicy("AllowOnlyLocalhost", true);
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Options, "/api/students");
            request.Headers.Add("Origin", "http://localhost:5000");
            request.Headers.Add("Access-Control-Request-Method", "GET");
            // Act
            var response = await _client.SendAsync(request);
            // Assert
            Assert.That(response.Headers.Contains("Access-Control-Allow-Origin"), "CORS header missing");
            Assert.That(response.Headers.GetValues("Access-Control-Allow-Origin"), Does.Contain("http://localhost:5000"));
        }

        [Test]
        public async Task Should_Reject_Unknown_Origins()
        {
            _client = TestServerMock.CreateClientWithCorsPolicy("AllowOnlyLocalhost", true);
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Options, "/api/students");
            request.Headers.Add("Origin", "http://unknown-origin.com");
            request.Headers.Add("Access-Control-Request-Method", "GET");
            // Act
            var response = await _client.SendAsync(request);
            // Assert
            Assert.That(response.Headers.Contains("Access-Control-Allow-Origin"), Is.False, "CORS header should not be present for unknown origins");
        }

        [TearDown]
        public void Cleanup()
        {
            _client.Dispose();
        }
    }
}

