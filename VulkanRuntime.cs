using System.Runtime.InteropServices;

namespace HelloBrutal;

internal static class VulkanRuntime
{
    private const string LoaderFileName = "libvulkan.1.dylib";

    public static void Discover()
    {
        if (Brutal.Platform.CurrentFamily != Brutal.Platform.Family.MacOS)
            return;

        // The environment already names a driver (e.g. setup-env.sh was sourced): leave it alone.
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("VK_DRIVER_FILES")) ||
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("VK_ICD_FILENAMES")))
        {
            return;
        }

        if (FindManifest() is { } manifest)
        {
            Environment.SetEnvironmentVariable("VK_DRIVER_FILES", manifest);
            Environment.SetEnvironmentVariable("VK_ICD_FILENAMES", manifest); // alias read by older loaders
        }

        if (FindLoader() is { } loader)
        {
            try
            {
                NativeLibrary.Load(loader);
            }
            catch (Exception)
            {

            }
        }
    }

    private static string? FindLoader()
    {
        foreach (var directory in LoaderDirectories())
        {
            var candidate = Path.Combine(directory, LoaderFileName);
            if (File.Exists(candidate))
                return candidate;
        }

        return SearchVulkanSdk(LoaderFileName);
    }

    private static string? FindManifest()
    {
        foreach (var directory in ManifestDirectories())
        {
            if (!Directory.Exists(directory))
                continue;
            if (FirstManifest(directory) is { } manifest)
                return manifest;
        }

        return SearchVulkanSdk("*_icd.json", "MoltenVK_icd.json");
    }

    private static string? FirstManifest(string directory)
    {
        try
        {
            var manifests = Directory.EnumerateFiles(directory, "*_icd.json").ToList();
            return manifests.FirstOrDefault(m => Path.GetFileName(m) == "MoltenVK_icd.json")
                ?? manifests.FirstOrDefault();
        }
        catch (IOException)
        {
            return null;
        }
    }

    // The SDK nests everything under a version folder (e.g. ~/VulkanSDK/<version>/macOS/...),
    // so walk it rather than guessing the layout.
    private static string? SearchVulkanSdk(string pattern, params string[] preferredNames)
    {
        var root = Path.Combine(Home, "VulkanSDK");
        if (!Directory.Exists(root))
            return null;

        try
        {
            var matches = Directory.EnumerateFiles(root, pattern, SearchOption.AllDirectories).ToList();
            foreach (var name in preferredNames)
                if (matches.FirstOrDefault(m => Path.GetFileName(m) == name) is { } preferred)
                    return preferred;
            return matches.FirstOrDefault();
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static IEnumerable<string> LoaderDirectories()
    {
        if (Environment.GetEnvironmentVariable("VULKAN_SDK") is { Length: > 0 } sdk)
            yield return Path.Combine(sdk, "lib");

        yield return "/opt/homebrew/lib";
        yield return "/usr/local/lib";
        yield return Path.Combine(Home, "lib");
    }

    private static IEnumerable<string> ManifestDirectories()
    {
        if (Environment.GetEnvironmentVariable("VULKAN_SDK") is { Length: > 0 } sdk)
            yield return Path.Combine(sdk, "share", "vulkan", "icd.d");

        yield return "/opt/homebrew/etc/vulkan/icd.d";
        yield return "/usr/local/etc/vulkan/icd.d";
        yield return "/usr/local/share/vulkan/icd.d";
    }

    private static string Home { get; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
}
