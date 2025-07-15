using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.KeyVault.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPServer.Tools;

[McpServerToolType]

public static class KeyVaultAnalyzerTools
{
    [McpServerTool, Description("Analyze Key Vault in a resource group and return deep insights with best-practice evaluation.")]
    public static async Task<List<string>> funAnalyzeKeyVault(string pSubscriptionId, string pResourceGroupName)
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
            var vKeyVaults = vResourceGroup.GetKeyVaults();
            await foreach (var vKV in vKeyVaults)
            {
                var vKV_Data = vKV.Data;
                vResult.Add($"\nKeyVault: {vKV_Data.Name}");
                vResult.Add($"- Location: {vKV_Data.Location}");
                vResult.Add($"- Enabled for Deployment: {vKV_Data.Properties.EnabledForDeployment ?? false}");
                vResult.Add($"- Enabled for Disk Encryption: {vKV_Data.Properties.EnabledForDiskEncryption ?? false}");
                vResult.Add($"- Enabled for Template Deployment: {vKV_Data.Properties.EnabledForTemplateDeployment ?? false}");
                vResult.Add($"- Soft Delete Enabled: {vKV_Data.Properties.EnableSoftDelete ?? false}");
                vResult.Add($"- Purge Protection Enabled: {vKV_Data.Properties.EnablePurgeProtection ?? false}");
                vResult.Add($"- Public Network Access: {vKV_Data.Properties.PublicNetworkAccess ?? "Unknown"}");
                vResult.Add($"- RBAC Authorization: {vKV_Data.Properties.EnableRbacAuthorization ?? false}");

                if (vKV_Data.Tags != null && vKV_Data.Tags.Count > 0)
                    vResult.Add("Tags: Present");
                else
                    vResult.Add("Tags: Missing (owner, env, cost center)");
            }


            var vKvBps = await KnowledgeBaseTools.funGetBestPracticesByResourceType("keyvault");
            vResult.Add("\nBest Practices (KeyVault):");
            vResult.AddRange(vKvBps);


            var vSecurityBp = await KnowledgeBaseTools.funGetBestPracticesByResourceType("security");
            vResult.Add("\nBest Practices (Security):");
            vResult.AddRange(vSecurityBp);

            var vMonitoringBp = await KnowledgeBaseTools.funGetBestPracticesByResourceType("monitoring");
            vResult.Add("\nBest Practices (Monitoring):");
            vResult.AddRange(vMonitoringBp);
        }
        catch (Exception ex)
        {
            vResult.Add($"[ERROR] VM Analysis Failed: {ex.Message}");
        }

        return vResult;
    }
}
