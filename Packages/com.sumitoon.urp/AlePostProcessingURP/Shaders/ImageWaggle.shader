// ============================================================
// 文件：ImageWaggle.shader
// 用途：画面抖动后处理：横纵偏移 + 缩放抖动
// 依赖：URP Core.hlsl；AlePostProcessingURP/ShaderLibrary/Core.hlsl
// 作者：AleFeng（原作者）    最后修改：2026-11-02
// 许可：MIT
// ============================================================
Shader "Hidden/Custom/ImageWaggle"
{
HLSLINCLUDE
    // URP 16: 必须先引入 URP Core.hlsl，否则 TEXTURE2D_X 等宏在 Blit.hlsl 中未定义
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    // This file contains the "Luminance" which we use to get the grayscale value
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
	#include "Packages/com.sumitoon.urp/AlePostProcessingURP/ShaderLibrary/Core.hlsl"

    TEXTURE2D_X(_MainTex);

    half _HorOffsetRange;
    half _HorOffsetSpeed;
    half _VerOffsetRange;
    half _VerOffsetSpeed;
    half _StartTime;
    half2 _ScaCenter;
    half _ScaStartScale;
    half _ScaEndScale;
    half _ScaTotalTime;

    PostProcessVaryings Vert(FullScreenTrianglePostProcessAttributes v)
    {
        PostProcessVaryings o;
        UNITY_SETUP_INSTANCE_ID(v);
    	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        o.positionCS = GetFullScreenTriangleVertexPosition(v.vertexID);
        o.texcoord = GetFullScreenTriangleTexCoord(v.vertexID);

        //缩放参数
        half2 distance = o.texcoord - _ScaCenter;
        half scale = _ScaStartScale - smoothstep(_StartTime, _StartTime + _ScaTotalTime, _Time.y) * saturate(_ScaStartScale - _ScaEndScale);
        //水平方向缩放
        o.texcoord.x = _ScaCenter.x + distance.x * scale;
        //垂直方向缩放
        o.texcoord.y = _ScaCenter.y + distance.y * scale;
        //水平方向偏移
        half offsetHor = sin(_Time.y * _HorOffsetSpeed) * _HorOffsetRange * scale;
        o.texcoord.x += offsetHor;
        //垂直方向偏移
        half offsetVer = sin(_Time.y * _VerOffsetSpeed) * _VerOffsetRange * scale;
        o.texcoord.y += offsetVer;

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
