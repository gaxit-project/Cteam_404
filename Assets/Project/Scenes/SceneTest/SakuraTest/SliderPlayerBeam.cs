using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーのビームゲージを管理するクラス
/// </summary>
public class SliderPlayerBeam : MonoBehaviour
{
    private Slider ULTGauge; // スライダーのUI
    private Player playerScript; // Playerスクリプトの参照
    private float sliderTargetValue; // 目標値
    private float upSpeed = 2f;
    private float changeSpeed = 2.0f; // スライダーの変化速度（0.5秒で変化するよう調整）


    void Start()
    {
        // スライダーのUIコンポーネント取得
        ULTGauge = GetComponent<Slider>();

        // Playerスクリプトを取得
        playerScript = GameObject.Find("Player").GetComponent<Player>();

        // スライダーの最大値と初期値を設定
        ULTGauge.maxValue = 1;// playerScript._attackMob;
        ULTGauge.value = playerScript.UltGauge;
        sliderTargetValue = playerScript._mobCounter;
    }



    private void LateUpdate()
    {

        ULTGauge.value = playerScript.UltGauge;
        /*

        // モブの撃破数が増えたらスライダーを更新
        if (playerScript._mobCounter > playerScript.prevMobCounter)
        {
            sliderTargetValue = playerScript._mobCounter;
            changeSpeed = upSpeed;
        }

        // ULT発動（適宜変更）
        if (playerScript.isULT) 
        {
            sliderTargetValue = 0;
            changeSpeed = (1 / playerScript._ultTime) * 2;
        }


        ULTGauge.value = Mathf.Lerp(ULTGauge.value, sliderTargetValue, Time.deltaTime * changeSpeed);
        playerScript.prevMobCounter = playerScript._mobCounter;
        */
    }
}
