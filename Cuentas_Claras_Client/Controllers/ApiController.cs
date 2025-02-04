using Cuentas_Claras_Client.Connection;
using Cuentas_Claras_Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace Cuentas_Claras_Client.Controllers
{
    public class ApiController
    {
        private readonly IConfiguration _configuration;
        private readonly ConnectionDB _context;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiController(IConfiguration configuration, ConnectionDB context, IHttpClientFactory httpClientFactory, string baseUrl)
        {
            _configuration = configuration;
            _context = context;
            _httpClient = httpClientFactory.CreateClient("ApiJwt");
            _baseUrl = configuration["ExternalApi:BaseUrl"];
        }

        public async Task<ApiResponse> GetTokenAsync(string username, string password)
        {
            var request = new
            {
                Username = username,
                Password = password
            };

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/register", content);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseData);

                return apiResponse;

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error en la solicitud HTTP: {ex.Message}");
                return null;
            } 
            catch (JsonException ex)
            {
                Console.WriteLine($"Error al deserializar la respuesta: {ex.Message}");
                return null;
            }
        }
    }
}

public class ApiResponse
{
    public string username { get; set; }
    public string password { get; set; }
}
