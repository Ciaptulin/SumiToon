// ============================================================
// 文件：ImageGrayscale.shader
// 用途：灰度后处理：可调混合强度
// 依赖：URP Core.hlsl；AlePostProcessingURP/ShaderLibrary/Core.hlsl
// 作者：AleFeng（原作者）    最后修改：2026-11-02
// 许可：MIT
// ============================================================
Shader "Hidden/Custom/ImageGrayscale"
{
HLSLINCLUDE
    // URP 16: 必须先引入 URP Core.hlsl，否则 TEXTURE2D_X 等宏在 Blit.hlsl 中未定义
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    // This file contains the "Luminance" which we use to get the grayscale value
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
	#include "Packages/com.sumitoon.urp/AlePostProcessingURP/ShaderLibrary/Core.hlsl"

    TEXTURE2D_X(_MainTex);
    float4 _MainTex_TexelSize;

    float _Blend;

    float4 Frag(PostProcessVaryings i) : SV_Target
    {
        float4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.texcoord);
        float luminance = dot(color.rgb, float3(0.2126729, 0.7151522, 0.0721750));
        color.rgb = lerp(color.rgb, luminance.xxx, _Blend.xxx);
        return color;
    }
ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
        HLSLPROGRAM
            #pragma vertex FullScreenTrianglePostProcessVertexProgram
            #pragma fragment Frag
        ENDHLSL
        }
    }
}