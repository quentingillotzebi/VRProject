#include "Shared.cginc"
#include "UnityCG.cginc"

float2 _BufferSize;
float2 _BufferPixel;
float2 _SamplePixel;

struct appdata
{
	float4 vertex : POSITION;
	float2 uv     : TEXCOORD0;
};

struct v2f
{
	float4 vertex : SV_POSITION;
};

void vert(appdata v, out v2f o)
{
	float2 bufferCoord = (_BufferPixel + v.uv.xy) / _BufferSize;

	o.vertex = float4(bufferCoord * 2.0f - 1.0f, 0.5f, 1.0f);
#if UNITY_UV_STARTS_AT_TOP
	o.vertex.y = -o.vertex.y;
#endif
}