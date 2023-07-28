Shader "Sample/Sea_V1"
{
    Properties
    {
        [MainColor]_BaseColor("Base Color", Color)              = (1, 1, 1, 1)
        _ShoalColor("ShoalColor", Color)                        = (1, 1, 1, 1)
        _ShoreColor("ShoreColor", Color)                        = (1, 1, 1, 1)
        _ColorLerp("ColorLerp", Range(-1, 1))                   = 0.5
        _WaveSpeed ("WaveSpeed", Range(0, 1))                   = 0
        _BumpTex ("NormalMap", 2D)                              = "bump" {}
        _BumpScale ("NormalScale", Range(-1, 1))                = 1
        _WaveDir ("WaveDir", Range(0, 1))                       = 0
        _WaveNoiseSpeed ("WaveNoiseSpeed", Range(0, 2))         = 0.1
        _WaveAttenuationCoefficient ("WaveAttenuationCoefficient", Range(1, 400)) = 10
        [NoScaleOffset] _EnvCubeTex ("EnvCubeTex", Cube)        = "white" {}
        _Rotation ("Rotation", Range(0, 360)) = 0
        
        _TestParam ("TestParam", Range(0, 360)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

        	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float3 normalOS      : NORMAL;
                float4 tangentOS     : TANGENT;
                float3 color : COLOR;
                float2 uv : TEXCOORD0;
                
            };

            struct v2f
            {
                float2 uv0 : TEXCOORD0;
                float3 color : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float3 tangentWS : TEXCOORD3;
                float3 bitangentWS : TEXCOORD4;
                float3 positionWS : TEXCOORD5;
                float4 positionHS : SV_POSITION;
            };

            TEXTURECUBE(_EnvCubeTex);
            SAMPLER(sampler_EnvCubeTex);
            TEXTURE2D(_BumpTex);
            SAMPLER(sampler_BumpTex);

CBUFFER_START(UnityPerMaterial)
    
            float4 _BumpTex_ST;
            float _WaveSpeed;
            float _BumpScale;
            float _WaveScale;
            float _WaveNoiseSpeed;
            float _WaveDir;
            float _ColorLerp;
            float _WaveAttenuationCoefficient;
            float3 _BaseColor;
            float4 _ShoalColor;
            float4 _ShoreColor;
            float _Rotation;
            float _TestParam;
            
CBUFFER_END
            
            

            v2f vert (appdata i)
            {
                v2f o = (v2f)0;
                o.color = i.color;
                float3 positionOS = i.positionOS.xyz;

                //positionOS.y += i.color.x * 0.0005 * _WaveSpeed * (sin(_Time.y) * 0.5 + 0.5);
                float3 positionWS = TransformObjectToWorld(positionOS);
                positionWS.y -= _WaveSpeed * (sin(_Time.y) * 0.5 + 0.5);
                // o.positionWS = TransformObjectToWorld(positionOS);
                o.positionWS = positionWS;
                
                o.positionHS = TransformWorldToHClip(positionWS);//TransformObjectToHClip(positionOS);
                o.uv0 = i.uv;

                float3 sign = float(i.tangentOS.w) * GetOddNegativeScale();
                // float3 normalWS = TransformObjectToWorldNormal(i.normalOS);
                // float3 tangentWS = float3(TransformObjectToWorldDir(i.tangentOS.xyz));
                // float3 bitangentWS = float3(cross(normalWS, float3(tangentWS))) * sign;
                float3 normalWS = float3(0, 1, 0);
                float3 tangentWS = float3(1, 0, 0);
                float3 bitangentWS = float3(cross(normalWS, float3(tangentWS))) * sign;

                o.normalWS    = normalWS;
                o.tangentWS   = tangentWS;
                o.bitangentWS = bitangentWS;
    
                return o;
            }

            float3 InterpolatedColor(float3 color1, float3 color2, float3 color3, float t)
            {
                return t * color1 + (1 - t) * (t * color2 + (1 - t) * color3);
            }

            float3 RotateAroundYInDegrees (float3 vertex, float degrees)
            {
                float alpha = degrees * 3.1415926 / 180.0;
                float sina, cosa;
                sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, -sina, sina, cosa);
                return float3(mul(m, vertex.xz), vertex.y).xzy;
            }

            float4 frag (v2f i) : SV_Target
            {

                float2 uv = i.uv0;

                float2 waveDir = float2(0, 1);
                float c = cos(_WaveDir * 2 * 3.1415926);
                float s = sin(_WaveDir * 2 * 3.1415926);
                float2x2 waveMatrix = float2x2(c, -s, s, c);
                waveDir = mul(waveMatrix, waveDir);

                float2 uv2 = (uv * _BumpTex_ST.xy + _BumpTex_ST.zw) + _Time.x * _WaveNoiseSpeed * waveDir;
                float4 n = SAMPLE_TEXTURE2D(_BumpTex, sampler_BumpTex, uv2);
                float3x3 tbn = float3x3((i.tangentWS.xyz), (i.bitangentWS.xyz), (i.normalWS.xyz));
                tbn = transpose(tbn);
                float3 normalTS = UnpackNormalScale(n, _BumpScale * saturate(1 - 1. * (i.positionHS.w/_WaveAttenuationCoefficient - 0.4 )));
                float3 normalWS = normalize(mul(tbn, normalTS));


                float3 viewDir = normalize(GetWorldSpaceViewDir(i.positionWS));

                Light mainLight = GetMainLight();
                float3 lightDir = mainLight.direction;
                float3 halfDir = SafeNormalize(viewDir + lightDir);

                float VdotN = saturate(dot(viewDir, normalWS));
                float LdotN = saturate(dot(lightDir, normalWS));

                float lambert = saturate(LdotN);

                
                float3 refl = reflect(-viewDir, normalWS);
                refl = RotateAroundYInDegrees(refl, 360 - _Rotation);
                // float3 env = DecodeHDREnvironment(SAMPLE_TEXTURECUBE_BIAS(_EnvCubeTex, sampler_EnvCubeTex, refl, _TestParam), unity_SpecCube0_HDR);
                // float3 env = DecodeHDREnvironment(SAMPLE_TEXTURECUBE_BIAS(_EnvCubeTex, sampler_EnvCubeTex, refl, 0), unity_SpecCube0_HDR);
                float4 env = SAMPLE_TEXTURECUBE(_EnvCubeTex, sampler_EnvCubeTex, refl);
                env.xyz = DecodeHDREnvironment(env, unity_SpecCube0_HDR);

                float4 col = float4(1, 1, 1, 1);
                // col.xyz = lerp(env.xyz, _BaseColor, pow(VdotN, _TestParam));
                col.xyz = lerp(env.xyz, _BaseColor, VdotN * VdotN * VdotN * VdotN);
                col.xyz = lerp(col.xyz, _ShoalColor.xyz, i.color.x);

                col.xyz = InterpolatedColor(_ShoreColor.xyz, _ShoalColor.xyz, col.xyz, pow(saturate(i.color.x), pow(10, -_ColorLerp)));

                // col *= lambert;

                // col.xyz += pow(HdotN, _TestParam) * 0.5;
                return col;
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "MotionVectors"
            Tags{ "LightMode" = "MotionVectors"}

            ZWrite On

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/OculusMotionVectorCore.hlsl"

            #pragma vertex vert
            #pragma fragment frag

            ENDHLSL
        }
	
    }
}
