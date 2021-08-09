using Azure.Identity;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventGridPublisher
{
    public static class EventGridServiceExtension
    {
        public static IServiceCollection RegisterEventGridService(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<ApplicationSettings>(configuration.GetSection("ApplicationCredential"));
            var settings = configuration.GetSection("ApplicationCredential").Get<ApplicationSettings>();
            var credential = new ClientSecretCredential(settings.TenantId, settings.ClientId, settings.ClientSecret);
            serviceCollection.AddScoped(_ => new EventGridPublisherClient(settings.EventGridEndpoint, credential));

            //serviceCollection.AddScoped<IBlobStorageService, BlobStorageService>();

            return serviceCollection;
        }
    }
}
