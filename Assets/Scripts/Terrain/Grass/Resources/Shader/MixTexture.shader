Shader "[Hidden]Editor/MixTexture"
{
	Properties
	{
		_R ("R", 2D) = "black" {}
		_G ("G", 2D) = "black" {}
		_B ("B", 2D) = "black" {}
		_A ("A", 2D) = "black" {}
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
			struct appdata
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			// sampler2D _MainTex;
			TEXTURE2D(_R);	SAMPLER(sampler_R);
			TEXTURE2D(_G);	SAMPLER(sampler_G);
			TEXTURE2D(_B);	SAMPLER(sampler_B);
			TEXTURE2D(_A);	SAMPLER(sampler_A);

			uniform float4 default_value;
			uniform float4 value_switch;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.positionOS);
				o.uv = v.uv;
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				const half2 uv = i.uv;
				half r = SAMPLE_TEXTURE2D(_R, sampler_R, uv).x;
				half g = SAMPLE_TEXTURE2D(_G, sampler_G, uv).x;
				half b = SAMPLE_TEXTURE2D(_B, sampler_B, uv).x;
				half a = SAMPLE_TEXTURE2D(_A, sampler_A, uv).x;

				half4 o = half4(r, g, b, a);
				o = lerp(default_value, o, value_switch);
				return o;
			}
			ENDHLSL
		}
	}
}