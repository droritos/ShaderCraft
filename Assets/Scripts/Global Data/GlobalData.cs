using UnityEngine;

namespace Global_Data
{
    public static class GlobalData
    {
        public static class ShaderIDs
        {
            // Unity calculates these integers once when the game boots up
            public static readonly int HitUV = Shader.PropertyToID("_HitUV");
            public static readonly int PaintValue = Shader.PropertyToID("_PaintValue");
            public static readonly int PaintColor = Shader.PropertyToID("_PaintColor");
            public static readonly int ShellHeight = Shader.PropertyToID("_ShellHeight");
            public static readonly int BrushSize = Shader.PropertyToID("_BrushSize");
        }
    }
}
    