using Brutal.GlfwApi;
using Brutal.NativeFileDialogApi;
using Brutal.Numerics;
using HelloBrutal;

if (!Glfw.Init())
{
    throw new Exception("Failed to initialize GLFW");
}

// On macOS, locate an installed Vulkan loader/driver so no environment setup is needed.
// This might be unnecessary later. MoltenVK will be bundled with the application.
VulkanRuntime.Discover();

if(!Glfw.VulkanSupported())
{
	throw new Exception("Failed to initialize Vulkan");
}
// Prefer Vulkan wherever the platform provides a loader/driver (including macOS via a
// translation layer such as MoltenVK). Only fall back to OpenGL where it does not.

Glfw.WindowHint(GlfwWindowHint.ClientApi, Glfw.Constants.GLFW_NO_API);

NativeFileDialog.Initialize();

using var window = Glfw.CreateWindow(
    new GlfwWindow.CreateInfo
    {
        Title = "BRUTAL mesh viewer",
        Size = new int2(1280, 720),
    }
);

try
{
    using var viewer = new MeshViewer(window);

    while (!window.ShouldClose)
    {
        Glfw.PollEvents();
        viewer.Frame();
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine();
    Console.Error.WriteLine(ex);
    Console.Error.WriteLine();
    Console.Error.WriteLine(VulkanLoaderHint());
    throw;
}
finally
{
    NativeFileDialog.Shutdown();
    Glfw.Terminate();
}

static string VulkanLoaderHint() =>
    "Ensure a Vulkan loader/driver is installed. The app searches VULKAN_SDK, ~/VulkanSDK and common "
    + "package-manager paths; set VK_DRIVER_FILES and DYLD_LIBRARY_PATH to point at an installation directly.";
