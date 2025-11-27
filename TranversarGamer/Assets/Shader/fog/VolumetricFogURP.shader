Shader "Custom/VolumetricFogURP"
{
    Properties
    {
        _FogColor      ("Fog Color", Color) = (0.8, 0.9, 1.0, 1.0)
        _FogDensity    ("Fog Density", Float) = 0.05
        _MaxDistance   ("Max Fog Distance", Float) = 80.0

        _FogHeight         ("Fog Base Height", Float) = 0.0
        _FogHeightFalloff  ("Fog Height Falloff", Float) = 0.2

        _NoiseTex      ("Noise 3D", 3D) = "white" {}
        _NoiseScale    ("Noise Scale", Float) = 0.0
        _NoiseSpeed    ("Noise Speed (xyz)", Vector) = (0.0, 0.0, 0.0, 0.0)

        _NumSteps      ("Raymarch Steps", Range(8, 128)) = 32
        _Jitter        ("Jitter", Range(0, 1)) = 0.2
        
        _FogSoftness   ("Fog Softness", Range(0.1, 2.0)) = 0.7
        _FogBrightness ("Fog Brightness", Range(0.5, 3.0)) = 1.5
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
            ZTest Always
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float4 _FogColor;
            float  _FogDensity;
            float  _MaxDistance;

            float  _FogHeight;
            float  _FogHeightFalloff;

            TEXTURE3D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);
            float  _NoiseScale;
            float4 _NoiseSpeed;

            float  _NumSteps;
            float  _Jitter;
            
            float  _FogSoftness;
            float  _FogBrightness;

            float SampleFogNoise(float3 worldPos)
            {
                float3 noiseUV = worldPos * _NoiseScale + _Time.y * _NoiseSpeed.xyz;
                float n = SAMPLE_TEXTURE3D(_NoiseTex, sampler_NoiseTex, noiseUV).r;
                return lerp(0.5, 1.5, n);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // 1) Color original de la escena
                float4 sceneColor = SAMPLE_TEXTURE2D_X(
                    _BlitTexture, sampler_LinearClamp, input.texcoord
                );

                float2 uv = input.texcoord;

                // 2) Profundidad
                #if UNITY_REVERSED_Z
                    real depth01 = SampleSceneDepth(uv);
                    if (depth01 <= 0.0001)
                        return sceneColor;
                #else
                    real depth01 = SampleSceneDepth(uv);
                    depth01 = lerp(UNITY_NEAR_CLIP_VALUE, 1.0, depth01);
                    if (depth01 >= 0.9999)
                        return sceneColor;
                #endif

                // 3) Reconstruir posición en mundo
                float3 worldPos = ComputeWorldSpacePosition(uv, depth01, UNITY_MATRIX_I_VP);

                // 4) Cámara y dirección
                float3 camPosWS = GetCameraPositionWS();
                float3 toPixel = worldPos - camPosWS;
                float  distToPixel = length(toPixel);

                distToPixel = min(distToPixel, _MaxDistance);

                if (distToPixel <= 0.01)
                    return sceneColor;

                float3 dir = toPixel / distToPixel;

                // 5) Raymarching
                int steps = (int)_NumSteps;
                steps = max(1, steps);

                float stepSize = distToPixel / steps;

                // Jitter
                float rand = frac(sin(dot(uv.xy, float2(12.9898, 78.233))) * 43758.5453);
                float jitterOffset = lerp(0.0, stepSize, rand * _Jitter);

                float3 samplePos = camPosWS + dir * jitterOffset;

                float3 fogAccum = 0.0;
                float  transmittance = 1.0;

                // 6) Integración de niebla MEJORADA
                [loop]
                for (int i = 0; i < steps; ++i)
                {
                    // Altura relativa
                    float height = samplePos.y - _FogHeight;
                    float heightFactor = exp(-saturate(height) * _FogHeightFalloff);

                    // Densidad base
                    float density = _FogDensity * heightFactor;

                    // Ruido 3D
                    float noiseFactor = SampleFogNoise(samplePos);
                    density *= noiseFactor;

                    // Extinción SUAVIZADA
                    float extinction = density * stepSize;
                    
                    // Contribución de niebla más visible y brillante
                    float fogAmount = saturate(extinction * _FogBrightness);
                    float3 localFog = _FogColor.rgb * fogAmount;

                    // Acumulamos
                    fogAccum += transmittance * localFog;

                    // Actualizamos transmittance de forma más suave
                    transmittance *= exp(-extinction * _FogSoftness);

                    // Avanzamos
                    samplePos += dir * stepSize;

                    // Early out
                    if (transmittance < 0.01)
                        break;
                }

                // 7) Combinar escena + niebla
                float3 finalColor = sceneColor.rgb * transmittance + fogAccum;

                return float4(finalColor, sceneColor.a);
            }

            ENDHLSL
        }
    }
}