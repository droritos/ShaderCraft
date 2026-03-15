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
            
            public const string HairLengthMap = "_HairLengthMap"; // The Texture 
            public const string MaxHairLength = "_MaxHairLength"; // The Float displacement
            public const string HitUV = "_HitUV"; // The Float displacement
        
            // Shader Names
            public const string ShaveShaderName = "Custom/ShaveShader";
        }
    }
}
