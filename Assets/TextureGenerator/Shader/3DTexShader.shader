Shader "Debug/3DTexShader"
{
	Properties
	{
		_X ("X", Range(0, 1)) = 0
		_Y ("Y", Range(0, 1)) = 0
		_Z ("Z", Range(0, 1)) = 0
		_3DNoise ("3D Noise", 3D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float3 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
			
			float _X;
			float _Y;
			float _Z;
			sampler3D _3DNoise;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.vertex.xyz;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col;
				i.uv.x += _X;
				i.uv.y += _Y;
				i.uv.z += _Z;
				col = tex3D(_3DNoise, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
