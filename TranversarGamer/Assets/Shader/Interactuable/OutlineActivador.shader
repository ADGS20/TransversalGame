Shader "Custom/OutlineActivador"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _OutlineThickness ("Outline Thickness", Range(0, 0.5)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry+1" }
        
        Pass
        {
            Name "Outline"
            Cull Front // Dibuja hacia afuera
            ZWrite On
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
            };

            float _OutlineThickness;
            float4 _OutlineColor;

            Varyings vert(Attributes IN) {
                Varyings OUT;
                
                // --- LA MAGIA ESTÁ AQUÍ ---
                // Calculamos la dirección desde el centro (0,0,0) hacia el vértice
                float3 direccionDesdeCentro = normalize(IN.positionOS.xyz);
                
                // Empujamos el vértice en esa dirección (las esquinas se mantienen pegadas)
                float3 pos = IN.positionOS.xyz + direccionDesdeCentro * _OutlineThickness;
                
                OUT.positionHCS = TransformObjectToHClip(pos);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}