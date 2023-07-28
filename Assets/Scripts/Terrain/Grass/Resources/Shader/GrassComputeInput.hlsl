#ifndef GRASS_COMPUTE_INPUT
#define GRASS_COMPUTE_INPUT

#define REAL_IS_HALF 1

#if REAL_IS_HALF

#define real half
#define real2 half2
#define real3 half3
#define real4 half4
#define real2x2 half2x2
#define real3x3 half3x3
#define real4x4 half4x4

#else

#define real float
#define real2 float2
#define real3 float3
#define real4 float4
#define real2x2 float2x2
#define real3x3 float3x3
#define real4x4 float4x4

#endif

// PI 相关
#define PI              3.14159265358979323846
#define TWO_PI          6.28318530717958647693
#define FOUR_PI         12.5663706143591729538
#define INV_PI          0.31830988618379067154
#define INV_TWO_PI      0.15915494309189533577
#define INV_FOUR_PI     0.07957747154594766788
#define HALF_PI         1.57079632679489661923
#define INV_HALF_PI     0.63661977236758134308
#define HALF_HALF_PI    0.78539816339744830962

// xyz: chunkSize position, w: chunkSize
real4 grassParam0;
// x: MaxGrassSize, y: TerrainHeightMax, z: MaskClip, w: SlopeClip
real4 grassParam1;
// xy: TerrainHeightMap Size, zw: unused
real4 grassParam2;
// x: MaxDisplayRange, y: GrassHeight, z: GrassWidth, w: unused
real4 grassParam3;
// x: FakeCloudPower, y: GrassWavePower, z: LambertStrength, w: shadowmask strength
real4 grassParam4;

// 假云影
// x: scaleX, y: scaleY, z: speed, w: strength
uniform real4 fakeCloudParameter;

// 草浪
// x: scaleX, y: scaleY, z: speed, w: strength
uniform real4 grassWaveParameter;

// 风
// x: wind rotation, y: wind split, z: wind strength, w: unused
uniform real4 windParameter;

// 转世界坐标矩阵
real4x4 localToWorldMatrix;
real4x4 worldToLocalMatrix;

// vp矩阵
real4x4 vpMatrix;
real3 cameraPositionWS;

// Engine Param
real4 _Time;
real4 unity_WorldTransformParams; // w is usually 1.0, or -1.0 for odd-negative scale transforms
real4 _MainLightPosition;



///////////////////////////////////////////////////////////////////////////////////


// 云影
#define _FakeCloudPower grassParam4.x

#define _FakeCloudScaleX fakeCloudParameter.x
#define _FakeCloudScaleY fakeCloudParameter.y
#define _FakeCloudSpeed fakeCloudParameter.z
#define _FakeCloudStrength fakeCloudParameter.w

// 草浪
#define _GrassWavePower grassParam4.y

#define _GrassWaveScaleX grassWaveParameter.x
#define _GrassWaveScaleY grassWaveParameter.y
#define _GrassWaveSpeed grassWaveParameter.z
#define _GrassWaveStrength grassWaveParameter.w

// 风
#define _WindRotation windParameter.x
#define _WindSplit windParameter.y
#define _WindStrength windParameter.z
#define _WindOfTilt windParameter.w

// lambert 强度
#define _GrassLambertStrength grassParam4.z


// 草的高度图
Texture2D TerrainHeightNormalMap;
SamplerState samplerTerrainHeightNormalMap;//sampler_GrassHeightMap;
#define GrassHeightMapTexelSize grassParam2.xy

Texture2D GrassDataMap;
SamplerState samplerGrassDataMap;
real grassNullParam;

// 单个Chunk信息
#define _ChunkPosition grassParam0.xyz
#define _ChunkSize grassParam0.w

// 单个Manage的尺寸大小
#define _GrassManageSize grassParam1.x
// #define _GrassManageHalfSize grassParam1.y
// 高度图的最大高度是多少
#define _MaxHeightMap grassParam1.y

// Mask贴图的clip值
#define _MaskClip grassParam1.z
#define _SlopeClip grassParam1.w

// 草的最大显示范围, 宽高信息
#define _MaxDisplayRange grassParam3.x
#define _GrassHeight grassParam3.y
#define _GrassWidth grassParam3.z

// shadow mask 强度
#define _ShadowMaskStrength grassParam4.w


// 假影子的纹理图
Texture2D FakeCloudShadowMap;
SamplerState samplerFakeCloudShadowMap;

// 草浪
Texture2D GrassWaveMap;
SamplerState samplerGrassWaveMap;




#endif
