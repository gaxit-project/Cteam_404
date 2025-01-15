using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private bool _isDamaged = false;        // ダメージを受けているか
    private bool _isInvincible = false;     // 無敵時間であるか
    private bool _isDebug = false;          // デバッグモードが有効か
    private float _damageTimer = 0f;        // ダメージを受けてからの経過時間
    private float _invincibilityTimer = 0f; // 無敵状態の経過時間
    private int _damageCount = 0;           // ダメージを受けた回数
    private string _enemyTag = "Mob";       // 敵オブジェクトを識別するタグ

    [Header("ダメージのクールダウン時間")]
    [SerializeField]
    private float _damageCooldownTime = 3f; // ダメージの回復時間

    [Header("無敵時間")]
    [SerializeField]
    private float _invincibilityTime = 2f;  // 無敵時間

    [Header("ダメージ時に色が変わる画面")]
    [SerializeField] 
    Image DamageImg;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Mob"))
        {
            TakeDamage();
        }
    }

    void Update()
    {
        HandleDamageRecovery();
        HandleInvincibility();

        DamageImg.color = Color.Lerp(DamageImg.color, Color.clear, Time.deltaTime);

        // デバッグ：Rキーでデバッグモードの切り替え
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleDebugMode();
        }
    }

    /// <summary>
    /// ダメージを受けてからの回復時間をチェックし，isDamagedを更新
    /// </summary>
    private void HandleDamageRecovery()
    {
        if (_isDamaged)
        {
            _damageTimer += Time.deltaTime;
            if (_damageTimer >= _damageCooldownTime)
            {
                _isDamaged = false;
                _damageTimer = 0f;
            }
        }
    }

    /// <summary>
    /// 無敵時間をチェックし，isInvincibleを更新
    /// </summary>
    private void HandleInvincibility()
    {
        if (_isInvincible)
        {
            _invincibilityTimer += Time.deltaTime;
            if (_invincibilityTimer >= _invincibilityTime)
            {
                _isInvincible = false;
                _invincibilityTimer = 0f;
            }
        }
    }

    /// <summary>
    /// ダメージを受けた時の処理
    /// </summary>
    public void TakeDamage()
    {
        // 無敵の場合はダメージを無効化
        if (_isInvincible)
        {
            return;
        }

        // ダメージを受けている最中に再度ダメージを受けたらゲームオーバー
        if (_isDamaged)
        {
            if (!_isDebug)
            {
                GameOver();
            }
            return;
        }

        DamageImg.color = new Color(0.7f, 0, 0, 0.7f);

        // 初回ダメージ処理
        _isDamaged = true;
        _isInvincible = true;
        _damageTimer = 0f; // ダメージタイマーをリセット
        _invincibilityTimer = 0f; // 無敵タイマーをリセット
        _damageCount++;

    }

    /// <summary>
    /// ゲームオーバー時の処理
    /// </summary>
    public void GameOver()
    {
        //　ゲームオーバーになった時に起こるイベントなどを挿入する
    }

    // デバッグモードの切り替え
    private void ToggleDebugMode()
    {
        _isDebug = !_isDebug;
    }
}
