namespace Global_Data
{
    public static class GlobalData
    {
        public static class ShaderProperties
        {
            // Compute Shader Kernels
            public const string MainKernel = "CSMain"; // Or 0 if using indices

            // Buffer Names
            public const string TriangleBuffer = "_TriangleBuffer";

            // Uniforms / Settings
            public const string Gravity = "_Gravity";
            public const string DeltaTime = "_DeltaTime";
        
            // Shader Names
            public const string ShaveShaderName = "Custom/ShaveShader";
        }
    }
}
