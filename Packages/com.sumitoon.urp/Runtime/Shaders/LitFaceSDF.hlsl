// ============================================================
// 文件：LitFaceSDF.hlsl
// 用途：正负图集双混合、平滑对称的 SDF 阴影（终极版）
// ============================================================
#ifndef LIT_FACE_SDF_INCLUDED
#define LIT_FACE_SDF_INCLUDED

#if defined(_FACESDF_ON)

// 从 3x3 图集中采样指定帧
half SampleFaceSDFAtlas(TEXTURE2D_PARAM(tex, samp), float2 uv, float frameF)
{
    float f0 = floor(frameF);
    float f1 = min(f0 + 1.0, 8.0);
    float t = frameF - f0;

    // 安全内缩，防止采到黑边
    float2 tileUV = uv * 0.98 + 0.01; 
    tileUV *= (1.0 / 3.0);

    float col0 = fmod(f0, 3.0);
    float row0 = floor(f0 / 3.0);
    float2 offset0 = float2(col0 / 3.0, (2.0 - row0) / 3.0);
    
    float col1 = fmod(f1, 3.0);
    float row1 = floor(f1 / 3.0);
    float2 offset1 = float2(col1 / 3.0, (2.0 - row1) / 3.0);

    half s0 = SAMPLE_TEXTURE2D(tex, samp, tileUV + offset0).r;
    half s1 = SAMPLE_TEXTURE2D(tex, samp, tileUV + offset1).r;
    
    return lerp(s0, s1, t);
}

half ComputeFaceSDFMask(
    float2 uv,
    float3 lightDirHeadLocal, // 这里直接传入转换到本地的光照方向
    half threshold,
    half softness,
    half angleOffsetDeg,
    half angleScale,
    float debugFrame)
{
    // 在模型本地空间中，Right 是 +X，Forward 是 +Z
    float side  = lightDirHeadLocal.x; 
    float front = lightDirHeadLocal.z;

    // 计算角度和帧数
    float angleRad = atan2(side, front);
    float a = (angleRad + radians(angleOffsetDeg)) * angleScale;
    float angle01 = abs(a) * (8.0 / PI);
    float frameF = min(angle01, 8.0);

    if (debugFrame >= 0) frameF = debugFrame;

    half sdf;
    half th;
    float2 sdfUV = uv;

    // 核心修复：根据光在左还是右，分别采正/负图集
    if (side >= 0.0) 
    {
        // 光在右脸，用 Positive
        sdf = SampleFaceSDFAtlas(TEXTURE2D_ARGS(_TexFaceSDFPositive, sampler_TexFaceSDFPositive), sdfUV, frameF);
        th = SAMPLE_TEXTURE2D(_TexFaceSDFThresholdPositive, sampler_TexFaceSDFThresholdPositive, sdfUV).r;
    } 
    else 
    {
        // 光在左脸，用 Negative，并且要把 UV 水平翻转（因为负向图通常是镜像的）
        sdfUV.x = 1.0 - sdfUV.x;
        sdf = SampleFaceSDFAtlas(TEXTURE2D_ARGS(_TexFaceSDFNegative, sampler_TexFaceSDFNegative), sdfUV, frameF);
        th = SAMPLE_TEXTURE2D(_TexFaceSDFThresholdNegative, sampler_TexFaceSDFThresholdNegative, sdfUV).r;
    }

    // SDF 贴图中：白(1)为亮部，黑(0)为阴影边界
    // 结合阈值图 th，计算平滑过渡
    // 如果 sdf 大于 th，说明是亮部 (1.0)；如果小于 th，说明是暗部 (0.0)
    half rawMask = smoothstep(th - softness, th + softness, sdf);

    // rawMask 目前是 1 代表亮，0 代表暗。转换为阴影遮罩（1为暗，0为亮）
    return 1.0 - rawMask;
}
#endif // _FACESDF_ON
#endif // LIT_FACE_SDF_INCLUDED
