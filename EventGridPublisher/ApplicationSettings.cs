using System;

namespace EventGridPublisher
{
    public sealed class ApplicationSettings
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public Uri EventGridEndpoint { get; set; }
    }
}
