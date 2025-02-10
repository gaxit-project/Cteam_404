using UnityEngine;
using SplineMesh;

[CreateAssetMenu(fileName = "LaserAttack", menuName = "BossAttacks/Laser")]
public class LaserAttack : ScriptableObject, IBossAttack
{
    [Header("���[�U�[�̃v���n�u")]
    public GameObject laserPrefab;

    [Header("���ˈʒu�̃I�t�Z�b�g")]
    public float forwardOffset = 3f;

    public void ExecuteAttack(GameObject boss)
    {
        PlayerMove player = GameObject.FindObjectOfType<PlayerMove>();
        if (player == null || player.CurrentRail == null)
        {
            Debug.LogWarning("�v���C���[�܂��̓��[����������܂���");
            return;
        }

        GameObject laser = Instantiate(laserPrefab);
        LaserFollow followScript = laser.AddComponent<LaserFollow>();

        // �Ǐ]�p�X�N���v�g�Ƀv���C���[����n��
        followScript.Initialize(player, forwardOffset);
    }
}
