Shader "Custom/ImageEffect/DamageVignette"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Power ( "Power", Float ) = 0
    }
    
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.uv;
                o.uv.zw = o.vertex.xy;
                return o;
            }

            sampler2D _MainTex;
            float _Power;

            fixed4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D( _MainTex, i.uv.xy );
                float2 vp = (i.uv.zw);
                col.rgb -= dot(vp,vp) * _Power;
                return col;
            }
            ENDCG
        }
    }
}
