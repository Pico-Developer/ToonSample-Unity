
#ifndef BEZIER_CURVE_HLSL_INCLUDE
#define BEZIER_CURVE_HLSL_INCLUDE

/// https://en.wikipedia.org/wiki/B%C3%A9zier_curve#Linear_B%C3%A9zier_curves
/// https://zh.wikipedia.org/zh-sg/%E8%B2%9D%E8%8C%B2%E6%9B%B2%E7%B7%9A#%E4%BA%8C%E6%AC%A1%E6%96%B9%E8%B2%9D%E8%8C%B2%E6%9B%B2%E7%B7%9A
/// 贝塞尔曲线库

inline float pow2(float t)
{
    return t * t;
}

inline float pow3(float t)
{
    return t * t * t;
}

inline float r(float t)
{
    return 1.0 - t;
}

// Linear Bézier curves 忽略

/// Quadratic Bézier curves
/// p₀: 起点, p₁: 中心偏移点, p2: 终点
/// B(t) = (1-t)²P₀ + 2t(1-t)P₁ + t²P₂，t∈[0,1]
inline float3 QuadraticBezierCurves(float3 p0, float3 p1, float3 p2, float t)
{
    t = saturate(t);
    float tt = r(t);
    return pow2(tt) * p0 + 2 * t * tt * p1 + t * t * p2;
}

/// Quadratic Bézier curves 导函数
/// B'(t) = 2(1-t) (P₁-P₀) + 2t(P₂-P₁)
/// 可用于求切线
inline float3 QuadraticBezierCurvesDerivative(float3 p0, float3 p1, float3 p2, float t)
{
    t = saturate(t);
    return 2.0 * (1.0 - t) * (p1 - p0) + 2.0 * t * (p2 - p1);
}

/// Quadratic Bézier curves 二阶导函数
/// B"(t) = 2(P₂ - 2P₁ + P₀)
/// 暂不确定意图
inline float3 QuadraticBezierCurvesDerivative2(float3 p0, float3 p1, float3 p2)
{
    return 2.0 * (p2 - 2 * p1 + p0);
}

/// Cubic Bézier curves
/// B(t) = (1-t)³P₀ + 3(1-t)²tP₁ + 3(1-t)t²P₂ + t³P₃
inline float3 CubicBezierCurves(float3 p0, float3 p1, float3 p2, float3 p3, float t)
{
    t = saturate(t);
    float tt = r(t);

    return pow3(tt) * p0 + 3 * pow2(tt) * t * p1 + 3 * tt * t * t * p2 + pow3(t) * p3;
}

/// Cubic Bézier curves 导函数
/// B'(t) = 3 (1-t)²(P₁ - P₀) + 6(1-t)t(P₂-P₁) + 3t²(P₃-P₂)
inline float3 CubicBezierCurvesDerivative(float3 p0, float3 p1, float3 p2, float3 p3, float t)
{
    t = saturate(t);
    return 3 * pow2(r(t)) * (p1 - p0) + 6 * r(t) * t * (p2 - p1) + 3 * t * t * (p3 - p2);
}

#endif
