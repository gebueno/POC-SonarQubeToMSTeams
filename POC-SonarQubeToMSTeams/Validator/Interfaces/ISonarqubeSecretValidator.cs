using Microsoft.AspNetCore.Http;

namespace POC_SonarQubeToMSTeams.Validator.Interfaces
{
    public interface ISonarqubeSecretValidator
    {
        bool IsValidSignature(HttpRequest request, string requestBody, string sonarqubeWebhookSecret);
        string GetHMACSHA256Hash(string text, string key);
    }
}
