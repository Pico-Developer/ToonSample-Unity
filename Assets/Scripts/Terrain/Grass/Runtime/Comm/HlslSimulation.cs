using System;
using UnityEngine;

namespace GamePlus.Urp.Terrain.Grass.Runtime.Comm
{
    public class HlslSimulation : MonoBehaviour
    {
        
        public Vector4 half4(float x, float y, float z, float w) => new Vector4(x, y, z, w);
        public Vector3 half3(float x, float y, float z) => new Vector3(x, y, z);
        public Vector2 half2(float x, float y) => new Vector2(x, y);
        
        
        public Vector4 half4(double x, double y, double z, double w) => new Vector4((float)x, (float)y, (float)z, (float)w);
        public Vector3 half3(double x, double y, double z) => new Vector3((float)x, (float)y, (float)z);
        public Vector2 half2(double x, double y) => new Vector2((float)x, (float)y);
        
        public Vector4 float4(float x, float y, float z, float w) => new Vector4(x, y, z, w);
        public Vector3 float3(float x, float y, float z) => new Vector3(x, y, z);
        public Vector2 float2(float x, float y) => new Vector2(x, y);
        
        public Vector4 float4(double x, double y, double z, double w) => new Vector4((float)x, (float)y, (float)z, (float)w);
        public Vector3 float3(double x, double y, double z) => new Vector3((float)x, (float)y, (float)z);
        public Vector2 float2(double x, double y) => new Vector2((float)x, (float)y);
        public Vector4 lerp(Vector4 a, Vector4 b, float t) => Vector4.Lerp(a, b, t);
        public Vector3 lerp(Vector3 a, Vector3 b, float t) => Vector3.Lerp(a, b, t);
        public Vector2 lerp(Vector2 a, Vector2 b, float t) => Vector2.Lerp(a, b, t);
        public float lerp(float a, float b, float t) => Mathf.Lerp(a, b, t);
        public float lerp(double a, double b, double t) => Mathf.Lerp((float)a, (float)b, (float)t);
        public Vector4 normalize(Vector4 a) => a.normalized;
        public Vector3 normalize(Vector3 a) => a.normalized;
        public Vector2 normalize(Vector2 a) => a.normalized;

        public Vector3 cross(Vector3 a, Vector3 b) => Vector3.Cross(a, b);

        public float dot(Vector2 a, Vector2 b) => Vector2.Dot(a, b);
        public float dot(Vector3 a, Vector3 b) => Vector3.Dot(a, b);
        public float dot(Vector4 a, Vector4 b) => Vector4.Dot(a, b);

        public Vector3 mul(Matrix3x3 m, Vector3 v) => m * v;

        public Matrix3x3 float3x3(
            float a, float b, float c,
            float d, float e, float f,
            float g, float h, float i) => new Matrix3x3(a, b, c, d, e, f, g, h, i);

        public float sin(float t) => (float)Math.Sin(t);
        public float sin(double t) => (float)Math.Sin(t);
        public float cos(float t) => (float)Math.Cos(t);
        public float cos(double t) => (float)Math.Cos(t);

        public float abs(double t) => (float)Math.Abs(t);

        public float clamp(float x, float min, float max) => Math.Clamp(x, min, max);
        public float clamp(double x, double min, double max) => (float)Math.Clamp(x, min, max);

        public float saturate(float x) => clamp(x, 0f, 1f);

        public Vector4 _Time => new Vector4(Time.time/ 20.0f, Time.time, Time.time * 2, Time.time * 3);
    }
}