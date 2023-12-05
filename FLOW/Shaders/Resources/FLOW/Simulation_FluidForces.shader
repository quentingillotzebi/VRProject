Shader "Hidden/FLOW/Simulation_FluidForces"
{
	Properties
	{
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "Shared.cginc"
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv     : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv     : TEXCOORD0;
			};

			void vert(appdata v, out v2f o)
			{
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv     = v.uv;
			}

			float4 frag(v2f i) : SV_Target
			{
				float2 columnCoord = SnapCoord(i.uv, _FlowCountXZ);
				Column column      = GetColumn(columnCoord);
				Column columnL     = GetColumn(columnCoord - _FlowCoordU000);
				Column columnR     = GetColumn(columnCoord + _FlowCoordU000);
				Column columnB     = GetColumn(columnCoord - _FlowCoord0V00);
				Column columnT     = GetColumn(columnCoord + _FlowCoord0V00);
				Fluid  fluid       = GetColumnFluid(columnCoord);
				Fluid  fluidL      = GetColumnFluid(columnCoord - _FlowCoordU000);
				Fluid  fluidR      = GetColumnFluid(columnCoord + _FlowCoordU000);
				Fluid  fluidB      = GetColumnFluid(columnCoord - _FlowCoord0V00);
				Fluid  fluidT      = GetColumnFluid(columnCoord + _FlowCoord0V00);
				float  total       = column.GroundHeight + fluid.Depth;
				float  totalL      = columnL.GroundHeight + fluidL.Depth;
				float  totalR      = columnR.GroundHeight + fluidR.Depth;
				float  totalB      = columnB.GroundHeight + fluidB.Depth;
				float  totalT      = columnT.GroundHeight + fluidT.Depth;
				float  height      = GetHeight(columnCoord);
				float4 edge        = Boundary(columnCoord);
				float4 maxDelta    = total - float4(totalL, totalR, totalB, totalT);
				float4 outflow     = GetOutflow(columnCoord);

				// Stop high viscosity fluids if they're too shallow
				//maxDelta *= fluid.Depth * 2.0f > fluid.ESMV.w;
				//maxDelta *= fluid.Depth > 0.05f;

				maxDelta *= saturate((fluid.Depth * 2.0f - fluid.ESMV.w) * 10.0f);

				//if (fluid.Depth > 0.0f)
				{
					float4 flippedInflow = float4(columnR.Outflow.x, columnL.Outflow.y, columnT.Outflow.z, columnB.Outflow.w);
					//float4 depthSides    = float4(fluidL.Depth, fluidR.Depth, fluidB.Depth, fluidT.Depth) * edge;

					// Alive
					//outflow += flippedInflow * max(0.0f, depthSides / fluid.Depth) * 0.1f;

					// Inverse peaks
					//float4 bath = float4(columnR.GroundHeight, columnL.GroundHeight, columnT.GroundHeight, columnB.GroundHeight) - column.GroundHeight;
					//float4 dst = flippedInflow * bath * 0.1f;
					//outflow += flippedInflow * saturate(bath);

					// Stable waves
					outflow += flippedInflow * saturate(maxDelta * 0.15f) * (1 - pow(saturate(fluid.Depth*0.1f),3));
				}
				//outflow += (maxDelta >= 0.001f) * saturate(1.0f - fluid.Depth * 10.0f) * 0.01f;

				// Tiny waves
				//maxDelta *= maxDelta > 0.1f;

				outflow += maxDelta * 0.15f;

				outflow *= 1.0f - fluid.ESMV.w;

				outflow *= edge;

				return LimitOutflow(outflow, fluid.Depth);
			}
			ENDCG
		}
	}
}