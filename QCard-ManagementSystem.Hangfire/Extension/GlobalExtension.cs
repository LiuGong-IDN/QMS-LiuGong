using Hangfire;
using Newtonsoft.Json;

namespace QCard.ManagementSystem.Hangfire.Extension
{
    public static class GlobalExtension
    {
        public static void UseMediatR(this IGlobalConfiguration configuration)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            configuration.UseSerializerSettings(jsonSettings);
        }
    }
}
