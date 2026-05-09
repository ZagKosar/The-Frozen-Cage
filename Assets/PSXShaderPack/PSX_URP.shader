Shader "Custom/PSX_URP_VHS"
{
    Properties
    {
        _PixelSize ("Pixel Size", Range(1,500)) = 160
        _Vignette ("Vignette", Range(0,2)) = 1
        _FishEye ("FishEye", Range(-1,1)) = 0.15
        _ColorDepth ("Color Depth", Range(2,256)) = 32

        _VHSIntensity ("VHS Intensity", Range(0,1)) = 0.5
        _ScanlineStrength ("Scanline Strength", Range(0,1)) = 0.3
        _NoiseStrength ("Noise Strength", Range(0,1)) = 0.2
        _RGBSplit ("RGB Split", Range(0,0.01)) = 0.002
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "PSX_VHS"
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _PixelSize;
            float _Vignette;
            float _FishEye;
            float _ColorDepth;

            float _VHSIntensity;
            float _ScanlineStrength;
            float _NoiseStrength;
            float _RGBSplit;

            float rand(float2 co)
            {
                return frac(sin(dot(co, float2(12.9898,78.233))) * 43758.5453);
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                float2 center = float2(0.5, 0.5);
                float2 offset = uv - center;

                float dist = length(offset);

                // ===== FishEye (плавный)
                float edgeFade = smoothstep(0.9, 0.5, dist);
                float distortion = 1 + _FishEye * dist * dist * edgeFade;
                uv = center + offset * distortion;

                // ===== VHS horizontal wave
                float wave = sin(uv.y * 80 + _Time.y * 5) * 0.003 * _VHSIntensity;
                uv.x += wave;

                // ===== Pixelation
                float2 resolution = _ScreenParams.xy;
                float2 pixelScale = resolution / _PixelSize;
                uv = floor(uv * pixelScale) / pixelScale;

                // ===== RGB Split
                float r = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(_RGBSplit,0)).r;
                float g = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv).g;
                float b = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - float2(_RGBSplit,0)).b;

                float3 color = float3(r,g,b);

                // ===== Color Depth
                color = floor(color * _ColorDepth) / (_ColorDepth - 1);

                // ===== Scanlines
                float scan = sin(uv.y * _ScreenParams.y * 1.5);
                color *= 1 - scan * _ScanlineStrength;

                // ===== Noise
                float noise = rand(uv + _Time.y) * _NoiseStrength;
                color += noise;

                // ===== Vignette
                color *= 1 - dist * _Vignette;

                return float4(color,1);
            }

            ENDHLSL
        }
    }
}