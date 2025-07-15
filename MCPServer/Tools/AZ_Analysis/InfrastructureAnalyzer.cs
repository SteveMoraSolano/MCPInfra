using System.ComponentModel;
using ModelContextProtocol.Server;

namespace MCPServer.Tools;

[McpServerToolType]
public static class InfraAnalyzerTools
{
[McpServerTool, Description("Analyze a resource group infrastructure and return with deep insights for best practice evaluation.")]
public static async Task<List<string>> funAnalyzeInfrastructure(string pSubscriptionId, string pResourceGroupName)
{
    var vResult = new List<string>();

    try
    {
        vResult.AddRange(await VMAnalyzerTools.funAnalyzeVMs(pSubscriptionId, pResourceGroupName));
        vResult.AddRange(await VNetAnalyzerTools.funAnalyzeVNets(pSubscriptionId, pResourceGroupName));
        vResult.AddRange(await StorageAnalyzerTools.funAnalyzeStorageAccount(pSubscriptionId, pResourceGroupName));
        vResult.AddRange(await KeyVaultAnalyzerTools.funAnalyzeKeyVault(pSubscriptionId, pResourceGroupName));
        vResult.AddRange(await ContainerAnalyzerTools.funAnalyzeContainerInstances(pSubscriptionId, pResourceGroupName));
        vResult.AddRange(await AppServiceAnalyzerTools.funAnalyzeAppServices(pSubscriptionId, pResourceGroupName));

        vResult.Add($"\nInfrastructure data collected successfully. Ready for best-practice evaluation.");
    }
    catch (Exception ex)
    {
        vResult.Add($"[ERROR] {ex.Message}");
    }

    return vResult;
}

}
