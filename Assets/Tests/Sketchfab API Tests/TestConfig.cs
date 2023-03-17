
using RestHTTP;
using SketchfabAPI;
using SketchfabAPI.DTOs;

namespace Tests.SketchafbAPI
{
    public static class TestConfig
    {
        public const string ModelSearchQuery = "Table";
        public const string DownloadModelId = "a24bcb6b7a2e448fad661004f403ae12";

        public static NetworkSettings NetworkSettings { get; } = new NetworkSettings()
        {
            DefaultRequestTimeoutSeconds = 10
        };

        public static SketchfabAPISettings SketchfabAPISettings { get; } = new SketchfabAPISettings
        {
            Client = new ClientSettings()
            {
                Id = "MgY9nnsYaL9c0rBaxj6eUmkWLUGGZBylvmBvoNMN",
                Secret = "J7NQxhXXB3jhQhnpmz9k7LCi8CIwmw9hRl19bGLEIo7GXh2yewzcIdqRrpddFIdr9ylhHvp0uj8TNf4Z6vJnANNDiFZ9YsqNmWJ2oNQdePIWDxRtXTvTUAg44VqIaBR6"
            }
        };

        public static CredentialsDTO LoginCredentials { get; } = new CredentialsDTO()
        {
            Email = "bobrider@tutanota.com",
            Password = "nJ4s9_wSa1"
        };
    }
}
