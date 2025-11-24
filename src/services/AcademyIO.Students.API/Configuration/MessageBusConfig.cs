using AcademyIO.Core.Utils;
using AcademyIO.MessageBus;

namespace AcademyIO.Students.API.Configuration
{
    internal static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"));
        }
    }
}
