Shader "Custom/MagiaSoloBorde"
{
    Properties
    {
        [HDR] _ColorBorde ("Color del Borde", Color) = (0, 1, 0, 1)
        _GrosorBorde ("Grosor del Borde", Float) = 0.5
        [Enum(Creacion (Aparece con V), 0, Corrupcion (Desaparece con C), 1)] _TipoMagia ("Tipo de Magia", Float) = 0
    }
    SubShader
    {
        // Etiquetas para que sea totalmente transparente (como un cristal)
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 pos : SV_POSITION; float3 worldPos : TEXCOORD0; };

            float4 _ColorBorde;
            float _GrosorBorde;
            float _TipoMagia;

            float _RadioCreacion;
            float _RadioCorrupcion;
            float4 _PosOndaCreacion;
            float4 _PosOndaCorrupcion;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float distancia = 0;
                float radio = 0;

                if (_TipoMagia == 0) 
                {
                    distancia = distance(i.worldPos, _PosOndaCreacion.xyz);
                    radio = _RadioCreacion;
                }
                else 
                {
                    distancia = distance(i.worldPos, _PosOndaCorrupcion.xyz);
                    radio = _RadioCorrupcion;
                }

                // Dibujamos el láser si está en la frontera de la onda
                if (abs(distancia - radio) < _GrosorBorde && radio > 0.1)
                {
                    return _ColorBorde;
                }

                // Todo lo demás es 100% invisible (no afecta a tu textura original)
                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}