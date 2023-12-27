Shader "Unlit/ComputeShader_Noise"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;

        struct Input
        {
            float3 worldPos;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            float2 samplePos = IN.worldPos.xy;
            float3 color = tex2D(_MainTex, samplePos);
            o.Albedo = color;
        }
        ENDCG
    }
}
