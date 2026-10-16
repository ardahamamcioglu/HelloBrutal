using Brutal.GlfwApi;
using Brutal.Numerics;
using Brutal.OpenGlApi;

namespace HelloBrutal;

public sealed unsafe class OpenGlRenderer : IRenderer
{
    private const int VertexStride = 24; // float3 position + float3 normal

    private readonly GlfwWindow _window;

    private GlVertexArray _vertexArray;
    private GlBuffer _vertexBuffer;
    private GlBuffer _indexBuffer;
    private GlProgram _program;
    private int _mvpLocation = -1;
    private int _indexCount;
    private int _width;
    private int _height;

    public OpenGlRenderer(GlfwWindow window)
    {
        _window = window;
        window.MakeContextCurrent();
        Glfw.SwapInterval(1);

        // GLAD loads the GL function pointers through GLFW.
        if (!Glad.Initialize(name => Glfw.PInvoke.GetProcAddress(name)))
            throw new Exception("Failed to initialize OpenGL (GLAD).");

        _program = CreateProgram();
        _mvpLocation = _program.GetUniformLocation("mvp"u8);

        _vertexArray = OpenGl.GenVertexArray();
        _vertexArray.Bind();
        _vertexBuffer = OpenGl.GenBuffer();
        _indexBuffer = OpenGl.GenBuffer();

        OpenGl.Enable(GlCapability.DepthTest);
        OpenGl.DepthFunc(GlFunc.LessEqual);

        var framebufferSize = window.FramebufferSize;
        Resize(framebufferSize.X, framebufferSize.Y);
    }

    public int Width => _width;
    public int Height => _height;

    public void SetMesh(MeshData data)
    {
        if (data.Vertices.Length == 0 || data.Indices.Length == 0)
        {
            _indexCount = 0;
            Console.WriteLine($"'{data.Name}' contains no indexed triangles.");
            return;
        }

        _vertexArray.Bind();

        _vertexBuffer.Bind(GlBindTarget.ArrayBuffer);
        OpenGl.BufferData(GlBindTarget.ArrayBuffer, data.Vertices, GlBufferUsage.StaticDraw);

        _indexBuffer.Bind(GlBindTarget.ElementArrayBuffer);
        OpenGl.BufferData(GlBindTarget.ElementArrayBuffer, data.Indices, GlBufferUsage.StaticDraw);

        OpenGl.VertexAttributes([
            new GlAttribute(0, 3, OpenGl.GL_FLOAT, GlScaling.Float, VertexStride, 0),
            new GlAttribute(1, 3, OpenGl.GL_FLOAT, GlScaling.Float, VertexStride, 12),
        ]);
        OpenGl.EnableVertexAttribArray(0);
        OpenGl.EnableVertexAttribArray(1);

        _indexCount = data.Indices.Length;
        Console.WriteLine($"Loaded '{data.Name}': {data.Vertices.Length} vertices, {data.TriangleCount} triangles.");
    }

    public void Resize(int width, int height)
    {
        _width = width;
        _height = height;
        OpenGl.Viewport(0, 0, width, height);
    }

    public void Draw(float4x4 mvp)
    {
        var framebufferSize = _window.FramebufferSize;
        if (framebufferSize.X <= 0 || framebufferSize.Y <= 0)
            return;

        Resize(framebufferSize.X, framebufferSize.Y);

        OpenGl.ClearColor(new float4(0.05f, 0.06f, 0.08f, 1f));
        OpenGl.Clear(GlClearBit.Color | GlClearBit.Depth);

        if (_indexCount > 0)
        {
            _program.Use();
            OpenGl.Uniform(_mvpLocation, mvp, false);
            _vertexArray.Bind();
            OpenGl.DrawIndexed(GlTopology.Triangles, GlIndexType.UInt, _indexCount, 1, 0, 0, 0);
        }

        _window.SwapBuffers();
    }

    private static GlProgram CreateProgram()
    {
        var vertex = OpenGl.CreateShader(GlShaderType.Vertex).SetSource(Shaders.OpenGlVertex).Compile();
        var fragment = OpenGl.CreateShader(GlShaderType.Fragment).SetSource(Shaders.OpenGlFragment).Compile();

        var program = OpenGl.CreateProgram().Attach(vertex).Attach(fragment).Link();

        vertex.Dispose();
        fragment.Dispose();
        return program;
    }

    public void Dispose()
    {
        _program.Dispose();
        OpenGl.DeleteBuffers([_vertexBuffer, _indexBuffer]);
        OpenGl.DeleteVertexArrays([_vertexArray]);
    }
}
