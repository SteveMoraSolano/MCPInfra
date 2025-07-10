using Azure.ResourceManager.Network;
using Azure.ResourceManager.Network.Models;
using System.ComponentModel;
using System.Diagnostics;
using ModelContextProtocol.Server;


namespace MCPServer.Tools;

[McpServerToolType]
public static class SubnetTools
{
    [McpServerTool, Description("List Subnets in a Virtual Network")]
    public static async Task<List<string>> funListSubnetsByVNet(string pSubscriptionId, string pResourceGroupName, string pVNetName)
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
            var vVNet = await vResourceGroup.GetVirtualNetworks().GetAsync(pVNetName);

            var vSubnets = vVNet.Value.GetSubnets();

            await foreach (var vSubnet in vSubnets)
            {
                var vData = vSubnet.Data;
                vResult.Add($"{vData.Name} - AddressPrefix: {string.Join(", ", vData.AddressPrefix)}");
            }
        }
        catch (Exception vEx)
        {
            vResult.Add($"[ERROR] {vEx.Message}");
        }

        return vResult;
    }

  
}