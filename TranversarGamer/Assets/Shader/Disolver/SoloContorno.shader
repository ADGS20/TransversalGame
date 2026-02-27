Shader "Custom/SoloContorno_Stencil"
{
    Properties
    {
        _OutlineColor("Color del Contorno", Color) = (0,0,0,1)
        _OutlineThickness("Grosor", Range(0, 0.1)) = 0.02
    }

    SubShader
    {
        // "Queue"="Geometry+1" para que se dibuje justo después del objeto principal
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry+1" }

        Pass
        {
            Name "OutlineOnly"
            Cull Front // Dibuja las caras de atrás (técnica de casco invertido)
            ZWrite On
            ZTest LEqual

            // Mantenemos tu lógica para que el agujero también corte el contorno
            Stencil
            {
                Ref 1
                Comp NotEqual
                Pass Keep
            }

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
                // Inflamos el modelo hacia afuera usando las normales
                float3 pos = IN.positionOS.xyz + IN.normalOS * _OutlineThickness;
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