// Unity ShaderLab
// Shader to create a black hole-esque distortion effect sampling what's behind it

Shader "Custom/BlackHoleDistortion"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Center("Center", Vector) = (0.5, 0.5, 0.0, 0.0)
        _DistortionAmount("Distortion Amount", Range(0.0, 1.0)) = 0.1
    }

        SubShader
        {
            Tags { "Queue" = "Transparent" }
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_TexelSize;
                float4 _Center;
                float _DistortionAmount;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    // Calculate the direction from the center to the current pixel
                    float2 dir = i.uv - _Center.xy;
                    float distance = length(dir);

                    // Calculate the distortion based on the distance from the center
                    float distortion = _DistortionAmount * (1.0 / (distance + 0.001));

                    // Offset the UV coordinates
                    float2 distortedUV = i.uv + distortion * dir;

                    // Sample the background texture with the distorted UV coordinates
                    half4 background = tex2D(_MainTex, distortedUV);

                    return background;
                }
                ENDCG
            }
        }
}