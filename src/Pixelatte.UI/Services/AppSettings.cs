using Microsoft.Extensions.Configuration;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Pixelatte.UI.Services
{
    public static class AppSettings
    {
        public static IConfiguration Configuration { get; }

        static AppSettings()
        {
            StorageFolder installedLocationFolder = Package.Current.InstalledLocation;
            var builder = new ConfigurationBuilder()
                .SetBasePath(installedLocationFolder.Path)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }
    }
}
