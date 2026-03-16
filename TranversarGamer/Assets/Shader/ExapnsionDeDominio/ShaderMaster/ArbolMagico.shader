Shader "Custom/ArbolMagicoURP"
{
    Properties
    {
        _MainTex ("Textura (Albedo)", 2D) = "white" {}
        _Color ("Color Base", Color) = (1,1,1,1)
        
        // --- PROPIEDAD DEL SCRIPT DE OPACIDAD ---
        _Opacity ("Opacidad (Obstaculo Inteligente)", Range(0,1)) = 1.0

        // --- PROPIEDADES DE LA MAGIA ---
        [HDR] _ColorBorde ("Color del Laser", Color) = (0, 1, 0, 1)
        _GrosorBorde ("Grosor del Laser", Float) = 0.3
        
        [Enum(Visible_Muere_Con_C, 0, Visible_Muere_Con_V, 1, Invisible_Nace_Con_V, 2, Invisible_Nace_Con_C, 3)] _TipoMagia ("Comportamiento Magico", Float) = 0
    }
    SubShader
    {
        // Etiquetas especiales para URP
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off // Para ver las hojas por ambos lados

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // Librería principal de URP obligatoria
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 positionWS  : TEXCOORD1;
            };

            // Definición de texturas en URP
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _Opacity;
                float4 _ColorBorde;
                float _GrosorBorde;
                float _TipoMagia;
            CBUFFER_END

            // Variables Globales controladas por tu script ZonaOndaMagica.cs
            float _RadioCreacion;
            float _RadioCorrupcion;
            float4 _PosOndaCreacion;
            float4 _PosOndaCorrupcion;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                // Transformaciones URP
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float distC = distance(IN.positionWS, _PosOndaCreacion.xyz);
                float distK = distance(IN.positionWS, _PosOndaCorrupcion.xyz);
                float mascara = 0;
                float distLaser = 0;
                float radLaser = 0;

                // --- LÓGICA DE LAS 4 LISTAS ---
                if (_TipoMagia < 0.5) {
                    // 0: Visible Natural (Muere con C)
                    mascara = distK - _RadioCorrupcion;
                    distLaser = distK; radLaser = _RadioCorrupcion;
                } else if (_TipoMagia < 1.5) {
                    // 1: Visible Corrupto (Muere con V)
                    mascara = distC - _RadioCreacion;
                    distLaser = distC; radLaser = _RadioCreacion;
                } else if (_TipoMagia < 2.5) {
                    // 2: Invisible Natural (Nace con V)
                    mascara = _RadioCreacion - distC;
                    distLaser = distC; radLaser = _RadioCreacion;
                } else {
                    // 3: Invisible Corrupto (Nace con C)
                    mascara = _RadioCorrupcion - distK;
                    distLaser = distK; radLaser = _RadioCorrupcion;
                }

                // 1. MAGIA DE RECORTE (Invisibilidad por láser)
                clip(mascara);

                // 2. TEXTURA Y OPACIDAD DE CÁMARA
                half4 colorFinal = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;
                colorFinal.a *= _Opacity; // Tu script ObstaculoInteligente.cs controla esto

                // 3. LÁSER DE CORTE (Brillo neón)
                if (abs(distLaser - radLaser) < _GrosorBorde && radLaser > 0.1)
                {
                    colorFinal.rgb = _ColorBorde.rgb; 
                }

                return colorFinal;
            }
            ENDHLSL
        }
    }
}