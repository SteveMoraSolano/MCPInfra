using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPServer.Tools;

public static class StorageAnalyzerTools
{
    [McpServerTool, Description("Analyze StorageAccount in a resource group and return VM metadata.")]
    public static async Task<List<string>> funAnalyzeStorageAccount(string pSubscriptionId, string pResourceGroupName)
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
            var vStorageAccounts = vResourceGroup.GetStorageAccounts();

            await foreach (var vSA in vStorageAccounts)
            {
                var vSA_Data = vSA.Data;

                vResult.Add($"\nStorage Account: {vSA_Data.Name}");
                vResult.Add($"- Location: {vSA_Data.Location}");
                vResult.Add($"- Redundancy: {vSA_Data.Sku.Name}");
                vResult.Add($"- TLS Minimum Version: {vSA_Data.MinimumTlsVersion}");
                vResult.Add($"- Access Tier: {vSA_Data.AccessTier}");
                vResult.Add($"- Allow Public Access: {(vSA_Data.AllowBlobPublicAccess == false ? "No" : "Yes")}");
                vResult.Add($"- Encryption Enabled: {(vSA_Data.Encryption.Services.Blob.IsEnabled == true ? "Yes" : "No")}");

                if (vSA_Data.Encryption.KeySource == StorageAccountKeySource.KeyVault)
                    vResult.Add($"- Encryption Key Source: Customer Managed (Key Vault)");
                else
                    vResult.Add($"- Encryption Key Source: Microsoft Managed");

                if (vSA_Data.IsHnsEnabled == true)
                    vResult.Add($"- Hierarchical Namespace (Data Lake): Enabled");

            }

         var vStorageBps = await KnowledgeBaseTools.funGetBestPracticesByResourceType("storage");
        vResult.Add("\n📘 Best Practices (Storage):");
        vResult.AddRange(vStorageBps);

        }
        catch (Exception ex)
        {
            vResult.Add($"[ERROR] Storage Analysis Failed: {ex.Message}");
        }

        return vResult;
    }
}
