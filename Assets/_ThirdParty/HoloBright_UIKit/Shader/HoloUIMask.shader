// URP용 UI 셰이더 — Panel_Mask_RGBA.png (R=면, G=테두리, B=광택, A=노이즈) 한 장으로
// 색/질감을 마테리얼 파라미터로 제어. UGUI Image 또는 UI Toolkit(RenderTexture/Painter2D 경유)에서 사용.
// 9-slice: Image.type = Sliced 로 쓰면 UV가 잘려 들어오므로 그대로 동작.
Shader "Elder/UI/HoloMask"
{
    Properties
    {
        [PerRendererData] _MainTex ("Mask (RGBA)", 2D) = "white" {}
        _FaceColor    ("Face Color",    Color) = (1,1,1,1)
        _EdgeColor    ("Edge Color",    Color) = (0.36,0.78,0.96,1)
        _GlossColor   ("Gloss Color",   Color) = (1,1,1,0.5)
        _NoiseStrength("Noise Strength", Range(0,1)) = 0.08
        _NoiseTiling  ("Noise Tiling",   Float) = 4
        _Saturation   ("Saturation",     Range(0,2)) = 1
        _Color        ("Tint (Image.color)", Color) = (1,1,1,1)
        // UI 마스킹용
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" "CanUseSpriteAtlas"="True" }
        Stencil { Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] }
        Cull Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _FaceColor, _EdgeColor, _GlossColor, _Color;
                half  _NoiseStrength, _NoiseTiling, _Saturation;
            CBUFFER_END

            struct Attributes { float4 positionOS:POSITION; float2 uv:TEXCOORD0; half4 color:COLOR; };
            struct Varyings   { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; half4 color:COLOR; };

            Varyings Vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;   // Image.color / vertex color 를 그대로 tint 로
                return o;
            }

            half4 Frag(Varyings i) : SV_Target
            {
                half4 m = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half face = m.r, edge = m.g, gloss = m.b;
                half noise = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv * _NoiseTiling).a;

                half3 col = _FaceColor.rgb * i.color.rgb;
                col *= 1.0 - _NoiseStrength * (noise - 0.5);         // 종이/천 질감
                col = lerp(col, _EdgeColor.rgb, edge * _EdgeColor.a);   // 테두리
                col = lerp(col, _GlossColor.rgb, gloss * _GlossColor.a);// 광택
                half lum = dot(col, half3(0.299,0.587,0.114));
                col = lerp(lum.xxx, col, _Saturation);

                half alpha = max(face, edge) * i.color.a * _FaceColor.a;
                return half4(col, alpha);
            }
            ENDHLSL
        }
    }
}
