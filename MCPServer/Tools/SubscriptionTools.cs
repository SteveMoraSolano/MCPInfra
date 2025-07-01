using System.ComponentModel;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using ModelContextProtocol.Server;

namespace MCPServer.Tools;

[McpServerToolType]
public static class SubscriptionTools
{
    [McpServerTool, Description("Lists available Azure subscriptions")]
    public static async Task<List<string>> funListSubscription()
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGerArmClientWithoutSubscription();

            await foreach (var vSub in vArmClient.GetSubscriptions().GetAllAsync())
            {
                vResult.Add($"{vSub.Data.DisplayName} ({vSub.Data.SubscriptionId})");
            }
        }
        catch (Exception Ex)
        {
            vResult.Add($"[ERROR] {Ex.Message}");
        }
        return vResult;
    }
}