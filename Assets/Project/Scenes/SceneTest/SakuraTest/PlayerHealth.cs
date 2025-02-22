using UnityEngine;
using UnityEngine.UI;

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
            }
        }
    }

    public void TakeDamage()
    {
        Debug.Log("ダメージ！");

        if (_isInvincible)
        {
            return;
        }

        if (_isDamaged)
        {
            if (!_isDebug)
            {
                AudioManager.Instance.StopBGM();
                GameOver();
            }
            return;
        }

        _isDamaged = true;
        _isInvincible = true;
        _damageTimer = 0f;
        _invincibilityTimer = 0f;
        _damageCount++;
    }

    public void GameOver()
    {
        SceneChangeManager.Instance.GameOver();
    }

    private void ToggleDebugMode()
    {
        _isDebug = !_isDebug;
    }
}
