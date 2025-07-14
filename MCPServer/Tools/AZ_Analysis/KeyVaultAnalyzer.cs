using Azure.ResourceManager.KeyVault;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPServer.Tools;

public static class KeyVaultAnalyzerTools
{
    [McpServerTool, Description("Analyze Key Vault in a resource group and return VM metadata.")]
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
            }

                     
        var vKvBps = await KnowledgeBaseTools.funGetBestPracticesByResourceType("keyvault");
        vResult.Add("\n📘 Best Practices (KeyVault):");
        vResult.AddRange(vKvBps);
        }
        catch (Exception ex)
        {
            vResult.Add($"[ERROR] VM Analysis Failed: {ex.Message}");
        }

        return vResult;
    }
}
