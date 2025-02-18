using UnityEngine;

public class PlayerBeam : MonoBehaviour
{
    private float _beamTimer = 0f;  // ビームの発車時間を管理するタイマー
    private bool _isFiring = false; // ビームが発射中かどうか
    private ParticleSystem particle; // ビームのエフェクト用のパーティクルシステム
    public int _mobCounter = 0;      // 倒したモブの数

    [Header("ビームチャージに必要なモブ数")]
    [SerializeField]
    public int _attackMob; //ビームを発射するために必要なモブの撃破数

    [Header("ビーム発射時間")]
    [SerializeField]
    private float _beamDepartureTime; //ビームの発射時間

    private SliderPlayerBeam sliderPlayerBeam;　//ビームチャージ用スライダーの管理スクリプト

    private void Start()
    {
        //パーティクルシステムを取得
        particle = GetComponentInChildren<ParticleSystem>();

        //スライダーUIを管理するスクリプトを取得
        sliderPlayerBeam = GetComponentInChildren<SliderPlayerBeam>();

        if (sliderPlayerBeam == null)
        {
            Debug.LogError("SliderPlayerBeam スクリプトが見つかりません！");
        }
    }

    /// <summary>
    /// モブにヒットした回数を加算するメソッド
    /// </summary>
    public void AddMobHit()
    {
        _mobCounter++;//撃破数を増やす
        Debug.Log("モブヒット回数: " + _mobCounter);
    }

    void Update()
    {
        //　モブ撃破数が必要数に達した場合、Enterキーでビームを発射できる
        if (_mobCounter >= _attackMob)
        {
            if (Input.GetKeyDown(KeyCode.Return) && !_isFiring)
            {
                StartBeam();

            }
            
        }

        //ビーム発射中の時、発射時間をカウントする
        if (_isFiring)
        {
            _beamTimer += Time.deltaTime;

            //ビームの発射時間が設定値を超えたらビーム停止
            if (_beamTimer >= _beamDepartureTime)
            {
                StopBeam();
            }
        }
    }

    /// <summary>
    /// ビームを発射する処理
    /// </summary>
    void StartBeam()
    {
        if (particle == null)
        {
            Debug.LogError("ParticleSystem が見つかりません！");
            return;
        }

        _mobCounter = 0;　//撃破カウントをリセット
        particle.Play();  //ビームエフェクトを再生
        _beamTimer = 0f;　//タイマーをリセット
        _isFiring = true; //ビーム発射状態に変更
        Debug.Log("ビーム発射");
        

        //スライダーのゲージをリセット
        if (sliderPlayerBeam != null)
        {
            sliderPlayerBeam.ResetSlider();
        }
    }

    /// <summary>
    /// ビームを停止する処理
    /// </summary>
    void StopBeam()
    {
        particle.Stop();    //ビームエフェクトを停止
        _isFiring = false;  //ビーム発射状態を解除
        Debug.Log("ビームストップ");
    }
}
