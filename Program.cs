using Brutal.GlfwApi;
using Brutal.NativeFileDialogApi;
using Brutal.Numerics;
using HelloBrutal;

if (!Glfw.Init())
    throw new Exception("Failed to initialize GLFW");

var useOpenGl = OperatingSystem.IsMacOS();

if (useOpenGl)
{
    Glfw.WindowHint(GlfwWindowHint.ClientApi, Glfw.Constants.GLFW_OPENGL_API);
    Glfw.WindowHint(GlfwWindowHint.ContextVersionMajor, 3);
    Glfw.WindowHint(GlfwWindowHint.ContextVersionMinor, 3);
    Glfw.WindowHint(GlfwWindowHint.OpenGlProfile, Glfw.Constants.GLFW_OPENGL_CORE_PROFILE);
    Glfw.WindowHint(GlfwWindowHint.OpenGlForwardCompat, 1);
}
else
{
    if (!Glfw.VulkanSupported())
        throw new Exception("GLFW reports that Vulkan is not available.\n" + VulkanLoaderHint());

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
