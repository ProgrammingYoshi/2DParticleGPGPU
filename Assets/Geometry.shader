// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Geometry"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" /*"RenderType" = "Opaque"*/ }
		LOD 100

		Pass
	{
		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Mode Off }
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma geometry geom

#include "UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
	};

	struct v2f
	{
		float4 color : COLOR;
		float4 vertex : SV_POSITION;
	};

	struct Particle
	{
		float2 position;
		float2 speed;
		float2 acceleration;
		float4 color;
	};

	//RWStructuredBuffer<Particle> Data;
	StructuredBuffer<Particle> dataBuffer;
	sampler2D _MainTex;
	float4 _MainTex_ST;

	v2f vert(appdata v)
	{
		v2f o;
		o.color = float4(0, 0, 0, 0);
		o.vertex = v.vertex;// UnityObjectToClipPos(v.vertex);
		return o;
	}

	[maxvertexcount(64)]
	void geom(point v2f input[1], inout PointStream<v2f> OutputStream)
	{
		v2f test = (v2f)0;

		/*test.vertex = input[0].vertex;//UnityObjectToClipPos(dataBuffer[i + pos].position);
		OutputStream.Append(test);*/

		for (int i = 0; i < 64; i++)
		{
			/*test.vertex.xyz = input[0].vertex.xyz;//UnityObjectToClipPos(dataBuffer[i + pos].position);
			OutputStream.Append(test);
			test.vertex = UnityObjectToClipPos(dataBuffer[i + pos].position + float3(1, 0, 0));
			OutputStream.Append(test);*/
			test.vertex = UnityObjectToClipPos(float4(dataBuffer[i + input[0].vertex.x].position, 0, 0));// +float3(0, 1, 0));
			test.color = dataBuffer[i + input[0].vertex.x].color;
			OutputStream.Append(test);
		}
	}

	fixed4 frag(v2f i) : SV_Target
	{
	return i.color;
	}
		ENDCG
	}
	}
}

