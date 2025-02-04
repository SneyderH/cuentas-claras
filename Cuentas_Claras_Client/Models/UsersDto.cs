using System.Text.Json.Serialization;

namespace Cuentas_Claras_Client.Models
{
    public class UsersDto
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("passwordHash")]
        public string PasswordHash { get; set; }
    }
}
