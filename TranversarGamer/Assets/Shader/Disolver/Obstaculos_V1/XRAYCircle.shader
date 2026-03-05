Shader "Custom/EsferaMascaraStencil"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 0) // Alpha en 0 para que sea invisible
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Geometry-1" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            // EL BLOQUE STENCIL DEBE IR AQUÍ
            Stencil {
                Ref 1
                Comp Always
                Pass Replace
            }

            // Desactivamos la escritura de color y profundidad para que la esfera sea invisible
            ColorMask 0
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
            CBUFFER_END

            Varyings vert(Attributes IN) {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target {
                return _BaseColor;
            }
            ENDHLSL
        }
    }
}