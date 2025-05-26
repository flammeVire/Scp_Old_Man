Shader "GrainShader"
{
    Properties
    {
        _Intensity("Intensity", Range(0, 1)) = 0.5
        _GrainSize("Grain Size", Float) = 1
        _Speed("Speed", Float) = 1
        _MainTex("MainTex", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Intensity;
            float _GrainSize;
            float _Speed;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                
                // Calcul du décalage temporel pour le bruit
                float timeOffset = _Time.y * _Speed;
                float2 grainUV = uv * _GrainSize + timeOffset;

                // Fonction de bruit pseudo-aléatoire
                float noise = frac(sin(dot(grainUV, float2(12.9898, 78.233))) * 43758.5453);
                float grain = (noise - 0.5) * _Intensity;

                fixed4 col = tex2D(_MainTex, uv);
                col.rgb += grain;

                return col;
            }
            ENDCG
        }
    }
}
