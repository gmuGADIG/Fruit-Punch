Shader "Unlit/InnerSpriteOutline HLSL"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OverwriteColor("Overwrite Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
//        Blend SrcAlpha OneMinusSrcAlpha
        cull off
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
 
			fixed4 _OutlineColor;
            fixed4 _OverwriteColor;
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // outline
				fixed leftPixel = tex2D(_MainTex, i.uv + float2(-_MainTex_TexelSize.x, 0)).a;
				fixed upPixel = tex2D(_MainTex, i.uv + float2(0, _MainTex_TexelSize.y)).a;
				fixed rightPixel = tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x, 0)).a;
				fixed bottomPixel = tex2D(_MainTex, i.uv + float2(0, -_MainTex_TexelSize.y)).a;
    
                fixed outline = max(max(leftPixel, upPixel), max(rightPixel, bottomPixel)) - col.a;
                fixed4 finalColor = lerp(col, _OutlineColor, outline);
                finalColor.rgb = lerp(finalColor.rgb, _OverwriteColor.rgb, _OverwriteColor.a);

                clip(finalColor.a - 0.5);
                
                return finalColor;
            }
            ENDCG
        }
    }
}