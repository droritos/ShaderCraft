Shader "Custom/ShaveShader"
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
                // We use nointerpolation because the ID shouldn't blend between vertices
                nointerpolation uint primitiveID : TEXCOORD1; 
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
                float3 Color;
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
            
                if (triData.lifetime < 3)
                {
                    float scale = saturate(triData.lifetime / 3.0); 
                    for (int i = 0; i < 3; i++)
                    {
                        Varyings OUT;
                        float3 localPos = edges[i].positionOS.xyz * scale;
                        float3 worldPos = TransformObjectToWorld(localPos);
                        worldPos += triData.positionOffset;
            
                        OUT.positionHCS = TransformWorldToHClip(worldPos);
                        OUT.uv = edges[i].uv;
                        OUT.primitiveID = primitiveID; // Pass the ID here
                        triStream.Append(OUT);
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Varyings OUT;
                        OUT.positionHCS = TransformObjectToHClip(edges[i].positionOS.xyz);
                        OUT.uv = edges[i].uv;
                        OUT.primitiveID = primitiveID; // Pass the ID here
                        triStream.Append(OUT);
                    }
                }
            }
                                    
            half4 frag(Varyings IN) : SV_Target 
            {
                // Use IN.primitiveID (the one we passed from the geom shader)
                float3 triangleColor = _TriangleBuffer[IN.primitiveID].Color;
                
                half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                
                // We multiply by triangleColor to see the 'Paint' or 'Shave' effect
                return texColor * _BaseColor * float4(triangleColor, 1.0);
            }
            
            ENDHLSL
        }
    }
}