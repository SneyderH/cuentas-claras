using Cuentas_Claras_Client.Connection;
using Cuentas_Claras_Client.Models;
using System.Text;
using System.Text.Json;

namespace Cuentas_Claras_Client.Controllers
{
    public class ApiController
    {
        private readonly IConfiguration _configuration;
        private readonly ConnectionDB _context;
        private readonly HttpClient _httpClient;

        public ApiController(IConfiguration configuration, ConnectionDB context, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _context = context;
            _httpClient = httpClientFactory.CreateClient("ApiJwt");
        }

        public async Task<Users> GetTokenAsync(string username, string password)
        {
            var request = new
            {
                Username = username,
                Password = password
            };

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("register", content);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                var usersDto = JsonSerializer.Deserialize<UsersDto>(responseData);

                var user = new Users
                {
                    Username = usersDto.Username,
                    Password = usersDto.PasswordHash
                };

                //var apiResponse = JsonSerializer.Deserialize<Users>(responseData);

                return user;

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

        public async Task<string> TokenLogin(string username, string password)
        {
            var loginRequest = new { username, password };
            var json = JsonSerializer.Serialize(loginRequest);
            Console.WriteLine(json);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("login", content);

            if (response.IsSuccessStatusCode)
            {
                var responserJson = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<string>(responserJson);
                return responseData;
            }
            else
            {
                throw new Exception("Error en la solicitud: " + response.StatusCode);
            }
        }
    }
}


