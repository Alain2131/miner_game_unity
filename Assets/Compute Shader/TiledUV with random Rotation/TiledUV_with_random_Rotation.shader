Shader "Unlit/TiledUV_with_random_Rotation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scale ("Scale", float) = 1
        [MaterialToggle] _VisUVs ("VisualizeUVs", int) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        float _Scale;
        int _VisUVs;


        struct Input
        {
            float3 worldPos;
        };

        // https://thebookofshaders.com/10/
        float random(float2 uv, float seed = 43758.5453)
        {
            return frac(sin(dot(uv, float2(12.9898, 78.233))) * seed);
        }

        void rotate90(inout float3 UVs)
        {
            UVs = UVs.yxz;
            UVs.x = (UVs.x * -1) + 1;
        }
        void rotate180(inout float3 UVs)
        {
            UVs.x = (UVs.x * -1) + 1;
            UVs.y = (UVs.y * -1) + 1;
        }
        void rotate270(inout float3 UVs)
        {
            UVs = UVs.yxz;
            UVs.y = (UVs.y * -1) + 1;
        }

        
        void surf(Input IN, inout SurfaceOutput o)
        {
            float3 samplePos = IN.worldPos.xyz;
            samplePos.z = 0;

            samplePos /= _Scale;
            float3 UVs = frac(samplePos);

            float3 ID = floor(samplePos);

            /*
             * All rotation possibilities
            if (ID.x == 1) // 0
            {
                UVs = UVs.yxz;
                UVs.x *= -1;
                UVs.y *= -1;

                UVs.x += 1;
                UVs.y += 1;
            }

            if (ID.x == 2) // 90
            {
                UVs = UVs.yxz;
                UVs.x *= -1;

                UVs.x += 1;
            }
            if (ID.x == 3) // 90
            {
                UVs.x *= -1;

                UVs.x += 1;
            }

            if (ID.x == 4) // 180
                UVs = UVs.yxz;
            if (ID.x == 5) // 180
            {
                UVs.x *= -1;
                UVs.y *= -1;

                UVs.x += 1;
                UVs.y += 1;
            }
            
            if (ID.x == 6) // 270
            {
                UVs = UVs.yxz;
                UVs.y *= -1;

                UVs.y += 1;
            }
            if (ID.x == 7) // 270
            {
                UVs.y *= -1;
                
                UVs.y += 1;
            }
            */
            
            
            // "Rotate" UV
            float randValue = random(ID.xy);
            if(randValue < 0.25)
                rotate90(UVs);
            else if(randValue < 0.5)
                rotate180(UVs);
            else if(randValue < 0.75)
                rotate270(UVs);


            float3 color = tex2D(_MainTex, UVs);
            if (_VisUVs == 1)
                color = UVs;
            
            //color = ID * 0.1; // *0.1 for visualisation
            o.Albedo = color;
        }
        ENDCG
    }
}
