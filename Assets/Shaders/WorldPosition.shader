Shader "RSLib/WorldPosition"
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
            };

            struct VertexOutput
            {
                float4 clipSpacePos : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };
            
            VertexOutput vert(VertexInput input)
            {
                VertexOutput output;
                output.clipSpacePos = UnityObjectToClipPos(input.vertex);
                output.worldPos = mul(unity_ObjectToWorld, input.vertex);
                return output;
            }
            
            fixed4 frag(VertexOutput o) : SV_Target
            {
                return float4(o.worldPos, 1);
            }
            
            ENDCG
        }
    }
}
