using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private bool _isDamaged = false;
    private bool _isInvincible = false;
    private bool _isDebug = false;
    private float _damageTimer = 0f;
    private float _invincibilityTimer = 0f;
    private int _damageCount = 0;
    private string _enemyTag = "Mob";
    private PlayerCol playerCol;
    private Coroutine blinkCoroutine;
    public bool isHit = false;

    [Header("ダメージのクールダウン時間")]
    [SerializeField]
    private float _damageCooldownTime = 3f;

    [Header("無敵時間")]
    [SerializeField]
    private float _invincibilityTime = 2f;

    [Header("ダメージ時に色が変わる画面")]
    [SerializeField]
    private Image _damageImg;

    [Header("ダメージエフェクトの点滅速度")]
    [SerializeField]
    private float _blinkSpeed = 5f; // 点滅速度

    [Header("無敵時間のプレイヤーの点滅速度")]
    [SerializeField]
    private float _playerBlinkSpeed = 0.2f;

    [Header("点滅するプレイヤーのRenderer")]
    [SerializeField]
    private SkinnedMeshRenderer[] _playerRenderers;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(_enemyTag))
        {
            Debug.Log("ダメージを食らった1");
            TakeDamage();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(_enemyTag))
        {
            Debug.Log("ダメージを食らった2");
            isHit = true;
            //TakeDamage();
        }
    }

    private void Start()
    {
        playerCol = GetComponent<PlayerCol>();
        _isDamaged = false;
        _isInvincible = false;
        _isDebug = false;
}

    void Update()
    {
        HandleDamageRecovery();
        HandleInvincibility();
        HandleDamageEffect();

        // デバッグ：Rキーでデバッグモードの切り替え
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleDebugMode();
        }
    }

    /// <summary>
    /// ダメージエフェクトの処理
    /// </summary>
    private void HandleDamageEffect()
    {
        if (_isDamaged)
        {
            float alpha = (Mathf.Sin(Time.time * _blinkSpeed) + 1f) / 2f; // 0～1の範囲で変化
            _damageImg.color = new Color(0.7f, 0, 0, alpha * 0.7f); // 最大0.7の透明度で点滅
        }
        else
        {
            _damageImg.color = Color.clear;
        }
    }

    private void HandleDamageRecovery()
    {
        if (_isDamaged)
        {
            _damageTimer += Time.deltaTime;
            if (_damageTimer >= _damageCooldownTime)
            {
                AudioManager.GetInstance().PlaySound(3);
                _isDamaged = false;
                _damageTimer = 0f;
            }
        }
    }

    private void HandleInvincibility()
    {
        if (_isInvincible)
        {
            _invincibilityTimer += Time.deltaTime;
            if (_invincibilityTimer >= _invincibilityTime)
            {
                _isInvincible = false;
                _invincibilityTimer = 0f;
                StopBlinking(); //無敵終了時に点滅を止める
            }
        }
    }

    public void TakeDamage()
    {

        if (_isInvincible)
        {
            return;
        }

        Debug.Log("ダメージ！");

        if (_isDamaged)
        {
            if (!_isDebug)
            {
                GameOver();
            }
            return;
        }
        else
        {
            AudioManager.Instance.PlayDamageSound(2);
        }

        _isDamaged = true;
        _isInvincible = true;
        _invincibilityTimer = 0f;
        _damageCount++;

        if (blinkCoroutine != null)
        {
            Debug.Log("コルーチン停止");
            StopCoroutine(blinkCoroutine);
        }

        StartCoroutine(BlinkPlayer(_invincibilityTime));
        Debug.Log("プレイヤー点滅開始");

        _damageTimer = 0f;
    }

    /// <summary>
    /// プレイヤーを無敵時間中に点滅させる
    /// </summary>
    private IEnumerator BlinkPlayer(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            foreach (var renderer in _playerRenderers) // 表示・非表示を切り替え
            {
                renderer.enabled = !renderer.enabled;
            }

            yield return new WaitForSeconds(_playerBlinkSpeed);
            elapsedTime += _playerBlinkSpeed;
        }

        foreach (var renderer in _playerRenderers)// 無敵終了後は表示をONにする
        {
            renderer.enabled = true;
        }
    }

    /// <summary>
    /// プレイヤーの点滅を停止する（無敵終了時）
    /// </summary>
    private void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }

        foreach (var renderer in _playerRenderers)
        {
            renderer.enabled = true;
        }
        Debug.Log("無敵終了後、表示ON");
    }

    public void GameOver()
    {
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.StopSound();
        SceneChangeManager.Instance.GameOver();
    }

    private void ToggleDebugMode()
    {
        _isDebug = !_isDebug;
    }

    public void SetIsInvincible()
    {
        _isInvincible = true;
    }



}
