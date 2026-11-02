// -----通用工具函数-----

// 绕锚点旋转UV：uv坐标、radian弧度、pivot锚点
float2 RotateUV(float2 uv, float radian, float2 pivot){
    float rCos = cos(radian);
    float rSin = sin(radian);
    return (mul(uv - pivot, float2x2(rCos, -rSin, rSin, rCos)) + pivot);
}

// -----噪声函数-----
// 社区里"试出来好用"的魔法数
float2 hash22(float2 p){
    p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
    return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
}

// 噪声贴图函数 不连续
float RandomNoiseMap(float seed){
    return RandomNoiseMap(float2(seed, 1.0));
}

// 噪声贴图函数 连续
float perlin_noise(float2 p){
    float2 pi = floor(p);
    float2 pf = p - pi;
    float2 w = pf * pf * (3.0 - 2.0 * pf);
    float2 value = lerp
    (
        lerp
        (
            dot(hash22(pi + float2(0.0, 0.0)), pf - float2(0.0, 0.0)),
            dot(hash22(pi + float2(1.0, 0.0)), pf - float2(1.0, 0.0)),
            w.x
        ),
        lerp
        (
            dot(hash22(pi + float2(0.0, 1.0)), pf - float2(0.0, 1.0)),
            dot(hash22(pi + float2(1.0, 1.0)), pf - float2(1.0, 1.0)),
            w.x
        ),
        w.y
    );
    return value;
}