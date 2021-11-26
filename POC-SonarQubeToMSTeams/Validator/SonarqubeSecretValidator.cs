﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using POC_SonarQubeToMSTeams.Validator.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;

namespace POC_SonarQubeToMSTeams.Validator
{
    public class SonarqubeSecretValidator : ISonarqubeSecretValidator
    {
        public const string SonarqubeAuthSignatureHeaderName = "X-Sonar-Webhook-HMAC-SHA256";
        public bool IsValidSignature(HttpRequest request, string requestBody, string sonarqubeWebhookSecret)
        {
            // Read the header that is sent by Sonarqube, which contains a computed HMAC SHA256 hash based on the request body and a configured secret.
            StringValues headerValues = request.Headers[SonarqubeAuthSignatureHeaderName];
            if (headerValues.Count == 0)
                return false;
            string receivedSignature = headerValues[0]; // Assume only one value for this header

            string expectedSignature = GetHMACSHA256Hash(requestBody, sonarqubeWebhookSecret);
            return object.Equals(expectedSignature, receivedSignature);
        }

        public string GetHMACSHA256Hash(string text, string key)
        {
            var encoding = new UTF8Encoding();

            Byte[] textBytes = encoding.GetBytes(text);
            Byte[] keyBytes = encoding.GetBytes(key);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
