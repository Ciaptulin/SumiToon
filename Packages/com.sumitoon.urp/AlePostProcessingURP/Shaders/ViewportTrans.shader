// ============================================================
// 文件：ViewportTrans.shader
// 用途：视口变换后处理：画面平移/缩放
// 依赖：URP Core.hlsl；AlePostProcessingURP/ShaderLibrary/Core.hlsl
// 作者：AleFeng（原作者）    最后修改：2026-11-02
// 许可：MIT
// ============================================================
Shader "Hidden/Custom/ViewportTrans"
{
HLSLINCLUDE
    // URP 16: 必须先引入 URP Core.hlsl，否则 TEXTURE2D_X 等宏在 Blit.hlsl 中未定义
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    // This file contains the "Luminance" which we use to get the grayscale value
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
	#include "Packages/com.sumitoon.urp/AlePostProcessingURP/ShaderLibrary/Core.hlsl"

    TEXTURE2D_X(_MainTex);

    float4 _ViewportRect;

    PostProcessVaryings Vert(FullScreenTrianglePostProcessAttributes v)
    {
        PostProcessVaryings o;
        UNITY_SETUP_INSTANCE_ID(v);
    	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        o.positionCS = GetFullScreenTriangleVertexPosition(v.vertexID);
        o.texcoord = GetFullScreenTriangleTexCoord(v.vertexID);

        //uv偏移
        o.texcoord = o.texcoord - _ViewportRect.xy;
        //nv缩放
        o.texcoord = o.texcoord / _ViewportRect.zw;

        return o;
    }

    float4 Frag(PostProcessVaryings i) : SV_Target
    {
        float4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.texcoord);
        return color;
    }
ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
        HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
        ENDHLSL
        }
    }
}
