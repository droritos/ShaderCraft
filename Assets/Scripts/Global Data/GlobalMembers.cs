using UnityEngine;

namespace Global_Data
{
    public static class GlobalMembers
    {
        public static class ShaderIDs
        {
            // Unity calculates these integers once when the game boots up
            public static readonly int HitUV = Shader.PropertyToID("_HitUV");
            public static readonly int PaintValue = Shader.PropertyToID("_PaintValue");
            public static readonly int PaintColor = Shader.PropertyToID("_PaintColor");
            public static readonly int ShellHeight = Shader.PropertyToID("_ShellHeight");
            public static readonly int BrushSize = Shader.PropertyToID("_BrushSize");
            
            public static readonly int PlayerCanvas = Shader.PropertyToID("PlayerCanvas");
            public static readonly int TargetCanvas = Shader.PropertyToID("TargetCanvas");
            public static readonly int PlayerColorCanvas = Shader.PropertyToID("PlayerColorCanvas");
            public static readonly int TargetColorCanvas = Shader.PropertyToID("TargetColorCanvas");
        
            public static readonly int ResultBuffer = Shader.PropertyToID("ResultBuffer");
            public static readonly int Tolerance = Shader.PropertyToID("Tolerance");
            public static readonly int Resolution = Shader.PropertyToID("Resolution");
            public static readonly int ScaleFactor = Shader.PropertyToID("ScaleFactor");
        }
    }
}

    