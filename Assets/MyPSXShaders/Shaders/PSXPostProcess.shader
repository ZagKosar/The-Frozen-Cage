// Shader "MyPSXShaders/PSXPostProcess"
// {
//     Properties
//     {
//         _MainTex ("Texture", 2D) = "white" {}
//         _Pixelation ("Pixelation", Range(1, 20)) = 4
//         _ColorDepth ("Color Depth", Range(2, 256)) = 255
//         _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.15
//         _VignetteIntensity ("Vignette Intensity", Range(0, 5)) = 1.0
//         _FisheyeStrength ("Fisheye Strength", Range(0, 2)) = 0.0
//     }

//     HLSLINCLUDE
//     #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
//     #pragma target 3.5

//     TEXTURE2D(_MainTex);
//     SAMPLER(sampler_MainTex);
//     float4 _MainTex_TexelSize;

//     float _Pixelation;
//     float _ColorDepth;
//     float _NoiseIntensity;
//     float _VignetteIntensity;
//     float _FisheyeStrength;

//     struct Attributes
//     {
//         float4 positionOS : POSITION;
//         float2 uv : TEXCOORD0;
//     };

//     struct Varyings
//     {
//         float4 positionCS : SV_POSITION;
//         float2 uv : TEXCOORD0;
//     };

//     Varyings Vert(Attributes input)
//     {
//         Varyings output;
//         // Blitter передаёт координаты уже в Clip Space
//         output.positionCS = input.positionOS;
//         output.uv = input.uv;
//         return output;
//     }

//     float4 Frag(Varyings input) : SV_Target
//     {
//         float2 uv = input.uv;

//         // 1. Fisheye (бочкообразное искажение)
//         float2 center = uv - 0.5;
//         float r = length(center);
//         float theta = atan2(center.y, center.x);
//         float newR = r * (1.0 + _FisheyeStrength * r * r);
//         uv = float2(cos(theta), sin(theta)) * newR + 0.5;
//         uv = saturate(uv); // Обрезаем выходящие за границы UV

//         // 2. Pixelation (привязка к сетке пикселей)
//         float2 pixelSize = _Pixelation / _ScreenParams.xy;
//         uv = floor(uv / pixelSize) * pixelSize;

//         // 3. Сэмплинг исходного текстуры
//         float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

//         // 4. Color Depth (квантование цвета)
//         float steps = max(_ColorDepth, 2.0);
//         col.rgb = floor(col.rgb * steps) / steps;

//         // 5. Noise (псевдо-шум зерна)
//         float2 noiseUV = input.uv * 100.0 + _Time.y * 0.05;
//         float noise = frac(sin(dot(noiseUV, float2(12.9898, 78.233))) * 43758.5453);
//         col.rgb += (noise - 0.5) * _NoiseIntensity;

//         // 6. Vignette (затемнение краёв)
//         float2 vigUV = input.uv * (1.0 - input.uv);
//         float vigFactor = pow(vigUV.x * vigUV.y * 15.0, _VignetteIntensity);
//         col.rgb *= vigFactor;

//         return col;
//     }
//     ENDHLSL

//     SubShader
//     {
//         Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
//         ZTest Always ZWrite Off Cull Off

//         Pass
//         {
//             Name "PSXPostProcess"
//             HLSLPROGRAM
//             #pragma vertex Vert
//             #pragma fragment Frag
//             ENDHLSL
//         }
//     }
// }