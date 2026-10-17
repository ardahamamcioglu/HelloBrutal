namespace HelloBrutal;

internal static class Shaders
{
    /// <summary>
    /// Vulkan vertex shader. Transforms by the push-constant MVP and remaps OpenGL-style clip space
    /// (Y up, Z in [-1, 1]) to Vulkan clip space (Y down, Z in [0, 1]).
    /// </summary>
    public const string Vertex = """
        #version 450

        layout(location = 0) in vec3 inPosition;
        layout(location = 1) in vec3 inNormal;

        layout(push_constant) uniform PushConstants
        {
            mat4 mvp;
        } pc;

        layout(location = 0) out vec3 vNormal;

        void main()
        {
            vec4 clip = pc.mvp * vec4(inPosition, 1.0);
            clip.y = -clip.y;
            clip.z = (clip.z + clip.w) * 0.5;
            gl_Position = clip;
            vNormal = inNormal;
        }
        """;

    public const string Fragment = """
        #version 450

        layout(location = 0) in vec3 vNormal;
        layout(location = 0) out vec4 outColor;

        void main()
        {
            vec3 n = normalize(vNormal);
            vec3 lightDir = normalize(vec3(0.35, 0.75, 0.55));
            float lambert = max(dot(n, lightDir), 0.0);
            float ambient = 0.25;
            vec3 base = vec3(0.85, 0.80, 0.72);
            outColor = vec4(base * (ambient + lambert), 1.0);
        }
        """;
}
