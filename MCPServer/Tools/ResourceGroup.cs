using Azure.ResourceManager.Resources;
using ModelContextProtocol.Server;
using System.ComponentModel;
using MCPServer.Tools;

[McpServerToolType]
public static class ResourceGroupTools
{
     [McpServerTool, Description("Lists all Azure Resource Groups in the subscription.")]
    public static async Task<List<string>> ListResourceGroups()
    {
          var result = new List<string>();

        try
        {
            var armClient = AZ_AuthHelper.funGetArmClient();
            var subscription = await armClient.GetDefaultSubscriptionAsync();
            var resourceGroups = subscription.GetResourceGroups();

            await foreach (ResourceGroupResource rg in resourceGroups.GetAllAsync())
            {
                result.Add(rg.Data.Name);
            }

            return result;
           }
        catch (Exception ex)
        {
            result.Add($"[ERROR] Could not retrieve resource groups: {ex.Message}");
            return result;
        }
    }
}