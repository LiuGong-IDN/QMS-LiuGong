using Microsoft.Extensions.Configuration;

namespace QCard.ManagementSystem.Domain
{
    public class AppConfiguration
    {
        private readonly string _connectionString = string.Empty;
        private readonly string _applicationLoginType = string.Empty;

        public AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            _connectionString = root.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            _applicationLoginType = root.GetSection("Authentication").GetSection("ApplicationLoginType").Value;

        }
        public string ConnectionString
        {
            get => _connectionString;
        }

        public string ApplicationLoginType
        {
            get => _applicationLoginType;
        }
    }
}