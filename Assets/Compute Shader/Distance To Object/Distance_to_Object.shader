Shader "Unlit/Distance_to_Object"
{
	Properties
	{
		_Maximum("Maximum Distance", Float) = 1
		_Origin("Origin", Vector) = (0,0,0,-1)
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert

		float _Maximum;
		float3 _Origin;


		struct Input
		{
			float3 worldPos;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			float value = distance(_Origin, IN.worldPos);

			// Remap, doesn't work or I'm dumb
			/*
			float from = value;
			float fromMin = 0;
			float fromMax = _Maximum; // HHHHHH
			float toMin = fromMin;
			float toMax = _Maximum;

			float fromAbs = from - fromMin;
			float fromMaxAbs = fromMax - fromMin;

			float normal = fromAbs / fromMaxAbs;

			float toMaxAbs = toMax - toMin;
			float toAbs = toMaxAbs * normal;

			float to = toAbs + toMin;
			*/
			o.Albedo = pow(value / _Maximum, 10);
		}
		ENDCG

	}
}
