using Azure.Identity;
using Azure.ResourceManager;

namespace MCPServer.Tools;

public static class AZ_AuthHelper
{

    public static ArmClient funGetArmClient(string pSubscriptionId)
    {
        string? vTenantId = null;
        string? vClientId = null;
        string? vClientSecret = null;
        try
        {
            vTenantId = Environment.GetEnvironmentVariable("AZ_TENANT_ID");
            vClientId = Environment.GetEnvironmentVariable("AZ_CLIENT_ID");
            vClientSecret = Environment.GetEnvironmentVariable("AZ_CLIENT_SECRET");
            if (string.IsNullOrWhiteSpace(vTenantId))
                throw new Exception("AZ_TENANT_ID is missing or empty.");
            if (string.IsNullOrWhiteSpace(vClientId))
                throw new Exception("AZ_CLIENT_ID is missing or empty.");
            if (string.IsNullOrWhiteSpace(vClientSecret))
                throw new Exception("AZ_CLIENT_SECRET is missing or empty.");
            if (string.IsNullOrWhiteSpace(pSubscriptionId))
                throw new Exception("AZ_SUBSCRIPTION_ID is missing or empty.");

            var vCredential = new ClientSecretCredential(vTenantId, vClientId, vClientSecret);
            return new ArmClient(vCredential, pSubscriptionId);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("[Auth ERROR] Failed to authenticate with Azure:");
            Console.Error.WriteLine($"AZ_TENANT_ID: {vTenantId ?? "null"}");
            Console.Error.WriteLine($"AZ_CLIENT_ID: {vClientId ?? "null"}");
            Console.Error.WriteLine($"AZ_CLIENT_SECRET: {(vClientSecret != null ? vClientSecret.Substring(0, 5) + "..." : "null")}");
            Console.Error.WriteLine($"AZ_SUBSCRIPTION_ID: {pSubscriptionId ?? "null"}");
            Console.Error.WriteLine($"Exception: {ex.Message}");
            throw;
        }
    }

    public static ArmClient funGerArmClientWithoutSubscription()
    {
        string? vTenantId = null;
        string? vClientId = null;
        string? vClientSecret = null;
          try
        {
            vTenantId = Environment.GetEnvironmentVariable("AZ_TENANT_ID");
            vClientId = Environment.GetEnvironmentVariable("AZ_CLIENT_ID");
            vClientSecret = Environment.GetEnvironmentVariable("AZ_CLIENT_SECRET");
            if (string.IsNullOrWhiteSpace(vTenantId))
                throw new Exception("AZ_TENANT_ID is missing or empty.");
            if (string.IsNullOrWhiteSpace(vClientId))
                throw new Exception("AZ_CLIENT_ID is missing or empty.");
            if (string.IsNullOrWhiteSpace(vClientSecret))
                throw new Exception("AZ_CLIENT_SECRET is missing or empty.");

            var vCredential = new ClientSecretCredential(vTenantId, vClientId, vClientSecret);
            return new ArmClient(vCredential);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("[Auth ERROR] Failed to authenticate with Azure:");
            Console.Error.WriteLine($"AZ_TENANT_ID: {vTenantId ?? "null"}");
            Console.Error.WriteLine($"AZ_CLIENT_ID: {vClientId ?? "null"}");
            Console.Error.WriteLine($"AZ_CLIENT_SECRET: {(vClientSecret != null ? vClientSecret.Substring(0, 5) + "..." : "null")}");
            Console.Error.WriteLine($"Exception: {ex.Message}");
            throw;
        }
    }
}