using System.Numerics;
using Brutal.GlfwApi;
using Brutal.Numerics;

namespace HelloBrutal;

public sealed class MeshViewer : IDisposable
{
    private readonly GlfwWindow _window;
    private readonly IRenderer _renderer;
    private readonly OrbitCamera _camera = new();

    private bool _orbiting;
    private bool _panning;
    private double2 _lastCursor;

    public MeshViewer(GlfwWindow window)
    {
        _window = window;
        _renderer = OperatingSystem.IsMacOS()
            ? new OpenGlRenderer(window)
            : new VulkanRenderer(window);
        _window.OnScroll += (_, offset) => _camera.Zoom(offset.Y);

        Console.WriteLine("Controls: [Tab] import a model, left-drag to orbit, shift+left-drag to pan, scroll to zoom, [Esc] to quit.");
    }

    public void Frame()
    {
        var framebufferSize = _window.FramebufferSize;
        if (framebufferSize.X == 0 || framebufferSize.Y == 0)
            return; // minimised

        _renderer.Resize(framebufferSize.X, framebufferSize.Y);
        HandleInput();

        var aspect = _renderer.Height > 0 ? (float)_renderer.Width / _renderer.Height : 1f;
        _renderer.Draw(_camera.ViewProjection(aspect));
    }

    private void HandleInput()
    {
        if (_window.GetKey(GlfwKey.Escape) == GlfwKeyAction.Press)
        {
            _window.ShouldClose = true;
        }

        if (_window.GetKey(GlfwKey.Tab) == GlfwKeyAction.Press)
        {
            LoadMesh();
        }

        _camera.Zoom(KeyboardZoom());

        var cursor = _window.CursorPos;
        var deltaX = cursor.X - _lastCursor.X;
        var deltaY = cursor.Y - _lastCursor.Y;

        var shiftHeld = _window.GetKey(GlfwKey.LeftShift) != GlfwKeyAction.Release;
        var dragging = _window.GetMouseButton(GlfwMouseButton.Number1) == GlfwButtonAction.Press;

        if (dragging && shiftHeld)
        {
            // Only apply the pan from the second frame of the drag so the view doesn't jump when the mode switches mid-gesture.
            if (_panning)
                _camera.Pan(deltaX, deltaY);
            _panning = true;
            _orbiting = false;
        }
        else if (dragging)
        {
            if (_orbiting)
                _camera.Orbit(deltaX, deltaY);
            _orbiting = true;
            _panning = false;
        }
        else
        {
            _orbiting = false;
            _panning = false;
        }

        _lastCursor = cursor;
    }

    private double KeyboardZoom()
    {
        var zoom = 0.0;
        if (_window.GetKey(GlfwKey.Equal) != GlfwKeyAction.Release)
            zoom += 1.0;
        if (_window.GetKey(GlfwKey.Minus) != GlfwKeyAction.Release)
            zoom -= 1.0;
        return zoom;
    }

    private void LoadMesh()
    {
        var mesh = MeshImport.OpenDialogAndLoad();
        if (mesh is null)
            return;

        _renderer.SetMesh(mesh);
        _camera.Frame(BoundingRadius(mesh));
    }

    private static float BoundingRadius(MeshData mesh)
    {
        var radiusSquared = 0f;
        foreach (var vertex in mesh.Vertices)
        {
            var distanceSquared =
                vertex.Position.X * vertex.Position.X
                + vertex.Position.Y * vertex.Position.Y
                + vertex.Position.Z * vertex.Position.Z;
            radiusSquared = MathF.Max(radiusSquared, distanceSquared);
        }

        return MathF.Sqrt(radiusSquared);
    }

    public void Dispose() => _renderer.Dispose();
}
