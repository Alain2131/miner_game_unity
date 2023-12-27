// https://pastebin.com/fuuTDpmT
Shader "Custom/World Space UVs"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Scale("Scale", Float) = 1
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float _Scale;


		struct Input
		{
			float3 worldPos;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			float3 color = tex2D(_MainTex, IN.worldPos.xy * _Scale);
			o.Albedo = color;
		}
		ENDCG

	}
}
