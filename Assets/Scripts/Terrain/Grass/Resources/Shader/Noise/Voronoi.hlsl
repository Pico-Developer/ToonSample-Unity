#ifndef INCLUDE_NOISE_VORONOI
#define INCLUDE_NOISE_VORONOI


inline float2 VoronoiRandomVector (float2 UV, float offset)
{
    float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
    UV = frac(sin(mul(UV, m)));
    return float2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
}


void NoiseVoronoi(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
{
    float2 g = floor(UV * CellDensity);
    float2 f = frac(UV * CellDensity);
    float t = 8.0;
    float3 res = float3(8.0, 0.0, 0.0);

    for(int y=-1; y<=1; y++)
    {
        for(int x=-1; x<=1; x++)
        {
            float2 lattice = float2(x,y);
            float2 offset = VoronoiRandomVector(lattice + g, AngleOffset);
            float d = distance(lattice + offset, f);

            if(d < res.x)
            {
                res = float3(d, offset.x, offset.y);
                Out = res.x;
                Cells = res.y;
            }
        }
    }
}

float2 NoiseVoronoi(float2 UV, float AngleOffset, float CellDensity)
{
    float2 o = (float2)0;
    NoiseVoronoi(UV, AngleOffset, CellDensity, o.x, o.y);
    return o;
}

#endif
