Shader "Custom/CRT_ColorBrush"
{
    Properties
    {
        _BrushSize ("Brush Size", Float) = 0.1
        _HitUV ("Hit UV", Vector) = (-1, -1, 0, 0)
        
        // This gives us a nice Color Picker in the Unity Inspector
        _PaintColor ("Paint Color", Color) = (1, 0, 0, 1) 
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Lighting Off
        Blend One Zero

        Pass
        {
            Name "Update"

            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"

            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag

            float _BrushSize;
            float4 _HitUV;
            float4 _PaintColor;

            float4 frag(v2f_customrendertexture IN) : SV_Target
            {
                // 1. Read the color from the previous frame
                float4 oldColor = tex2D(_SelfTexture2D, IN.localTexcoord.xy);

                // 2. Calculate the distance from the brush center
                float dist = distance(IN.localTexcoord.xy, _HitUV.xy);

                // 3. Create the soft circular brush shape
                float brush = 1 - smoothstep(_BrushSize * 0.5, _BrushSize, dist); 
                
                float4 finalColor = lerp(oldColor, _PaintColor, brush);
                
                return finalColor;
            }
            ENDCG
        }
    }
}