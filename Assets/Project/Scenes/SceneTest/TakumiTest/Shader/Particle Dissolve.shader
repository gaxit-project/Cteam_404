Shader "Custom/Particle Dissolve"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {} // メインのテクスチャ
        _NoiseTex ("Noise", 2D) = "white" {} // ディゾルブ用のノイズテクスチャ
        _Gradient ("Gradient", Range(0.0, 1.0)) = 0.1 // ディゾルブの滑らかさを調整
        [KeywordEnum(None, Front, Back)] _Cull("Culling", Int) = 2 // カリングの設定
    }
 
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" } // 透明用のレンダーキュー
        Blend SrcAlpha One // 加算ブレンド（透過処理）
        Cull [_Cull] // カリング（None, Front, Back のいずれか）
        Lighting Off ZWrite Off Fog { Mode Off } // ライティング、Z書き込み、フォグを無効化
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert // 頂点シェーダー
            #pragma fragment frag // フラグメント（ピクセル）シェーダー
            #pragma fragmentoption ARB_precision_hint_fastest // 高速処理の最適化
            #pragma multi_compile_particles // パーティクル用のマルチコンパイル

            #include "UnityCG.cginc"

            // テクスチャの定義
            sampler2D _MainTex; // メインテクスチャ（ディフューズ用）
            sampler2D _NoiseTex; // ノイズテクスチャ（ディゾルブ用）

            // 頂点シェーダーの入力（頂点データ）
            struct appdata_t
            {
                float4 vertex : POSITION; // 頂点座標
                fixed4 color : COLOR; // 頂点カラー（アルファ値を含む）
                float2 texcoord : TEXCOORD0; // UV座標
            };

            // 頂点シェーダーの出力（フラグメントシェーダーへの入力）
            struct v2f
            {
                float4 vertex : POSITION; // クリップ空間での座標
                fixed4 color : COLOR; // 頂点カラー
                float4 texcoord : TEXCOORD0; // UV座標（xy: メインテクスチャ, zw: ノイズテクスチャ）
            };

            // テクスチャのUV変換用スケール
            float4 _MainTex_ST;
            float4 _NoiseTex_ST;
            float _Gradient; // ディゾルブの滑らかさ調整用

            // 頂点シェーダー
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // 頂点座標をクリップ空間に変換
                o.color = v.color; // 頂点カラーをそのまま渡す
                o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex); // メインテクスチャのUV変換
                o.texcoord.zw = TRANSFORM_TEX(v.texcoord, _NoiseTex); // ノイズテクスチャのUV変換
                return o;
            }

            // フラグメント（ピクセル）シェーダー
            fixed4 frag (v2f i) : SV_Target
            {
                // メインテクスチャの色取得
                fixed4 col = tex2D(_MainTex, i.texcoord.xy);
                fixed texAlpha = col.a; // メインテクスチャのアルファ値

                // ノイズテクスチャの取得（赤チャンネルを使用）
                fixed maskAlpha = tex2D(_NoiseTex, i.texcoord.zw).r;

                // 頂点カラーのアルファ（フェード用）
                fixed vtxAlpha = 1.0 - i.color.a;

                // 頂点カラーの影響をメインテクスチャのRGBに乗算
                col.rgb *= i.color.rgb;

                // ディゾルブ用のアルファ値計算
                maskAlpha = maskAlpha * max(1.0 - _Gradient, 0.0) + _Gradient;

                // ディゾルブ処理（一定の閾値を超えたピクセルだけ描画）
                clip(texAlpha * maskAlpha - vtxAlpha - 0.01);

                // 滑らかなフェードアウトを適用
                col.a = smoothstep(vtxAlpha, vtxAlpha + _Gradient, maskAlpha);
      
                return col;
            }
            ENDCG 
        }
    }  
    FallBack Off // フォールバックシェーダーなし
}
