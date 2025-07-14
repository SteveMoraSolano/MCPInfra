using Azure.ResourceManager.Network;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPServer.Tools;

public static class VNetAnalyzerTools
{
    [McpServerTool, Description("Analyze Virtual Network(vnet) in a resource group and return VM metadata.")]
    public static async Task<List<string>> funAnalyzeVNets(string pSubscriptionId, string pResourceGroupName)
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
            var vVNetList = vResourceGroup.GetVirtualNetworks();

            await foreach (var vVNet in vVNetList)
            {
                var vVNet_Data = vVNet.Data;
                vResult.Add($"\nVirtualNetwork: {vVNet_Data.Name}");
                foreach (var vPrefix in vVNet_Data.AddressPrefixes)
                {
                    vResult.Add($"- Address Prefix: {vPrefix}");
                }
                foreach (var vSubnet in vVNet_Data.Subnets)
                {
                    vResult.Add($"- Subnet: {vSubnet.Name} ({string.Join(",", vSubnet.AddressPrefix)})");
                }
            }
            
             var vVnetBps = await KnowledgeBaseTools.funGetBestPracticesByResourceType("vnet");
            vResult.Add("\n📘 Best Practices (VNet):");
            vResult.AddRange(vVnetBps);

        }
        catch (Exception ex)
        {
            vResult.Add($"[ERROR] VM Analysis Failed: {ex.Message}");
        }

        return vResult;
    }
}
