using System.ComponentModel;
using System.Diagnostics;
using Azure.ResourceManager.Network;
using Azure.ResourceManager.Network.Models;
using ModelContextProtocol.Server;


namespace MCPServer.Tools;

[McpServerToolType]
public static class VNETTools
{
[McpServerTool, Description("List Virtual Networks by Resource Group Name")]
public static async Task<List<string>> funListVirtualNetworksByResourceGroup(string pSubscriptionId, string pResourceGroupName)
{
    var vResult = new List<string>();
    try
    {
        var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
        var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
        var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
        var vVnets = vResourceGroup.GetVirtualNetworks();

        await foreach (var vVnet in vVnets)
        {
            var vData = vVnet.Data;
            var vAddressPrefixes = string.Join(", ", vData.AddressSpace.AddressPrefixes);
            vResult.Add($"{vData.Name} ({vData.Id}) - AddressPrefixes: {vAddressPrefixes}");
        }
    }
    catch (Exception vEx)
    {
        vResult.Add($"[ERROR] {vEx.Message}");
    }

    return vResult;
}
[McpServerTool, Description("List All Virtual Networks in the Subscription")]
public static async Task<List<string>> funListAllVirtualNetworks(string pSubscriptionId)
{
    var vResult = new List<string>();
    try
    {
        var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
        var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
        var vResourceGroups = vSubscription.GetResourceGroups();

        await foreach (var vRg in vResourceGroups)
        {
            var vVnets = vRg.GetVirtualNetworks();
            await foreach (var vVnet in vVnets)
            {
                var vData = vVnet.Data;
                var vAddressPrefixes = string.Join(", ", vData.AddressSpace.AddressPrefixes);
                vResult.Add($"{vData.Name} ({vData.Id}) - AddressPrefixes: {vAddressPrefixes}");
            }
        }
    }
    catch (Exception vEx)
    {
        vResult.Add($"[ERROR] {vEx.Message}");
    }

    return vResult;
}

}