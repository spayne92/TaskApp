using System.Text.Json.Serialization;

namespace BaseCoreAPI.Infrastructure
{
    public class JwtTokenConfig
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        [JsonPropertyName("audience")]
        public string Audience { get; set; }

        [JsonPropertyName("accessTokenExpiration")]
        public int AccessTokenExpiration { get; set; }

        [JsonPropertyName("refreshTokenExpiration")]
        public int RefreshTokenExpiration { get; set; }
    }
}
