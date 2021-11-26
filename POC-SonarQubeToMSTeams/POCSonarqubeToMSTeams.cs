using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using POC_SonarQubeToMSTeams.Business;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace POC_SonarQubeToMSTeams
{
    public class POCSonarqubeToMSTeams
    {
        private readonly string _teamsWebhookUrl;
        private readonly string _sonarqubeWebhookSecret;
        private readonly string _disableAuthentication;
        private readonly string _culture;

        public const string Setting_QualityGateStatusExcludeList = "QualityGateStatusExcludeList";

        private readonly HttpClient _httpClient;
        private readonly ISonarqubeToMSTeamsConverter _sonarqubeToMSTeamsConverter;
        private readonly ISonarqubeToMSTeamsFilter _sonarqubeToMSTeamsFilter;
        public POCSonarqubeToMSTeams(HttpClient httpClient, ISonarqubeToMSTeamsConverter sonarqubeToMSTeamsConverter, ISonarqubeToMSTeamsFilter sonarqubeToMSTeamsFilter,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _sonarqubeToMSTeamsConverter = sonarqubeToMSTeamsConverter;
            _sonarqubeToMSTeamsFilter = sonarqubeToMSTeamsFilter;
            _teamsWebhookUrl = configuration["TeamsWebhookUrl"];
            _disableAuthentication = configuration["DisableAuthentication"];
            _sonarqubeWebhookSecret = configuration["SonarqubeWebhookSecret"];
            _culture = configuration["Culture"];
        }

        [FunctionName("POCSonarqubeToMSTeams")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Request received from Sonarqube webhook.");

            string name = req.Query["name"];

            // ----------------------------------------------------
            // Get and validate config settings
            // ----------------------------------------------------
            // Required setting: SonarqubeWebhookSecret. Contains the Sonarqube Webhook secret. It's the same secret that was configured in the Sonarqube Webhook.
            //if(string.IsNullOrEmpty(_sonarqubeWebhookSecret))
            //{
            //    log.LogError($"Required setting {SonarqubeWebhookSecret is missing.");
            //    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            //}

            // Required setting: TeamsWebhookUrl
            //var teamsWebhookUrl = Environment.GetEnvironmentVariable(_teamsUrl, EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(_teamsWebhookUrl))
            {
                log.LogError($"Required setting {_teamsWebhookUrl} is missing.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            // Optional setting: QualityGateStatusExcludeList. A comma separated list of Sonarqube Quality Status values that should be ignored.
            var qualityGateStatusExcludes = Environment.GetEnvironmentVariable(Setting_QualityGateStatusExcludeList, EnvironmentVariableTarget.Process);

            // Optional setting: Culture. Affects displayed Date/time values in MS Teams card. Examples: en-US, sv-SE. If not specified, dates/times are displayed as received from Sonarqube
            var culture = Environment.GetEnvironmentVariable(_culture, EnvironmentVariableTarget.Process);

            // ----------------------------------------------------
            // Validate signature in http header
            // ----------------------------------------------------
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //if(!disableAuthentication && !_sonarqubeSecretValidator.IsValidSignature(req, requestBody, sonarqubeWebhookSecret))
            //{
            //    log.LogWarning($"Sonarqube secret http header is missing or not valid. Config setting {Setting_SonarqubeWebhookSecret} must match secret in Sonarqube Webhook header {SonarqubeSecretValidator.SonarqubeAuthSignatureHeaderName}.");
            //    return new UnauthorizedResult();
            //}

            // ----------------------------------------------------
            // Check if a card should be sent to MS Teams or not.
            // ----------------------------------------------------
            dynamic sonarqubeRequestJson = JsonConvert.DeserializeObject(requestBody);

            if (!_sonarqubeToMSTeamsFilter.ShouldProcess(sonarqubeRequestJson, qualityGateStatusExcludes))
            {
                log.LogInformation($"Message was not sent to MS Teams due to filter.");
                return new OkResult();
            }

            // ----------------------------------------------------
            // Build MS Teams card from Sonarqube Webhook data 
            // ----------------------------------------------------
            var msTeamsCard = _sonarqubeToMSTeamsConverter.ToComplexCard(sonarqubeRequestJson, _culture);

            // ----------------------------------------------------
            // Send message to MS Teams webhook url
            // ----------------------------------------------------
            log.LogInformation($"Sending request to MS Teams webhook URL: {_teamsWebhookUrl}");
            var teamsCardContent = new StringContent(JsonConvert.SerializeObject(msTeamsCard), Encoding.UTF8, "application/json");
            await _httpClient.PostAsync(_teamsWebhookUrl, teamsCardContent);

            log.LogInformation("Request successfully sent to MS Teams webhook.");
            return new OkResult();            
        }
    }
}
