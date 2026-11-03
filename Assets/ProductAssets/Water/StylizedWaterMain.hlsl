#ifndef STYLIZED_WATER_MAIN_INCLUDED
#define STYLIZED_WATER_MAIN_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

CBUFFER_START(UnityPerMaterial)
    float4 _NormalMap_ST;
    float _NormalScale;
    float _WaveSpeed;
    
    half4 _ShallowColor;
    half4 _DeepColor;
    float _DepthMaxDistance;
    
    half4 _SpecColor;
    half _SpecSmoothness;
    half _SpecThreshold;

    half4 _FoamColor;
    half _FoamDistance;
    half _FoamThreshold;
    half _FoamSpeed;

    float _RefractionIntensity;
CBUFFER_END

TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);

struct Attributes
{
    float4 positionOS : POSITION;
    float2 uv : TEXCOORD0;
    float3 normalOS : NORMAL;
    float4 tangentOS : TANGENT;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 positionWS : TEXCOORD1;
    float3 normalWS : TEXCOORD2;
    float4 tangentWS : TEXCOORD3;
    float4 screenPos : TEXCOORD4;
    float fogFactor : TEXCOORD5;
};

Varyings vert (Attributes IN)
{
    Varyings OUT;
    VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
    VertexNormalInputs normInputs = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);

    OUT.positionCS = posInputs.positionCS;
    OUT.uv = IN.uv;
    OUT.positionWS = posInputs.positionWS;
    OUT.normalWS = normInputs.normalWS;
    OUT.tangentWS = float4(normInputs.tangentWS, IN.tangentOS.w * GetOddNegativeScale());
    OUT.screenPos = ComputeScreenPos(posInputs.positionCS);
    OUT.fogFactor = ComputeFogFactor(posInputs.positionCS.z);
    return OUT;
}

float hash(float2 p) { return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123); }
float noise(float2 p) {
    float2 i = floor(p); float2 f = frac(p);
    float a = hash(i); float b = hash(i + float2(1.0, 0.0));
    float c = hash(i + float2(0.0, 1.0)); float d = hash(i + float2(1.0, 1.0));
    float2 u = f * f * (3.0 - 2.0 * f);
    return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}

half4 frag (Varyings IN) : SV_Target
{
    float3 viewDirWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);
    float2 uvScreen = IN.screenPos.xy / IN.screenPos.w;

    // 双层滚动法线
    float2 uv1 = IN.uv * _NormalMap_ST.xy + _NormalMap_ST.zw;
    float2 uv2 = IN.uv * _NormalMap_ST.xy * 0.8 + _NormalMap_ST.zw;
    
    float2 moveDir1 = float2(0.5, 0.5) * _Time.y * _WaveSpeed;
    float2 moveDir2 = float2(-0.6, 0.3) * _Time.y * _WaveSpeed;

    half3 n1 = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv1 + moveDir1), _NormalScale);
    half3 n2 = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv2 - moveDir2), _NormalScale);
    half3 normalTS = normalize(n1 + n2);

    half3 binormalWS = cross(IN.normalWS, IN.tangentWS.xyz) * IN.tangentWS.w;
    half3 normalWS = normalize(mul(normalTS, half3x3(IN.tangentWS.xyz, binormalWS, IN.normalWS)));

    // 深度与折射
    float sceneDepth = LinearEyeDepth(SampleSceneDepth(uvScreen), _ZBufferParams);
    float surfaceDepth = IN.screenPos.w;
    float depthDifference = max(sceneDepth - surfaceDepth, 0);
    float depth01 = saturate(depthDifference / _DepthMaxDistance);

    float2 distortion = normalTS.xy * _RefractionIntensity * depth01;
    half3 colorRefract = SampleSceneColor(uvScreen + distortion).rgb;

    // 3. 颜色混合（拒绝蓝漆，清澈见底）
    half3 waterColor = lerp(colorRefract * _ShallowColor.rgb, _DeepColor.rgb, depth01);
    
    // 边缘菲涅尔反射（稍微加点白边）
    float fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), 3.0);
    waterColor = lerp(waterColor, half3(1,1,1), fresnel * 0.3);

    // 4. 卡通碎光高光（限制高光强度，防爆白）
    Light mainLight = GetMainLight();
    float3 halfDir = normalize(viewDirWS + mainLight.direction);
    float ndoth = saturate(dot(normalWS, halfDir));
    float specMask = smoothstep(0.8, 0.9, ndoth) * _SpecThreshold;
    half3 spec = _SpecColor.rgb * specMask * _SpecSmoothness;

    // 5. 岸边泡沫（窄边）
    float foamDepth = saturate(1.0 - (depthDifference / _FoamDistance));
    float foamMask = smoothstep(_FoamThreshold - 0.05, _FoamThreshold + 0.05, foamDepth);
    foamMask = saturate(foamMask);

    // 6. 最终输出
    half3 finalColor = waterColor + spec;
    finalColor = lerp(finalColor, _FoamColor.rgb, foamMask);
    finalColor = MixFog(finalColor, IN.fogFactor);

    // 关键透明度：浅水极度透明，深水逐渐不透明
    half alpha = smoothstep(0.01, 1.0, depth01);
    alpha = max(alpha, foamMask); 
    alpha = max(alpha, 0.15);

    return half4(finalColor, alpha);
}
#endif
