using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPServer.Tools;

[McpServerToolType]

public static class StorageAnalyzerTools
{
    [McpServerTool, Description("Analyze StorageAccount in a resource group and return deep insights with best-practice evaluation.")]
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
                vResult.Add($"- Public Access Allowed: {(vSA_Data.AllowBlobPublicAccess == true ? "Yes" : "No")}");
                vResult.Add($"- Encryption Enabled: {(vSA_Data.Encryption?.Services?.Blob?.IsEnabled == true ? "Yes" : "No")}");
                vResult.Add($"- Encryption Key Source: {(vSA_Data.Encryption?.KeySource == StorageAccountKeySource.KeyVault ? "Customer Managed (Key Vault)" : "Microsoft Managed")}");

                if (vSA_Data.IsHnsEnabled == true)
                    vResult.Add("- Hierarchical Namespace (Data Lake): Enabled");
                else
                    vResult.Add("- Hierarchical Namespace (Data Lake): Disabled");

                vResult.Add($"- HTTPS Traffic Only: {(vSA_Data.EnableHttpsTrafficOnly == true ? "Yes" : "No")}");

                if (vSA_Data.NetworkRuleSet?.DefaultAction == StorageNetworkDefaultAction.Deny)
                    vResult.Add("Public Network Access: Restricted (Deny by Default)");
                else
                    vResult.Add("Public Network Access: Not Restricted");

                if (vSA_Data.Tags != null && vSA_Data.Tags.Count > 0)
                    vResult.Add("Tags: Present");
                else
                    vResult.Add("Tags: Missing (owner, env, cost center)");
            }


            var vStorageBps = await KnowledgeBaseTools.funGetBestPracticesByResourceType("storage");
            vResult.Add("\nBest Practices (Storage):");
            vResult.AddRange(vStorageBps);

            var vSecurityBp = await KnowledgeBaseTools.funGetBestPracticesByResourceType("security");
            vResult.Add("\nBest Practices (Security):");
            vResult.AddRange(vSecurityBp);

            var vCostBp = await KnowledgeBaseTools.funGetBestPracticesByResourceType("cost-optimization");
            vResult.Add("\nBest Practices (Cost Optimization):");
            vResult.AddRange(vCostBp);

            var vPerfBp = await KnowledgeBaseTools.funGetBestPracticesByResourceType("performance");
            vResult.Add("\nBest Practices (Performance):");
            vResult.AddRange(vPerfBp);

            var vGovBp = await KnowledgeBaseTools.funGetBestPracticesByResourceType("governance");
            vResult.Add("\nBest Practices (Governance):");
            vResult.AddRange(vGovBp);

            var vMonitoringBp = await KnowledgeBaseTools.funGetBestPracticesByResourceType("monitoring");
            vResult.Add("\nBest Practices (Monitoring):");
            vResult.AddRange(vMonitoringBp);

        }
        catch (Exception ex)
        {
            vResult.Add($"[ERROR] Storage Analysis Failed: {ex.Message}");
        }

        return vResult;
    }
}
