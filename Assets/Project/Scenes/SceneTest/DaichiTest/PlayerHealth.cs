using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private bool isDamaged = false;        // ダメージを受けているか
    private bool isInvincible = false;     // 無敵時間であるか
    private bool isDebug = false;          // デバッグモードが有効か
    private float damageTimer = 0f;        // ダメージを受けてからの経過時間
    private float invincibilityTimer = 0f; // 無敵状態の経過時間
    private int damageCount = 0;           // ダメージを受けた回数

    [Header("ダメージのクールダウン時間")]
    [SerializeField]
    private float damageCooldownTime = 3f; // ダメージの回復時間

    [Header("無敵時間")]
    [SerializeField]
    private float invincibilityTime = 2f;  // 無敵時間

    void Update()
    {
        HandleDamageRecovery();
        HandleInvincibility();

        // デバッグ：Dキーでデバッグモードの切り替え
        if (Input.GetKeyDown(KeyCode.D))
        {
            ToggleDebugMode();
        }
    }

    /// <summary>
    /// ダメージを受けてからの回復時間をチェックし，isDamagedを更新
    /// </summary>
    private void HandleDamageRecovery()
    {
        if (isDamaged)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageCooldownTime)
            {
                isDamaged = false;
                damageTimer = 0f;
            }
        }
    }

    /// <summary>
    /// 無敵時間をチェックし，isInvincibleを更新
    /// </summary>
    private void HandleInvincibility()
    {
        if (isInvincible)
        {
            invincibilityTimer += Time.deltaTime;
            if (invincibilityTimer >= invincibilityTime)
            {
                isInvincible = false;
                invincibilityTimer = 0f;
            }
        }
    }

    /// <summary>
    /// ダメージを受けた時の処理
    /// </summary>
    public void TakeDamage()
    {
        // 無敵の場合はダメージを無効化
        if (isInvincible)
        {
            return;
        }

        // ダメージを受けている最中に再度ダメージを受けたらゲームオーバー
        if (isDamaged)
        {
            if (!isDebug)
            {
                GameOver();
            }
            return;
        }

        // 初回ダメージ処理
        isDamaged = true;
        isInvincible = true;
        damageTimer = 0f; // ダメージタイマーをリセット
        invincibilityTimer = 0f; // 無敵タイマーをリセット
        damageCount++;
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
        isDebug = !isDebug;
    }
}
