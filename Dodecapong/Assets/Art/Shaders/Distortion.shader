Shader "Dvornik/Distortion"
{
    Properties
    {
        _Refraction("Refraction", Range(-10.00, 10.0)) = 0
        [HideInInspector][PerRendererData]_MainTex("Sprite Texture", 2D) = "white" {}
        _DistortTex("Distort (RG)",2D) = "gray" {}
    }
    SubShader
    {
        Tags{ "RenderType" = "Transparent" "Queue" = "Overlay" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
        // Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members normal)
        #pragma exclude_renderers d3d11
        #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            sampler2D _GrabTexture : register(s0);
            sampler2D _MainTex : register(s2);
            sampler2D _DistortTex : register(s3);
            float _Refraction;

            float4 _DistortTex_TexelSize;

            struct Input {
                float2 uv_MainTex;
                float2 uv_DistortTex;
                float4 screenPos;
                float3 color;
            };

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _MainTex_ST;

            void vert(inout appdata_full v, out Input o)
            {
                UNITY_INITIALIZE_OUTPUT(Input, o);
                o.color = v.color;
            }

            float4 frag(Input IN) : SV_Target
            {
                float4 o;

                float4 main = tex2D(_MainTex, IN.uv_MainTex);
                float4 dist = tex2D(_DistortTex, IN.uv_DistortTex);
                float2 distort = float2(dist.r - 0.5, dist.g - 0.5);
                float2 offset = distort * _Refraction * _DistortTex_TexelSize.xy;
                //float4 bgColor = tex2Dproj(_GrabTexture, IN.screenPos);

                #if UNITY_UV_STARTS_AT_TOP
                IN.screenPos.y = 1 - IN.screenPos.y;
                #endif

                IN.screenPos.xy = offset + IN.screenPos.xy;
                float4 refrColor = tex2Dproj(_GrabTexture, IN.screenPos);
                o = main.rgb * main.a * main.a + (1 - main.a) * refrColor.rgb;
                o.w = 1;
                return o;

                // apply fog
                //UNITY_APPLY_FOG(IN.fogCoord, col);
            }
            ENDCG
        }
    }
}
