using UnityEngine;
using System.Collections;

public class MissileAttack : MonoBehaviour
{
    [Header("ミサイル設定")]
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private int missileCount = 3;  //ミサイルの数
    [SerializeField] private float missileLaunchInterval = 1f;  //ミサイルの発射間隔
    [SerializeField] private float missileSpeed = 10f;  //ミサイルの速度
    [SerializeField] private GameObject warningAreaPrefab;
    [SerializeField] private float warningAreaOffsetDistance = 5f; //PlayerとWarningAreaの距離

    [Header("生成位置設定")]
    [SerializeField] private Transform missileSpawnPoint;  //ミサイルの生成位置

    [Header("目標地点設定")]
    [SerializeField] private float targetHeightOffset = 7f;


    private Player player;
    private GameObject[] warningAreaInstances;
    private AudioSource audioSource;

    void Start()
    {
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            return;
        }

        warningAreaInstances = new GameObject[missileCount];
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void ExecuteAttack()
    {
        StartCoroutine(LaunchMissiles());
    }

    private IEnumerator LaunchMissiles()
    {
        for (int i = 0; i < missileCount; i++)
        {
            Vector3 playerForwardDirection = player.transform.forward;
            Vector3 warningAreaPosition = player.transform.position + playerForwardDirection * warningAreaOffsetDistance;
            GameObject warningArea = Instantiate(warningAreaPrefab, warningAreaPosition, Quaternion.identity);
            warningAreaInstances[i] = warningArea;
            AudioManager.GetInstance().PlaySound(10);

            Vector3 landingPosition = warningArea.transform.position;

            float noiseValue = Mathf.PerlinNoise(player.transform.position.x, player.transform.position.z);
            Vector3 targetPosition = landingPosition + Vector3.up * targetHeightOffset;

            GameObject missile = Instantiate(missilePrefab, missileSpawnPoint.position, Quaternion.identity);
            Missile missileScript = missile.GetComponent<Missile>();

            if (missileScript != null)
            {
                missileScript.SetTarget(targetPosition, landingPosition, missileSpeed, warningArea);
            }

            yield return new WaitForSeconds(0.4f);

            AudioManager.GetInstance().PlaySound(11);

            yield return new WaitForSeconds(missileLaunchInterval);
        }

        yield return new WaitForSeconds(3f);

        ResetWarningAreas();
    }

    /// <summary>
    /// すべてのWarningAreaを削除する
    /// </summary>
    private void ResetWarningAreas()
    {
        for (int i = 0; i < warningAreaInstances.Length; i++)
        {
            if (warningAreaInstances[i] != null)
            {
                Destroy(warningAreaInstances[i]);
            }
        }
    }
}
