Shader "RSLib/UV"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }

        Pass
        {
            CGPROGRAM
            
            #pragma vertex vert // Defines the name of the vertex shader method.
            #pragma fragment frag // Defines the name of the fragment shader method.

            #include "UnityCG.cginc"

            struct VertexInput
            {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 vertex : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };

            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv0 = v.uv0;
                return o;
            }

            fixed4 frag(VertexOutput o) : SV_Target
            {
                return float4(o.uv0, 0, 0);
            }
            
            ENDCG
        }
    }
}
