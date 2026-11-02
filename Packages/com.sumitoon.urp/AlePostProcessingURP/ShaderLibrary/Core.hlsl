// ============================================================
// 文件：Core.hlsl
// 用途：后处理通用工具：全屏三角形顶点、_ScreenSize、采样辅助
// 依赖：URP Core.hlsl
// 作者：AleFeng（原作者）    最后修改：2026-11-02
// 许可：MIT
// ============================================================
#ifndef YETMAN_POSTPROCESS_CORE_INCLUDED
#define YETMAN_POSTPROCESS_CORE_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

// Instead of recieving the vertex position, we just receive a vertex id (0,1,2)
// and convert it to a clip-space postion in the vertex shader
struct FullScreenTrianglePostProcessAttributes
{
    uint vertexID : SV_VertexID;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// This is what the fragment program should recieve if you use "FullScreenTrianglePostProcessVertexProgram" as the vertex shader
struct PostProcessVaryings
{
    float4 positionCS : SV_POSITION;
    float2 texcoord   : TEXCOORD0;
    UNITY_VERTEX_OUTPUT_STEREO
};

PostProcessVaryings FullScreenTrianglePostProcessVertexProgram(FullScreenTrianglePostProcessAttributes input)
{
    PostProcessVaryings output;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
    output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
    output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
    return output;
}

#endif