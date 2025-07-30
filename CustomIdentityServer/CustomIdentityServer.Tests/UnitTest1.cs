using Xunit;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;



namespace CustomIdentityServer.Tests
{
    public class RegisterTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public RegisterTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_ReturnsOk()
        {
            var uniqueId = System.Guid.NewGuid().ToString("N").Substring(0, 8);
            var payload = new
            {
                username = $"testuser_{uniqueId}",
                email = $"testuser_{uniqueId}@example.com",
                password = "Test1234!",
                masterPin = "123456"
            };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/register", content);

            Assert.True(response.IsSuccessStatusCode, $"Status code: {response.StatusCode}");

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Contains("User created successfully", responseBody);
        }
    }
}
