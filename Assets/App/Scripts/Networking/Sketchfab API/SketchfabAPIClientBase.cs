
using RestHTTP;
using SketchfabAPI.Entities;
using System.Collections.Generic;

namespace SketchfabAPI{
    public class SketchfabAPIClientBase : APIClientBase
    {
        private const string BearerTokenType = "Bearer";

        protected SketchfabAPISettings APISettings { get; set; }
        protected string AccessToken { get; set; }

        public void SetAPISettings(SketchfabAPISettings settings)
        {
            APISettings = settings;
        }

        public void SetAccessToken(string accessTokenType, string accessToken) 
        {
            AccessToken = accessTokenType + " " +accessToken;
        }

        public string GetToken() => $"{BearerTokenType} {AccessToken}";
    }
}
