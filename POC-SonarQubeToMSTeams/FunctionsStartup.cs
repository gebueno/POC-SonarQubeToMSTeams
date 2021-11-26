using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using POC_SonarQubeToMSTeams.Business;

[assembly: FunctionsStartup(typeof(POC_SonarQubeToMSTeams.Startup))]

namespace POC_SonarQubeToMSTeams
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            builder.Services.AddSingleton<ISonarqubeToMSTeamsConverter, SonarqubeToMSTeamsConverter>();

        }
    }
}