using Azure.Identity;
using Azure.ResourceManager;

namespace MCPServer.Tools;

public static class AZ_AuthHelper
{

    public static ArmClient funGetArmClient()
    {
         string? vTenantId = null;
    string? vClientId = null;
    string? vClientSecret = null;
    string? vSubscriptionId = null;
        try
        {
            vTenantId = Environment.GetEnvironmentVariable("AZ_TENANT_ID");
            Console.Error.WriteLine($"AZ_TENANT_ID: {vTenantId ?? "null"}");
            vClientId = Environment.GetEnvironmentVariable("AZ_CLIENT_ID");
            Console.Error.WriteLine($"AZ_CLIENT_ID: {vClientId ?? "null"}");
            vClientSecret = Environment.GetEnvironmentVariable("AZ_CLIENT_SECRET");
              Console.Error.WriteLine($"AZ_CLIENT_SECRET: {(vClientSecret != null ? vClientSecret.Substring(0, 5) + "..." : "null")}");
            vSubscriptionId = Environment.GetEnvironmentVariable("AZ_SUBSCRIPTION_ID");
              Console.Error.WriteLine($"AZ_SUBSCRIPTION_ID: {vSubscriptionId ?? "null"}");
            if (string.IsNullOrWhiteSpace(vTenantId))
                throw new Exception("AZ_TENANT_ID is missing or empty.");
            if (string.IsNullOrWhiteSpace(vClientId))
                throw new Exception("AZ_CLIENT_ID is missing or empty.");
            if (string.IsNullOrWhiteSpace(vClientSecret))
                throw new Exception("AZ_CLIENT_SECRET is missing or empty.");
            if (string.IsNullOrWhiteSpace(vSubscriptionId))
                throw new Exception("AZ_SUBSCRIPTION_ID is missing or empty.");

            var vCredential = new ClientSecretCredential(vTenantId, vClientId, vClientSecret);
            return new ArmClient(vCredential, vSubscriptionId);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("[Auth ERROR] Failed to authenticate with Azure:");
            Console.Error.WriteLine($"AZ_TENANT_ID: {vTenantId ?? "null"}");
            Console.Error.WriteLine($"AZ_CLIENT_ID: {vClientId ?? "null"}");
            Console.Error.WriteLine($"AZ_CLIENT_SECRET: {(vClientSecret != null ? vClientSecret.Substring(0, 5) + "..." : "null")}");
            Console.Error.WriteLine($"AZ_SUBSCRIPTION_ID: {vSubscriptionId ?? "null"}");
            Console.Error.WriteLine($"Exception: {ex.Message}");
            throw;
        }
    }
}