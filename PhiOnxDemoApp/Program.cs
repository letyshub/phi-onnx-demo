using Microsoft.Extensions.Configuration;
using PhiOnxDemoApp;

var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

IConfiguration config = builder.Build();
var settings = config.GetSection("AppSettings").Get<AppSettings>();
ArgumentNullException.ThrowIfNull(settings);

new PhiPrompt(settings).Run();