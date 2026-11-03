Shader "Custom/StylizedWater"
{
    Properties
    {
        [Header(Textures)]
        _NormalMap ("Water Normal Map", 2D) = "bump" {}
        _NormalScale ("Normal Scale", Range(0, 2)) = 0.5
        _WaveSpeed ("Wave Speed", Range(0, 2)) = 0.5

        [Header(Colors)]
        [HDR] _ShallowColor ("Shallow Color", Color) = (0.1, 0.9, 0.8, 1)
        [HDR] _DeepColor ("Deep Color", Color) = (0.05, 0.2, 0.5, 1)
        
        [Header(Depth)]
        _DepthMaxDistance ("Depth Max Distance", Float) = 2.0

        [Header(Specular)]
        [HDR] _SpecColor ("Specular Color", Color) = (1, 1, 1, 1)
        _SpecSmoothness ("Specular Smoothness", Range(0, 1)) = 0.9
        _SpecThreshold ("Specular Threshold", Range(0, 1)) = 0.6

        [Header(Foam)]
        [HDR] _FoamColor ("Foam Color", Color) = (1, 1, 1, 1)
        _FoamDistance ("Foam Distance", Float) = 0.3
        _FoamThreshold ("Foam Threshold", Range(0, 1)) = 0.8
        _FoamSpeed ("Foam Speed", Float) = 1.0

        [Header(Refraction)]
        _RefractionIntensity ("Refraction Distortion", Range(0, 0.1)) = 0.02
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            
            ZWrite Off
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma target 3.0
            #pragma multi_compile_fog
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "StylizedWaterMain.hlsl"
            ENDHLSL
        }
    }
}
