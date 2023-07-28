Shader "Sample/GrassShader"
{
	Properties
	{
//		_ShadowMaskTex ("ShadowMaskTex", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
        {
        	Cull OFF
        	NAME "Grass"
        	
            HLSLPROGRAM
            
            #pragma vertex vert
			#pragma fragment frag
			#pragma target 4.5
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			// #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fog

            
            #pragma multi_compile_instancing
            // #pragma instancing_options procedural:GrassInstancingSetup
            
        	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "GrassData.cs.hlsl"
			#include "BezierCurves.hlsl"

            struct a2v
			{
			    uint vid : VERTEXID_SEMANTIC;
			    UNITY_VERTEX_INPUT_INSTANCE_ID
			};
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
            	float4 param0: TEXCOORD0;// x: height, y: wind strength, z: grassType, w: diffuseLumaFix
            	float4 param1: TEXCOORD1;
            	float3 normal : TEXCOORD2;
            	float4 positionWS : TEXCOORD3;
            	float2 uv0 : TEXCOORD4;
            	// nointerpolation float grassType : TEXCOORD4;
            	// float fakeShadow : TEXCOORD4;
            	// float3 normalTerrain : TEXCOORD4;
            	float4 debug :TEXCOORD5;
            	
			    UNITY_VERTEX_INPUT_INSTANCE_ID
			    UNITY_VERTEX_OUTPUT_STEREO
            };

			// M 矩阵
			uniform float4x4 localToWorldMatrix;
			uniform float4x4 worldToLocalMatrix;

            // 风的方向
            // uniform float2 windDir;

            uniform float4 grassColorTop;
            uniform float4 grassColorBottom;
            uniform float4 shadowColorTop;
            uniform float4 shadowColorBottom;
            uniform float4 waveColorTop;
            uniform float4 waveColorBottom;

            uniform float4 _Position[15];

            uniform float4 _ShaderParam0;
            #define _ColorBias _ShaderParam0.x
            #define _LightIntensity _ShaderParam0.y
            #define _GrassTime _ShaderParam0.z

            StructuredBuffer<GrassData> instanceData;

            v2f vert (a2v i, uint iid : INSTANCEID_SEMANTIC) {
                v2f o = (v2f)0;

       			UNITY_SETUP_INSTANCE_ID(i);
			    UNITY_TRANSFER_INSTANCE_ID(i, o);
			    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            	
                GrassData vert = instanceData[iid];

            	// random 0~1
            	float hash0 = vert.PerBladeHash;
            	// random -1~1
            	float hash1 = hash0 * 2.0 - 1.0;
            	

                float3 defaultFacing = float3(0, 0, 1);
                float3 facing = float3(vert.Facing.x, 0, vert.Facing.y);
            	

                float3 positionBlade = _Position[i.vid].xyz;
            	float t = saturate(positionBlade.y);
            	positionBlade.xz *= vert.Width;
            	positionBlade *= vert.Height;

            	// 草叶旋转
            	defaultFacing = normalize(mul(defaultFacing, (float3x3)worldToLocalMatrix));
             	float c = dot(facing, defaultFacing);
                float s = cross(facing, defaultFacing).y;
             	float3x3 matrixRotation = float3x3(c, 0, -s, 0, 1, 0, s, 0, c);
				positionBlade = mul(matrixRotation, positionBlade);

            	// 朝着风向倾斜
            	positionBlade.xz += normalize(vert.Wind) * vert.Tilt * t * vert.Height;

            	// 弯曲
            	float3 root = float3(0, 0, 0);
                float3 up = float3(0, 1, 0) * vert.Height;

            	// wind
            	float3 finalWindEnd = up + vert.Height * float3(vert.Wind.x, 0, vert.Wind.y) * (sin(_GrassTime + hash1 * 10 * 3.1415) * 0.5 + 0.5);
            	finalWindEnd = normalize(finalWindEnd) * length(up);
            	
            	float3 bezier = QuadraticBezierCurves(root, up, finalWindEnd, t);
            	positionBlade.xz += bezier.xz;

            	// 草的朝向法线
            	float3 bladeNormal = float3(0, 0, 1);
            	bladeNormal = mul(matrixRotation, bladeNormal);
            	bladeNormal = normalize(mul(float4(bladeNormal, 0), worldToLocalMatrix)).xyz;
            	bladeNormal = normalize(bladeNormal + up);
            	float3 tangent = (QuadraticBezierCurvesDerivative(root, up, finalWindEnd, t));
            	float3 bitangent = (cross(bladeNormal, tangent));
            	bladeNormal = (cross(tangent, bitangent));
            	// bladeNormal = tangent;

            	/////////////////////////////////////////////////////////////////
            	// float3 positionOS = vert.Position + positionBlade;
            	float4 positionWS = float4(vert.Position + positionBlade, 1);
            	float3 positionVS = TransformWorldToView(positionWS.xyz);
            	float4 positionHS = TransformWViewToHClip(positionVS);

            	o.vertex = positionHS;
                o.normal = bladeNormal;
            	o.param0 = float4(
            		t,
            		vert.FakeShadow,
            		vert.Wave,
            		vert.Lambert
            	);
            	o.param1 = float4(vert.ShadowMask, hash0, 0, 0);
            	o.positionWS.xyz = positionWS.xyz;
            	o.positionWS.w = ComputeFogFactor(o.vertex.z);
            	// o.normalTerrain = vert.Normal;
            	// o.fakeShadow = vert.FakeShadow;
                return o;
            }

            // blinn-phone
			float3 specularBlinnPhone(float3 lightDir, float3 viewDir, float3 normal, float phonePow)
			{
			    return  pow(max(0,dot(normalize(viewDir + lightDir), normal)), phonePow);
			}

   //          void Unity_ColorspaceConversion_RGB_Linear_float(float3 In, out float3 Out)
			// {
			//     float3 linearRGBLo = In / 12.92;
			//     float3 linearRGBHi = pow(max(abs((In + 0.055) / 1.055), 1.192092896e-07), float3(2.4, 2.4, 2.4));
			//     Out = float3(In <= 0.04045) ? linearRGBLo : linearRGBHi;
			// }

            float4 frag (v2f i, FRONT_FACE_TYPE vFace : FRONT_FACE_SEMANTIC) : SV_Target
            {

    			UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
            	
	            float t = i.param0.x;
            	float grassFakeShadowStrength = i.param0.y;
            	float grassWaveStrength = i.param0.z;
            	float lambert = i.param0.w;

            	float shadowMaskStrength = i.param1.x;
            	float hash = i.param1.y;

                float4 roof = lerp(grassColorTop, waveColorTop, grassWaveStrength);
                float4 root = lerp(grassColorBottom, waveColorBottom, grassWaveStrength);

				roof *= lerp(shadowColorTop, shadowColorBottom, grassFakeShadowStrength);
				root *= lerp(shadowColorTop, shadowColorBottom, grassFakeShadowStrength);

            	float4 col = lerp(root, roof, saturate(t + _ColorBias));
            	col *= 1 - (hash * 0.2);
            	
            	Light light = GetMainLight();
            	col.xyz *= lerp(1, light.color, _LightIntensity);

            	float4 shadowCoords = TransformWorldToShadowCoord(i.positionWS.xyz);
            	float shadow = MainLightRealtimeShadow(shadowCoords);
            	col.xyz *= min(max(shadow, grassFakeShadowStrength), shadowMaskStrength);
            	col.xyz *= lambert;
            	
            	col.xyz = MixFog(col.xyz, i.positionWS.w);
            	
            	return col;
            }
            ENDHLSL
        }
	}
}