using Azure;
using Azure.Core;
using Azure.ResourceManager.Network;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Compute;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MCPServer.Tools;

[McpServerToolType]

public static class VMAnalyzerTools
{
    [McpServerTool, Description("Analyze Virtual Machines in a resource group and return VM deep insights with best-practice evaluation.")]
    public static async Task<List<string>> funAnalyzeVMs(string pSubscriptionId, string pResourceGroupName)
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
            var vVmList = vResourceGroup.GetVirtualMachines();

            await foreach (var vVm in vVmList)
            {
                var vVmData = vVm.Data;
                vResult.Add($"VirtualMachine: {vVmData.Name}");
                vResult.Add($"- VM Size: {vVmData.HardwareProfile?.VmSize}");
                vResult.Add($"- OS Type: {vVmData.StorageProfile?.OSDisk?.OSType}");
                vResult.Add($"- Location: {vVmData.Location}");
                vResult.Add($"- Public IP Assigned: {(vVmData.NetworkProfile?.NetworkInterfaces.Any() == true ? "Yes" : "No")}");
                vResult.Add($"- Admin Username: {vVmData.OSProfile?.AdminUsername}");

                // Disk Encryption
                if (vVmData.StorageProfile?.OSDisk?.EncryptionSettings != null)
                    vResult.Add("Disk Encryption: Enabled");
                else
                    vResult.Add("Disk Encryption: Not Enabled");

                // Managed Identity
                if (vVmData.Identity?.ManagedServiceIdentityType != null)
                    vResult.Add("Managed Identity: Configured");
                else
                    vResult.Add("Managed Identity: Not Configured");

                // NIC and Public IP
                if (vVmData.NetworkProfile?.NetworkInterfaces.Count > 0)
                {
                    foreach (var nicRef in vVmData.NetworkProfile.NetworkInterfaces)
                    {
                        var nicId = new ResourceIdentifier(nicRef.Id);
                        var nic = await vArmClient.GetNetworkInterfaceResource(nicId).GetAsync();
                        var ipConfig = nic.Value.Data.IPConfigurations.FirstOrDefault();

                        if (ipConfig?.PublicIPAddress != null)
                        {
                            var pipId = new ResourceIdentifier(ipConfig.PublicIPAddress.Id);
                            var pip = await vArmClient.GetPublicIPAddressResource(pipId).GetAsync();
                            vResult.Add($"- Public IP: {pip.Value.Data.IPAddress}");
                        }
                        else
                        {
                            vResult.Add("Public IP: None detected");
                        }

                        // NSG
                        if (nic.Value.Data.NetworkSecurityGroup != null)
                        {
                            vResult.Add("NSG: Associated");
                        }
                        else
                        {
                            vResult.Add("NSG: Not Associated");
                        }
                    }
                }

                // Boot Diagnostics
                vResult.Add(vVmData.BootDiagnostics.Enabled == true ? "Boot Diagnostics: Enabled" : "Boot Diagnostics: Disabled");

                // Availability
                if (!string.IsNullOrEmpty(vVmData.AvailabilitySetId) || vVmData.Zones.Count > 0)
                    vResult.Add("Availability Zone/Set: Configured");
                else
                    vResult.Add("Availability Zone/Set: Not Configured");

                // Tags Check
                if (vVmData.Tags != null && vVmData.Tags.Count > 0)
                    vResult.Add("Tags: Present");
                else
                    vResult.Add("Tags: Missing (owner, env, cost center)");

            }

            var vBestPractices = await KnowledgeBaseTools.funGetBestPracticesByResourceType("vm");
            vResult.Add("\nBest Practices (VM):");
            vResult.AddRange(vBestPractices);

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
