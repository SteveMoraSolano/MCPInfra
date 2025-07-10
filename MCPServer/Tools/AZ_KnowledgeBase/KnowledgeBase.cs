using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using ModelContextProtocol.Server;
using System.Reflection;

namespace MCPServer.Tools;

[McpServerToolType]
public static class KnowledgeBaseTools
{
    private const string vBasePath = "KnowledgeBase/AZ_BestPractices";

 [McpServerTool, Description(
    "List Best Practices for a specific resource type.\n\n" +
    "Only the following resource types are supported:\n" +
    "- cost-optimization\n" +
    "- vm\n" +
    "- storage\n" +
    "- vnet\n" +
    "- monitoring\n" +
    "- governance\n" +
    "- performance\n" +
    "- appservices\n" +
    "- containers\n" +
    "- keyvault\n" +
    "- functions\n\n" +
    "Please provide one of the above values as input (e.g., 'vm')."
)]
    public static Task<List<string>> funGetBestPracticesByResourceType(string pResourceType)
    {
        var vResult = new List<string>();

        try
        {
            if (string.IsNullOrWhiteSpace(pResourceType))
            {
                vResult.Add("[ERROR] Resource type is required.");
                return Task.FromResult(vResult);
            }

            var vAssembly = Assembly.GetExecutingAssembly();
            var vFileName = $"{pResourceType}.txt";
            var vResourceName = EmbeddedResourceHelper.FindEmbeddedResource(vAssembly, vFileName);
            var vContent = EmbeddedResourceHelper.ReadEmbeddedResource(vAssembly, vResourceName);

            vResult.Add($"[{vFileName}]\n{vContent}");
        }
        catch (Exception Ex)
        {
            vResult.Add($"[ERROR] {Ex.Message}");
        }

        return Task.FromResult(vResult);
    }
}
