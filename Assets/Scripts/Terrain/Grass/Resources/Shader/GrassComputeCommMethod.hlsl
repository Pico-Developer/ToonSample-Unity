#ifndef GRASS_COMM_METHOD
#define GRASS_COMM_METHOD


#include "GrassComputeInput.hlsl"


// 视锥裁剪相关
// 透视除法之后, 坐标转为ndc空间, 范围在[-1,1]之间
// 这里直接简化了除法的计算, 用 abs(xyz) > w 来判断
// 在DX下, NDC的z方向范围是[0,1], 可以使用宏 UNITY_NEAR_CLIP_VALUE 来判断z的近平面的值, OpenGL环境是-1, DX下是0
inline bool FrustumTestHSSupportGPU_VPMatrix(real4 positionHS)
{
    real ndcZ = positionHS.z * rcp(positionHS.w);
    positionHS = abs(positionHS);
    return ndcZ < 0 || ndcZ > 1
    || positionHS.x > positionHS.w
    || positionHS.y > positionHS.w;
}

inline bool FrustumTestHS(real4 positionHS)
{
    positionHS = abs(positionHS);
    return positionHS.z > positionHS.w
    || positionHS.x > positionHS.w
    || positionHS.y > positionHS.w;
}

bool FrustumTestOS(real3 positionOS)
{
    real4 positionWS = mul(localToWorldMatrix, real4(positionOS, 1));
    real4 positionHS = mul(vpMatrix, positionWS);
    return FrustumTestHSSupportGPU_VPMatrix(positionHS);
}


inline real CalculateGridSize(int width)
{
    return _ChunkSize * rcp(width);
}

// 相对位置[grass manage]转uv
inline real2 Relative2UV(real2 position)
{
    return (position) / _GrassManageSize + 0.5;
}

// 点采样
real4 SampleMapWithPoint(Texture2D map, real2 uv, real2 sizeOfTex)
{
    return map.Load(uint3(uint2(uv * sizeOfTex), 0));
}

// 线性采样
real4 SampleMapWithLinear(Texture2D map, SamplerState samplerState, real2 uv)
{
    return map.SampleLevel(samplerState, uv, 0);
}

/**
 * Gather方式的线性插值方案, 顺序方案来自大表哥, 已通过验证
 * https://zhuanlan.zhihu.com/p/127435500
 * Gather方法在Unity中的取样存储的顺序应该是下面这样的顺序：
 * UV对应w像素的覆盖范围或者四个像素交叉位置
 *        ┌—-—┰—-—┐    ┌—-—┰—-—┐
 *        | w | z |    | 3 | 2 |
 *        ├—-—┼—-—┤    ├—-—┼—-—┤
 *        | x | y |    | 0 | 1 |
 *        └—-—┴—-—┘    └—-—┴—-—┘
 */
real SampleLinearWithGatherRed(Texture2D map, SamplerState samplerState, real2 uv, real2 sizeOfTex)
{
    real4 c = map.GatherRed(samplerState, uv);
    real2 f = frac(uv * sizeOfTex + 0.5);
    return lerp(lerp(c.w, c.z, f.x), lerp(c.x, c.y, f.x), f.y);
}

real SampleLinearWithGatherGreen(Texture2D map, SamplerState samplerState, real2 uv, real2 sizeOfTex)
{
    real4 c = map.GatherGreen(samplerState, uv);
    real2 f = frac(uv * sizeOfTex + 0.5);
    return lerp(lerp(c.w, c.z, f.x), lerp(c.x, c.y, f.x), f.y);
}

real SampleLinearWithGatherBlue(Texture2D map, SamplerState samplerState, real2 uv, real2 sizeOfTex)
{
    real4 c = map.GatherBlue(samplerState, uv);
    real2 f = frac(uv * sizeOfTex + 0.5);
    return lerp(lerp(c.w, c.z, f.x), lerp(c.x, c.y, f.x), f.y);
}

real SampleLinearWithGatherAlpha(Texture2D map, SamplerState samplerState, real2 uv, real2 sizeOfTex)
{
    real4 c = map.GatherAlpha(samplerState, uv);
    real2 f = frac(uv * sizeOfTex + 0.5);
    return lerp(lerp(c.w, c.z, f.x), lerp(c.x, c.y, f.x), f.y);
}


// 将real转为int
uint4 UnpackToInt(real4 f, uint bits)
{
    uint maxInt = (1u << bits) - 1u;
    return (uint4)(f * maxInt + 0.5);
}

// 将int转为real
real4 PackToFloat(uint4 i, uint numBits)
{
    uint maxInt = (1u << numBits) - 1u;
    return saturate(i * rcp(maxInt));
}

// 对一张贴图的rg通道进行采样, 并且混合成16bit进行返回
real SampleLinearWithRG(Texture2D map, SamplerState samplerState, real2 uv, real2 sizeOfTex)
{
    real4 r = map.GatherRed(samplerState, uv);
    real4 g = map.GatherGreen(samplerState, uv);

    uint maxInt8Bit = (1u << 8) - 1u;
    uint4 lo = (uint4)(g * maxInt8Bit + 0.5);
    uint4 hi = (uint4)(r * maxInt8Bit + 0.5);
    uint4 cb = (hi << 8) + lo;
    uint maxInt16Bit = (1u << 16) - 1u;
    real4 c = saturate(cb * rcp(maxInt16Bit));
    real2 f = frac(uv * sizeOfTex + 0.5);
    return  lerp(lerp(c.w, c.z, f.x), lerp(c.x, c.y, f.x), f.y);
}

void Rotation(inout float3 dir, float angle)
{
    float c = cos(angle);
    float s = sin(angle);
    const float3x3 mat = float3x3(c, 0, -s, 0, 1, 0, s, 0, c);
    dir = mul(mat, dir);
}
#endif
