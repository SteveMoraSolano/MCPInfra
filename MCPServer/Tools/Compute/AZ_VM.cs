using System.ComponentModel;
using System.Diagnostics;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using ModelContextProtocol.Server;

namespace MCPServer.Tools;

[McpServerToolType]
public static class VMTools
{

[McpServerTool, Description("List Virtual Machines by Resource Group Name")]
public static async Task<List<string>> funListVirtualMachinesByResourceGroup(string pSubscriptionId, string pResourceGroupName)
{
    var vResult = new List<string>();
    try
    {
        var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
        var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
        var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
        var vVmCollection = vResourceGroup.GetVirtualMachines();

        await foreach (var vVm in vVmCollection)
        {
            var vVmInfo = vVm.Data;
            vResult.Add($"{vVmInfo.Name} ({vVmInfo.Id}) - Size: {vVmInfo.HardwareProfile?.VmSize} - OS: {vVmInfo.StorageProfile?.OSDisk?.OSType}");
        }
    }
    catch (Exception vEx)
    {
        vResult.Add($"[ERROR] {vEx.Message}");
    }

    return vResult;
}

[McpServerTool, Description("List All Virtual Machines in the Subscription")]
public static async Task<List<string>> funListAllVirtualMachines(string pSubscriptionId)
{
    var vResult = new List<string>();
    try
    {
        var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
        var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
        var vResourceGroups = vSubscription.GetResourceGroups();

        await foreach (var vRg in vResourceGroups)
        {
            var vVmCollection = vRg.GetVirtualMachines();
            await foreach (var vVm in vVmCollection)
            {
                var vVmInfo = vVm.Data;
                vResult.Add($"{vVmInfo.Name} ({vVmInfo.Id}) - Size: {vVmInfo.HardwareProfile?.VmSize} - OS: {vVmInfo.StorageProfile?.OSDisk?.OSType}");
            }
        }
    }
    catch (Exception vEx)
    {
        vResult.Add($"[ERROR] {vEx.Message}");
    }

    return vResult;
}

[McpServerTool, Description("Get VM details by name and resource group")]
public static async Task<List<string>> funGetVirtualMachineDetails(string pSubscriptionId, string pResourceGroupName, string pVmName)
{
    var vResult = new List<string>();
    try
    {
        var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
        var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
        var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
        var vVm = await vResourceGroup.GetVirtualMachines().GetAsync(pVmName);
        var vVmData = vVm.Value.Data;

        vResult.Add($"Name: {vVmData.Name}");
        vResult.Add($"Size: {vVmData.HardwareProfile.VmSize}");
        vResult.Add($"OS Type: {vVmData.StorageProfile.OSDisk.OSType}");
        vResult.Add($"Disk Type: {vVmData.StorageProfile.OSDisk.ManagedDisk?.StorageAccountType}");
        vResult.Add($"Admin Username: {vVmData.OSProfile?.AdminUsername}");
        vResult.Add($"Network Interfaces: {string.Join(", ", vVmData.NetworkProfile?.NetworkInterfaces?.Select(nic => nic.Id.ToString()))}");
    }
    catch (Exception ex)
    {
        vResult.Add($"[ERROR] {ex.Message}");
    }

    return vResult;
}

  [McpServerTool, Description("Deploys a VM.")]
    public static string funDeployVirtualMachine(string pSubscriptionId,string pVmName, string pLocation, string pSshPublicKey)
    {
        try
        {
            var vWorkingDir = Path.Combine(Path.GetTempPath(), $"terraform_{Guid.NewGuid()}");
            Directory.CreateDirectory(vWorkingDir);

            var vTenantId = Environment.GetEnvironmentVariable("AZ_TENANT_ID");
            var vClientId = Environment.GetEnvironmentVariable("AZ_CLIENT_ID");
            var vClientSecret = Environment.GetEnvironmentVariable("AZ_CLIENT_SECRET");

            var vTfContent = @$"
provider ""azurerm"" {{
  features {{}}
  tenant_id       = ""{vTenantId}""
  subscription_id = ""{pSubscriptionId}""
  client_id       = ""{vClientId}""
  client_secret   = ""{vClientSecret}""
}}

resource ""azurerm_resource_group"" ""rg"" {{
  name     = ""rg-{pVmName}""
  location = ""{pLocation}""
}}

resource ""azurerm_virtual_network"" ""vnet"" {{
  name                = ""vnet-{pVmName}""
  address_space       = [""10.0.0.0/16""]
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
}}

resource ""azurerm_subnet"" ""subnet"" {{
  name                 = ""subnet-{pVmName}""
  resource_group_name  = azurerm_resource_group.rg.name
  virtual_network_name = azurerm_virtual_network.vnet.name
  address_prefixes     = [""10.0.1.0/24""]
}}

resource ""azurerm_public_ip"" ""public_ip"" {{
  name                = ""public-ip-{pVmName}""
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  allocation_method   = ""Static""
  sku                 = ""Standard""
}}

resource ""azurerm_network_interface"" ""nic"" {{
  name                = ""nic-{pVmName}""
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name

  ip_configuration {{
    name                          = ""ipconfig1""
    subnet_id                     = azurerm_subnet.subnet.id
    private_ip_address_allocation = ""Dynamic""
    public_ip_address_id          = azurerm_public_ip.public_ip.id
  }}
}}

resource ""azurerm_linux_virtual_machine"" ""vm"" {{
  name                = ""vm-{pVmName}""
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  size                = ""Standard_B1s""
  admin_username      = ""azureuser""

  network_interface_ids = [
    azurerm_network_interface.nic.id,
  ]

  os_disk {{
    caching              = ""ReadWrite""
    storage_account_type = ""Standard_LRS""
    name                 = ""osdisk-{pVmName}""
  }}

  source_image_reference {{
    publisher = ""Canonical""
    offer     = ""UbuntuServer""
    sku       = ""18.04-LTS""
    version   = ""latest""
  }}

  admin_ssh_key {{
    username   = ""azureuser""
    public_key = ""{pSshPublicKey}""
  }}

  disable_password_authentication = true
}}

output ""vm_public_ip"" {{
  value = azurerm_public_ip.public_ip.ip_address
}}
";

            File.WriteAllText(Path.Combine(vWorkingDir, "main.tf"), vTfContent);

            var vInitResult = funRunTerraformCommand("init", vWorkingDir);
            if (!vInitResult.success) return $"[Terraform Init Failed] {vInitResult.output}";

            var vApplyResult = funRunTerraformCommand("apply -auto-approve", vWorkingDir);
            if (!vApplyResult.success) return $"[Terraform Apply Failed] {vApplyResult.output}";

            return $"VM '{pVmName}' deployed in region '{pLocation}'.\n\n{vApplyResult.output}";
        }
        catch (Exception vEx)
        {
            return $"[ERROR] Terraform execution failed: {vEx.Message}";
        }
    }

    private static (bool success, string output) funRunTerraformCommand(string pArgs, string pWorkingDir)
    {
        try
        {
            var vPsi = new ProcessStartInfo
            {
                FileName = "terraform",
                Arguments = pArgs,
                WorkingDirectory = pWorkingDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var vProcess = Process.Start(vPsi);
            vProcess.WaitForExit();

            var vOutput = vProcess.StandardOutput.ReadToEnd() + "\n" + vProcess.StandardError.ReadToEnd();
            return (vProcess.ExitCode == 0, vOutput);
        }
        catch (Exception vEx)
        {
            return (false, vEx.Message);
        }
    }
}
