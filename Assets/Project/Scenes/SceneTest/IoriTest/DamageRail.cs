using UnityEngine;

public class DamageRail : MonoBehaviour
{
    [Header("RailManager")]
    [SerializeField] RailManager _railManager;

    [Header("PlayerHealth")]
    [SerializeField] PlayerHealth _playerHealth;

    [Header("ダメージレールパーティクル")]
    [SerializeField] ParticleSystem _damageRailEffect;

    [Header("テスト用参照Index")]
    [SerializeField] private int _testIndex;

    private Vector3 NearestPosition;

    private void Start()
    {
        if(_railManager == null)
        {
            _railManager = GetComponent<RailManager>();
        }
        if(_playerHealth == null)
        {
            _playerHealth = GetComponent<PlayerHealth>();
        }

        
        
    }

    private void Update()
    {
        
    }

    private bool OnDamageRail()
    {
        for(int i = 0; i < _testIndex + 4; i++)
        {
            NearestPosition = _railManager.GetNearPosition(_testIndex);

        }

    }

}
