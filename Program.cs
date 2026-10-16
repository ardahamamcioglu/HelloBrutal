using Brutal.GlfwApi;
using Brutal.NativeFileDialogApi;
using Brutal.Numerics;
using HelloBrutal;

if (!Glfw.Init())
    throw new Exception("Failed to initialize GLFW");

// Prefer Vulkan wherever the platform provides a loader/driver (including macOS via a
// translation layer such as MoltenVK). Only fall back to OpenGL where it does not.
var useOpenGl = !Glfw.VulkanSupported();

if (useOpenGl)
{
    Console.WriteLine("Vulkan is not available; falling back to the OpenGL renderer.");
    Glfw.WindowHint(GlfwWindowHint.ClientApi, Glfw.Constants.GLFW_OPENGL_API);
    Glfw.WindowHint(GlfwWindowHint.ContextVersionMajor, 3);
    Glfw.WindowHint(GlfwWindowHint.ContextVersionMinor, 3);
    Glfw.WindowHint(GlfwWindowHint.OpenGlProfile, Glfw.Constants.GLFW_OPENGL_CORE_PROFILE);
    Glfw.WindowHint(GlfwWindowHint.OpenGlForwardCompat, 1);
}
else
{
    Glfw.WindowHint(GlfwWindowHint.ClientApi, Glfw.Constants.GLFW_NO_API);
}

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
    using var viewer = new MeshViewer(window, useOpenGl);

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
    if (!useOpenGl)
    {
        Console.Error.WriteLine();
        Console.Error.WriteLine(VulkanLoaderHint());
    }
    throw;
}
finally
{
    NativeFileDialog.Shutdown();
    Glfw.Terminate();
}

static string VulkanLoaderHint() =>
    "Ensure a Vulkan driver/loader is installed and reachable (VK_ICD_FILENAMES / library search path).";
