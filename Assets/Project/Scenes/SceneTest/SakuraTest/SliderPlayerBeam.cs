using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


/// <summary>
/// プレイヤーのビームゲージを管理するクラス
/// </summary>
public class SliderPlayerBeam : MonoBehaviour
{
    [SerializeField]
    Slider DashGage; //スライダーのUI

    private float currentVelocity = 0;//スライダーの値をスムーズに変化させるための補助変数
    private PlayerBeam playerBeam;//PlayerBeamスクリプトの参照

    void Start()
    {
        // PlayerBeamコンポーネントを取得
        playerBeam = GetComponentInParent<PlayerBeam>();

        if (playerBeam == null)
        {
            Debug.LogError("PlayerBeam スクリプトが見つかりません！");
            return;
        }

        //スライダーの最大値を_attackMobにする
        DashGage.maxValue = playerBeam._attackMob;
        DashGage.value = 0;//初期値を0に設定
    }

    void Update()
    {
        if (playerBeam != null)
        {
            // _mobCounterをスライダーの現在地にスムーズに反映
            DashGage.value = Mathf.SmoothDamp(DashGage.value, playerBeam._mobCounter, ref currentVelocity, 0.1f);
        }
    }
    
    /// <summary>
    /// スライダーをリセットする(ビーム発射後に呼ばれる)
    /// </summary>
    public void ResetSlider()
    {
        DashGage.value = 0;
    }
}
