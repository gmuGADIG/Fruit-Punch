Shader "Custom/InnerBillboard"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _UvOffset ("UV Offset", Vector) = (0, 0, 0, 0)
        _CamToTexScale ("Camera-To-Texture Scale", Vector) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _UvOffset;
            float4 _CamToTexScale;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate texture coordinates based on screen position
                float2 uv = i.screenPos.xy;
                uv.xy /= _CamToTexScale.xy;
                
                // Apply offset
                uv += _UvOffset.xy;
                
                // Sample the texture
                fixed4 col = tex2D(_MainTex, uv);

                // debug
                // col *= .8;

                return col;
                // return float4(i.screenPos.x, i.screenPos.y, 0, 1);
            }
            ENDCG
        }
    }
}