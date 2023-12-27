Shader "Unlit/Sample_from_ComputeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scale("Scale", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
            float2 samplePos = IN.worldPos.xy;
            // Scale "from the top-left" instead of bottom-left.
            samplePos.y += _Scale;
            samplePos /= _Scale;
            
            float3 color = tex2D(_MainTex, samplePos);
            o.Albedo = color;
        }
        ENDCG
    }
}
