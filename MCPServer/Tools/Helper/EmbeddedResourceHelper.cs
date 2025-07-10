using System.Reflection;

public static class EmbeddedResourceHelper
{
    public static string FindEmbeddedResource(Assembly assembly, string fileName)
    {
        var resources = assembly.GetManifestResourceNames();
        foreach (var resource in resources)
        {
            if (resource.EndsWith(fileName, StringComparison.InvariantCultureIgnoreCase))
                return resource;
        }

        throw new FileNotFoundException($"Embedded resource '{fileName}' not found.");
    }

    public static string ReadEmbeddedResource(Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream ?? throw new FileNotFoundException($"Resource stream not found for {resourceName}."));
        return reader.ReadToEnd();
    }
}
