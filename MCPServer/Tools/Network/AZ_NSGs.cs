//Network Security Groups (NSGs)
using Azure.ResourceManager.Network;
using Azure.ResourceManager.Network.Models;
using System.ComponentModel;
using System.Diagnostics;
using ModelContextProtocol.Server;


namespace MCPServer.Tools;

[McpServerToolType]
public static class NetworkSecurityGroupTools
{
    [McpServerTool, Description("List Network Security Groups(NSGs) by Resource Group")]
    public static async Task<List<string>> funListNSGsByResourceGroup(string pSubscriptionId, string pResourceGroupName)
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
            var vNSGs = vResourceGroup.GetNetworkSecurityGroups();

            await foreach (var vNSG in vNSGs)
            {
                vResult.Add($"{vNSG.Data.Name} ({vNSG.Data.Id})");
            }
        }
        catch (Exception vEx)
        {
            vResult.Add($"[ERROR] {vEx.Message}");
        }

        return vResult;
    }
    [McpServerTool, Description("List All Network Security Groups in the Subscription")]
    public static async Task<List<string>> funListAllNSGs(string pSubscriptionId)
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroups = vSubscription.GetResourceGroups();

            await foreach (var vRg in vResourceGroups)
            {
                var vNSGs = vRg.GetNetworkSecurityGroups();
                await foreach (var vNSG in vNSGs)
                {
                    vResult.Add($"{vNSG.Data.Name} ({vNSG.Data.Id})");
                }
            }
        }
        catch (Exception vEx)
        {
            vResult.Add($"[ERROR] {vEx.Message}");
        }

        return vResult;
    }

[McpServerTool, Description("List Security Rules in a Network Security Group")]
public static async Task<List<string>> funListNSGRules(string pSubscriptionId, string pResourceGroupName, string pNSGName)
{
    var vResult = new List<string>();
    try
    {
        var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
        var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
        var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
        var vNSG = await vResourceGroup.GetNetworkSecurityGroups().GetAsync(pNSGName);

        foreach (var vRule in vNSG.Value.Data.SecurityRules)
        {
            vResult.Add($"Rule: {vRule.Name} | Priority: {vRule.Priority} | Access: {vRule.Access} | Direction: {vRule.Direction} | Port: {vRule.DestinationPortRange} | Protocol: {vRule.Protocol}");
        }
    }
    catch (Exception vEx)
    {
        vResult.Add($"[ERROR] {vEx.Message}");
    }

    return vResult;
}

}