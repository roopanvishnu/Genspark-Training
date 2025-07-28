using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChienVHShopAPI.Models
{
    public class CaptchaResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}
