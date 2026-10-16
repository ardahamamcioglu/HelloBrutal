using Brutal.Numerics;

namespace HelloBrutal;

/// <summary>The minimal surface the mesh viewer needs from a graphics backend.</summary>
public interface IRenderer : IDisposable
{
    int Width { get; }
    int Height { get; }

    void SetMesh(MeshData data);

    void Resize(int width, int height);

    void Draw(float4x4 mvp);
}
