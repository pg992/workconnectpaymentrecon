using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentReconciliation;
using PaymentReconciliation.Services;

[assembly: WebJobsStartup(typeof(Startup))]
namespace PaymentReconciliation
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var confBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            builder.Services.AddSingleton<IConfiguration>(confBuilder);
            builder.Services.AddScoped<IPaypalService, PaypalService>();
            builder.Services.AddTransient<IRestClient, RestClient>();
        }
    }
}
