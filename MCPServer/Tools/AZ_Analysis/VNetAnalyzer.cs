using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Network;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPServer.Tools;

[McpServerToolType]

public static class VNetAnalyzerTools
{
    [McpServerTool, Description("Analyze Virtual Network(vnet) in a resource group and return deep insights with best-practice evaluation.")]
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
                vResult.Add($"\nVirtual Network: {vVNet_Data.Name}");
                vResult.Add($"- Location: {vVNet_Data.Location}");
                vResult.Add($"- DNS Servers: {(vVNet_Data.DhcpOptionsDnsServers?.Count > 0 ? string.Join(", ", vVNet_Data.DhcpOptionsDnsServers) : "Default (Azure)")}");
                vResult.Add($"- Address Spaces: {string.Join(", ", vVNet_Data.AddressPrefixes)}");

                // DDoS
                vResult.Add(vVNet_Data.EnableDdosProtection == true ? "DDoS Protection: Enabled" : "DDoS Protection: Not Enabled");

                // Peering
                if (vVNet_Data.VirtualNetworkPeerings.Count > 0)
                {
                    foreach (var peering in vVNet_Data.VirtualNetworkPeerings)
                    {
                        vResult.Add($"Peering: {peering.Name} to {peering.RemoteVirtualNetworkId}");
                        vResult.Add($"   - Allow Forwarded Traffic: {peering.AllowForwardedTraffic}");
                        vResult.Add($"   - Allow Gateway Transit: {peering.AllowGatewayTransit}");
                        vResult.Add($"   - Use Remote Gateway: {peering.UseRemoteGateways}");
                    }
                }
                else
                {
                    vResult.Add("Peering: Not Configured");
                }

                // Subnets
                if (vVNet_Data.Subnets.Count == 0)
                {
                    vResult.Add("Subnets: None Defined");
                }
                else
                {
                    foreach (var vSubnet in vVNet_Data.Subnets)
                    {
                        vResult.Add($"- Subnet: {vSubnet.Name} ({string.Join(", ", vSubnet.AddressPrefix)})");

                        vResult.Add(vSubnet.NetworkSecurityGroup != null ? "NSG: Associated" : "NSG: Not Associated");
                        vResult.Add(vSubnet.RouteTable != null ? "Route Table: Associated" : " Route Table: Not Associated");
                        vResult.Add(vSubnet.PrivateEndpointNetworkPolicy == "Disabled" ? "Private Endpoint Policies: Configured" : "Private Endpoint Policies: Not Configured");

                        // Detect wide open subnet
                        if (vSubnet.AddressPrefixes != null && vSubnet.AddressPrefixes.Any(p => p == "0.0.0.0/0"))
                            vResult.Add("Subnet exposes all traffic (0.0.0.0/0)");
                    }
                }

            }

            var vVnetBps = await KnowledgeBaseTools.funGetBestPracticesByResourceType("vnet");
            vResult.Add("\n Best Practices (VNet):");
            vResult.AddRange(vVnetBps);

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
            vResult.Add($"[ERROR] VM Analysis Failed: {ex.Message}");
        }

        return vResult;
    }
}
