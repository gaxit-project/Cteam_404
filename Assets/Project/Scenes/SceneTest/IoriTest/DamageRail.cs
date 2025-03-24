using UnityEngine;

public class DamageRail : MonoBehaviour
{
    [Header("RailManager")]
    [SerializeField] RailManager _railManager;

    [Header("PlayerHealth")]
    [SerializeField] PlayerHealth _playerHealth;

    [Header("PlayerPosition")]
    [SerializeField] Transform _player;

    [Header("ダメージレールパーティクル")]
    [SerializeField] ParticleSystem _damageRailEffect;

    [Header("テスト用参照Index")]
    [SerializeField] private int _testIndex;

    [Header("ダメージレールの長さ")]
    [SerializeField] private int _lengthIndex;

    private Vector3[] DamageObject;

    private void Start()
    {
        if (_railManager == null)
        {
            _railManager = GetComponent<RailManager>();
        }
        if (_playerHealth == null)
        {
            _playerHealth = FindObjectOfType<PlayerHealth>();
        }

        DamageObject = new Vector3[_lengthIndex];

        for (int i = 0; i < _lengthIndex; i++)
        {
            DamageObject[i] = _railManager.GetNearPosition(_testIndex + i);
        }
    }

    private void Update()
    {
        for (int i = 0; i < _lengthIndex - 1; i++)
        {
            if (OnDamageRail(_player.position, DamageObject[i], DamageObject[(i + 1) % _lengthIndex]))
            {
                Debug.Log("ダメージレールに乗っている");
                return;
            }
        }
    }

    private bool OnDamageRail(Vector3 PlayerPosition, Vector3 StartObject, Vector3 FinishObject, float threshold = 0.5f)
    {
        Vector3 AB = FinishObject - StartObject;
        Vector3 AP = PlayerPosition - StartObject;

        float t = Mathf.Clamp(Vector3.Dot(AP, AB) / Vector3.Dot(AB, AB), 0f, 1f);

        Vector3 closestPoint = StartObject + t * AB;

        return Vector3.Distance(PlayerPosition, closestPoint) < threshold;
    }
}
