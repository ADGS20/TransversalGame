Shader "Custom/MagiaUmbralCompleta"
{
    Properties
    {
        _Color ("Color (Filtro)", Color) = (1,1,1,1)
        _MainTex ("Textura Original (Albedo)", 2D) = "white" {}
        _Glossiness ("Brillo (Smoothness)", Range(0,1)) = 0.5
        _Metallic ("Metalizado", Range(0,1)) = 0.0
        
        [HDR] _ColorBorde ("Color del Láser", Color) = (0, 1, 0, 1)
        _GrosorBorde ("Grosor del Láser", Float) = 0.3
        [Enum(Creacion (Aparece con V), 0, Corrupcion (Desaparece con C), 1)] _TipoMagia ("Tipo de Magia", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 200
        Cull Off // Para que las hojas y objetos se vean por ambos lados

        CGPROGRAM
        // Surface shader: Soporta luces, sombras y reflejos automáticamente
        #pragma surface surf Standard fullforwardshadows addshadow
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        
        float4 _ColorBorde;
        float _GrosorBorde;
        float _TipoMagia;

        float _RadioCreacion;
        float _RadioCorrupcion;
        float4 _PosOndaCreacion;
        float4 _PosOndaCorrupcion;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float distancia = 0;
            float radio = 0;
            float mascara = 0;

            // Calculamos la distancia de la onda
            if (_TipoMagia == 0) 
            {
                distancia = distance(IN.worldPos, _PosOndaCreacion.xyz);
                radio = _RadioCreacion;
                mascara = radio - distancia;
            }
            else 
            {
                distancia = distance(IN.worldPos, _PosOndaCorrupcion.xyz);
                radio = _RadioCorrupcion;
                mascara = distancia - radio;
            }

            // TRANSICIÓN SUAVE: Recorta el objeto píxel por píxel
            clip(mascara);

            // Pintamos la textura normal del objeto
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            // Pintamos el Láser justo en el borde del recorte
            if (abs(distancia - radio) < _GrosorBorde && radio > 0.1)
            {
                o.Emission = _ColorBorde.rgb;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}