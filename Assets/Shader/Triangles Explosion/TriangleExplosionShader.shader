Shader "Custom/TriangleExplosionShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white"
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma target 4.5 
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2g
            {
                float4 positionOS : TEXCOORD0;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
            CBUFFER_END

            struct TriangleData
            {
                float3 positionOffset;
                float3 velocity;
                float lifetime;
            };

            StructuredBuffer<TriangleData> _TriangleBuffer;

            v2g vert(Attributes IN)
            {
                v2g OUT;
                OUT.positionOS = IN.positionOS; 
                OUT.normalOS = IN.normalOS;     
                OUT.uv = IN.uv;                 
                return OUT;
            }
                        
            [maxvertexcount(3)]
            void geom(triangle v2g edges[3], uint primitiveID : SV_PrimitiveID, inout TriangleStream<Varyings> triStream)
            {
                TriangleData triData = _TriangleBuffer[primitiveID];

                if (triData.lifetime > 0)
                {
                    // 1. Scale based on lifetime (starts at 1.0, goes to 0)
                    float scale = saturate(triData.lifetime / 3.0); 

                    for (int i = 0; i < 3; i++)
                    {
                        Varyings OUT;

                        // Shrink in Local Space
                        float3 localPos = edges[i].positionOS.xyz * scale;
                        // Apply it in World Space
                        float3 worldPos = TransformObjectToWorld(localPos);
                        
                        //  Calculate by the Compute Shader
                        worldPos += triData.positionOffset;

                        // Convert to final Screen Space
                        OUT.positionHCS = TransformWorldToHClip(worldPos);
                        OUT.uv = edges[i].uv;
                        
                        triStream.Append(OUT);
                    }
                }
            }
                        
            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                return color;
            }
            
            ENDHLSL
        }
    }
}