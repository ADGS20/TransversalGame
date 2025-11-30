//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//
// ===========================================================
//  Custom/VolumetricFogURP
//  Volumetric fog por raymarching en URP (post-procesado).
//  - Lee el color de la escena desde _BlitTexture.
//  - Reconstruye la posición en mundo con el depth buffer.
//  - Hace raymarch a lo largo del rayo de cámara -> píxel.
//  - Integra densidad de niebla + luz direccional (god rays).
//  - Usa un volumen de ruido 3D para dar forma a la niebla.
// ===========================================================
Shader "Custom/VolumetricFogURP"
{
    Properties
    {
        // ------------------------------------------
        // Color y densidad base de la niebla
        // ------------------------------------------
        _FogColor        ("Base Fog Color", Color) = (0.7, 0.85, 1.0, 1.0)
        _FogDensity      ("Fog Density", Range(0,1)) = 0.05
        _MaxDistance     ("Max Fog Distance", Float) = 80.0

        // ------------------------------------------
        // Control por altura (fog height)
        // ------------------------------------------
        _FogHeight       ("Fog Base Height", Float) = 0.0
        _FogHeightFalloff("Fog Height Falloff", Float) = 0.2

        // ------------------------------------------
        // Parámetros del raymarch (muestreo del rayo)
        // ------------------------------------------
        _StepSize        ("Step Size", Range(0.5, 20)) = 4.0
        _JitterAmount    ("Step Jitter", Range(0,2)) = 1.0

        // ------------------------------------------
        // Ruido 3D para dar forma a la niebla
        // ------------------------------------------
        _NoiseTex        ("Noise 3D", 3D) = "white" {}
        _NoiseTiling     ("Noise Tiling", Float) = 0.05
        _NoiseThreshold  ("Noise Threshold", Range(0,2)) = 0.7
        _NoiseIntensity  ("Noise Contrast", Float) = 1.0

        // ------------------------------------------
        // Luz / rayos (scattering volumétrico)
        // ------------------------------------------
        _LightColorTint  ("Light Color Tint", Color) = (1.0, 0.9, 0.7, 1.0)
        _LightScattering ("Light Scattering", Range(0,4)) = 1.8
        _Anisotropy      ("Anisotropy g", Range(0,0.95)) = 0.6

        // ------------------------------------------
        // Modo debug (ver sólo la niebla)
        // ------------------------------------------
        [Toggle] _DebugMode ("Debug Fog Only (Gray)", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalRenderPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Pass
        {
            Name "FullScreenPass"

            // Es un efecto de pantalla completa:
            ZWrite Off
            ZTest Always
            Cull Off

            // Mezcla transparente: el alpha lo controlamos nosotros
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            // ------------------------------------------
            // Pragma: funciones de vértice / fragmento
            // ------------------------------------------
            #pragma vertex   Vert
            #pragma fragment Frag

            // ------------------------------------------
            // Sombras de la luz principal (como en Lit de URP)
            // ------------------------------------------
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _SHADOWS_SOFT

            // ------------------------------------------
            // Includes de URP necesarios
            // ------------------------------------------
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            // ------------------------------------------
            // Uniforms (parámetros del shader)
            // ------------------------------------------
            float4 _FogColor;
            float  _FogDensity;
            float  _MaxDistance;
            float  _FogHeight;
            float  _FogHeightFalloff;

            float  _StepSize;
            float  _JitterAmount;

            TEXTURE3D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);
            float  _NoiseTiling;
            float  _NoiseThreshold;
            float  _NoiseIntensity;

            float4 _LightColorTint;
            float  _LightScattering;
            float  _Anisotropy;

            float  _DebugMode;

            // --- Utilidades ----

            // Hash sencillo 2D -> 1 float [0,1]
            float Hash12(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

            // Fase Henyey-Greenstein (para direccionar la luz)
            float HenyeyGreenstein(float cosTheta, float g)
            {
                float g2 = g * g;
                float denom = 1.0 + g2 - 2.0 * g * cosTheta;
                return (1.0 - g2) / (4.0 * PI * denom * sqrt(denom));
            }

            // Densidad del medio en una posición del mundo
            // Combina altura + ruido 3D para dar volumen a la niebla.
            float ComputeDensity(float3 worldPos)
            {
                // Altura: más cerca de _FogHeight => más niebla
                float height = worldPos.y - _FogHeight;
                float heightFactor = exp(-saturate(height) * _FogHeightFalloff);

                float density = _FogDensity * heightFactor;

                // 3D noise para romper uniformidad y crear “nubes”
                if (_NoiseTiling > 0.0001)
                {
                    float3 noiseUV = worldPos * _NoiseTiling;
                    float4 n = SAMPLE_TEXTURE3D(_NoiseTex, sampler_NoiseTex, noiseUV);
                    float noiseVal = dot(n, n); // 0..4 aprox
                    noiseVal = saturate((noiseVal - _NoiseThreshold) * _NoiseIntensity + 0.5);
                    density *= noiseVal;
                }

                return max(density, 0.0);
            }

            // --- Fragmento principal (raymarch volumétrico) ---

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;

                // 1) Color original de la escena desde _BlitTexture
                float4 sceneColor = SAMPLE_TEXTURE2D_X(
                    _BlitTexture, sampler_LinearClamp, uv
                );

                // 2) Profundidad del píxel (0..1)
                real depth01 = SampleSceneDepth(uv);

                // Si es cielo / nada dibujado, no añadimos niebla
                #if UNITY_REVERSED_Z
                    if (depth01 <= 0.0001)
                        return sceneColor; // cielo
                #else
                    depth01 = lerp(UNITY_NEAR_CLIP_VALUE, 1.0, depth01);
                    if (depth01 >= 0.9999)
                        return sceneColor;
                #endif

                // 3) Posición en mundo del píxel usando depth + matriz inversa VP
                float3 worldPos = ComputeWorldSpacePosition(uv, depth01, UNITY_MATRIX_I_VP);
                float3 camPos   = GetCameraPositionWS();

                float3 toPixel  = worldPos - camPos;
                float  distPix  = length(toPixel);
                if (distPix <= 0.01)
                    return sceneColor;

                // Limitamos la distancia máxima del raymarch
                distPix = min(distPix, _MaxDistance);

                // Dirección normalizada de la cámara hacia el píxel
                float3 viewDir  = toPixel / distPix;

                // 4) Configuración del raymarch
                float stepSize = max(_StepSize, 0.1);
                int   maxSteps = min(128, (int)(distPix / stepSize) + 1);

                // Jitter por píxel para reducir banding
                float2 pixelCoords = uv * _ScreenParams.xy;
                float  jitter = (Hash12(pixelCoords + _Time.y) * 2.0 - 1.0) * _JitterAmount;
                float  startDist = saturate(0.5 + 0.5 * jitter) * stepSize;  // 0..stepSize

                float  transmittance = 1.0;     // Beer's law: cuánta luz atraviesa el medio
                float3 fogLightAccum = 0.0;     // luz volumétrica acumulada (god rays)

                // 5) Bucle de raymarch a lo largo del rayo
                [loop]
                for (int i = 0; i < maxSteps; ++i)
                {
                    float currentDist = startDist + stepSize * i;
                    if (currentDist > distPix)
                        break;

                    float3 samplePos = camPos + viewDir * currentDist;

                    // Densidad local de niebla
                    float density = ComputeDensity(samplePos);
                    if (density <= 0.00001)
                        continue;

                    float extinction = density * stepSize;

                    // Luz principal + sombras en el punto sampleado
                    float4 shadowCoord = TransformWorldToShadowCoord(samplePos);
                    Light mainLight    = GetMainLight(shadowCoord);

                    float3 lightDir  = normalize(-mainLight.direction);
                    float  cosTheta  = dot(viewDir, lightDir);
                    float  phase     = HenyeyGreenstein(cosTheta, _Anisotropy);

                    float3 lightCol  = mainLight.color * mainLight.shadowAttenuation;
                    float3 scatterCol = lightCol * _LightColorTint.rgb *
                                        (_LightScattering * phase);

                    // Acumular scattering teniendo en cuenta la transmitancia actual
                    fogLightAccum += transmittance * scatterCol * extinction;

                    // Beer's law: T *= exp(-sigma * ds)
                    transmittance *= exp(-extinction);
                    if (transmittance < 0.01)
                        break;
                }

                // 6) Cantidad final de niebla en este rayo
                float fogAmount = 1.0 - transmittance;
                fogAmount = saturate(fogAmount);

                // Modo debug: ver mapa de niebla en gris
                if (_DebugMode > 0.5)
                {
                    float debugVal = saturate(fogAmount + length(fogLightAccum));
                    return float4(debugVal.xxx, 1.0);
                }

                // Color base de la niebla (sin scattering adicional)
                float3 fogBase = _FogColor.rgb * fogAmount;

                // 7) Mezcla final:
                //    - Escena atenuada por (1 - fogAmount)
                //    - + niebla base
                //    - + luz volumétrica acumulada (god rays)
                float3 finalColor = sceneColor.rgb * (1.0 - fogAmount)
                                  + fogBase
                                  + fogLightAccum;

                return float4(finalColor, sceneColor.a);
            }

            ENDHLSL
        }
    }
}