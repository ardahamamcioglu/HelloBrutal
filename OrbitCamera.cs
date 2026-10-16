using Brutal.Numerics;

namespace HelloBrutal;

public sealed class OrbitCamera
{
    public float Yaw = 0.7f;
    public float Pitch = 0.45f;
    public float Distance = 3.0f;
    public float3 Target = new(0f, 0f, 0f);

    public float4x4 ViewProjection(float aspectRatio)
    {
        var cosPitch = MathF.Cos(Pitch);
        var eye = new float3(
            Target.X + Distance * cosPitch * MathF.Sin(Yaw),
            Target.Y + Distance * MathF.Sin(Pitch),
            Target.Z + Distance * cosPitch * MathF.Cos(Yaw)
        );

        var view = float4x4.CreateLookAt(eye, Target, new float3(0f, 1f, 0f));
        var projection = float4x4.CreatePerspectiveFieldOfView(MathF.PI / 4f, aspectRatio, 0.05f, 500f);
        return view * projection;
    }

    public void Orbit(double deltaX, double deltaY)
    {
        Yaw -= (float)deltaX * 0.01f;
        Pitch = Math.Clamp(Pitch + (float)deltaY * 0.01f, -1.5f, 1.5f);
    }

    public void Zoom(double amount)
    {
        Distance = Math.Clamp(Distance * (float)Math.Exp(-amount * 0.15), 0.2f, 200f);
    }

    public void Frame(float radius)
    {
        Distance = Math.Clamp(radius * 2.5f, 0.5f, 200f);
    }

    public void Pan(double deltaX, double deltaY)
    {
        var sinYaw = MathF.Sin(Yaw);
        var cosYaw = MathF.Cos(Yaw);
        var sinPitch = MathF.Sin(Pitch);
        var cosPitch = MathF.Cos(Pitch);

        var right = new float3(cosYaw, 0f, -sinYaw);
        var up = new float3(-sinYaw * sinPitch, cosPitch, -cosYaw * sinPitch);

        const float pixelsToWorld = 0.0015f;
        var scale = Distance * pixelsToWorld;
        Target += (up * (float)deltaY - right * (float)deltaX) * scale;
    }
}
