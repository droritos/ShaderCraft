Shader "Custom/CRT_BrushCode"
{
    Properties
    {
        _BrushSize ("Brush Size", Float) = 0.05
        _PaintValue ("Paint Value", Float) = 0.0
        _HitUV ("Hit UV", Vector) = (-1,-1,0,0)
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
            float _PaintValue;
            float4 _HitUV;

            float4 frag(v2f_customrendertexture IN) : SV_Target
            {
                // Read previous frame
                float oldValue = tex2D(_SelfTexture2D, IN.localTexcoord.xy).r;

                // Distance from brush center
                float dist = distance(IN.localTexcoord.xy, _HitUV.xy);

                // Create circular brush
                float brush = 1 - smoothstep(_BrushSize * 0.5, _BrushSize, dist); 
                
                // Paint over previous frame
                float result = oldValue - brush * 0.05;
                result = saturate(result);
                return float4(result, result, result, 1);
            }

            ENDCG
        }
    }
}