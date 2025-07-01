using System.ComponentModel;
using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.CosmosDB;
using Azure.ResourceManager.CosmosDB.Models;
using Azure.ResourceManager.Resources;
using ModelContextProtocol.Server;
using MCPServer.Tools;
using System.Net;

[McpServerToolType]

public static class CosmoDBTool
{
    [McpServerTool, Description("List Cosmos DB Account By Resource Group Name")]
    public static async Task<List<string>> funListCosmosAccountByResourceGroup(string pSubscriptionId, string pResourceGroupName)
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
            var vCosmosDBAccount = vResourceGroup.GetCosmosDBAccounts();

            await foreach (var vCA in vCosmosDBAccount)
            {
                vResult.Add($"{vCA.Data.Name} ({vCA.Data.Id})");
            }

        }
        catch (Exception Ex)
        {
            vResult.Add($"[ERROR]{Ex.Message}");
        }

        return vResult;
    }

    [McpServerTool, Description("List All Cosmos DB Account")]
    public static async Task<List<string>> funListAllCosmosAccounts(string pSubscriptionId)
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroups = vSubscription.GetResourceGroups();

            await foreach (var vRg in vResourceGroups)
            {
                var vCosmosDBAccounts = vRg.GetCosmosDBAccounts();
                await foreach (var vCA in vCosmosDBAccounts)
                {
                    vResult.Add($"{vCA.Data.Name} ({vCA.Data.Id})");
                }
            }
        }
        catch (Exception Ex)
        {
            vResult.Add($"[ERROR] {Ex.Message}");
        }

        return vResult;
    }

    [McpServerTool, Description("List databases")]
    public static async Task<List<string>> funListDatabases(string pSubscriptionId, string pResourceGroupName,string pCosmosAccount)
    {
        var vResult = new List<string>();
        try
        {
            var vArmClient = AZ_AuthHelper.funGetArmClient(pSubscriptionId);
            var vSubscription = await vArmClient.GetDefaultSubscriptionAsync();
            var vResourceGroup = vSubscription.GetResourceGroup(pResourceGroupName).Value;
            var vCosmosAccount = vResourceGroup.GetCosmosDBAccount(pCosmosAccount).Value;

            
        if (vCosmosAccount.Data.Kind == CosmosDBAccountKind.GlobalDocumentDB)
        {
            var vDatabases = vCosmosAccount.GetCosmosDBSqlDatabases();
            await foreach (var vDb in vDatabases)
            {
                vResult.Add($"{vDb.Data.Name} ({vDb.Data.Id})");
            }
        }
        else if (vCosmosAccount.Data.Kind == CosmosDBAccountKind.MongoDB)
        {
            var vMongoDatabases = vCosmosAccount.GetMongoDBDatabases();
            await foreach (var vDb in vMongoDatabases)
            {
                vResult.Add($"{vDb.Data.Name} ({vDb.Data.Id})");
            }
        }           
            
        }
        catch (Exception Ex)
        {
            vResult.Add($"[ERROR] {Ex.Message}");
        }
        return vResult;
    }
}