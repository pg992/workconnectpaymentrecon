using Newtonsoft.Json;
using System;

namespace PaymentReconciliation.Contracts
{
    public partial class TokenResponse
    {
        [JsonProperty("scope")]
        public Uri Scope { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("app_id")]
        public string AppId { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }
    }
}
