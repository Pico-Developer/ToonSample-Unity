Shader "Sample/PICO4"
{
    Properties
    {
        [NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}
        _DiffuseColor("DiffuseColor",Color) = (1,1,1,1)
        [HDR]_FresnelCol("FresnelColor",Color) =(1,1,1,1)
        _FresnelPower("FresnelPower",Range(0,99))=1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "IgnoreProjector" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100
        ZWrite On
        ZTest On
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 positionHS : SV_POSITION;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
CBUFFER_START(UnityPerMaterial)
            uniform float4 _FresnelCol;
            uniform float _FresnelPower;
            uniform float4 _DiffuseColor;
CBUFFER_END
            v2f vert(appdata v)
            {
                v2f o;
                o.positionHS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float3 vDir = normalize(_WorldSpaceCameraPos.xyz - i.positionWS);
                float fresnelRate = 1.0 - saturate(dot(i.normalWS, vDir));
                float3 fresnelCol = pow(fresnelRate, _FresnelPower) * _FresnelCol;

                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                col.rgb *= (1 - fresnelCol) * _DiffuseColor;
                col.rgb += fresnelCol;
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