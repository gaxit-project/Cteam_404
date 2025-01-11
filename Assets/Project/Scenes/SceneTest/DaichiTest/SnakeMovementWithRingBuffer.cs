using UnityEngine;

public class SnakeMovementWithRingBuffer : MonoBehaviour
{
    public GameObject segmentPrefab;  // �ւ̑̂̐߂̃v���n�u
    public int segmentCount = 10;    // �߂̐�
    public float segmentSpacing = 0.5f;  // �ߊԂ̋���
    public float speed = 5f;         // �擪�̈ړ����x
    public float rotationSpeed = 200f;  // ��]���x

    private Transform[] segments;    // �ւ̐߂��Ǘ�����z��
    private Vector3[] positionBuffer; // �����O�o�b�t�@�ňʒu�������Ǘ�
    private int bufferSize;          // �����O�o�b�t�@�̃T�C�Y
    private int bufferHead;          // �����O�o�b�t�@�̐擪�C���f�b�N�X

    void Start()
    {
        // �����O�o�b�t�@�̏������i�K�v�ȗ����T�C�Y���v�Z�j
        bufferSize = Mathf.CeilToInt((segmentCount + 1) * segmentSpacing / (speed * Time.fixedDeltaTime));
        positionBuffer = new Vector3[bufferSize];
        bufferHead = 0;

        // �����ʒu�������O�o�b�t�@�ɓo�^
        for (int i = 0; i < bufferSize; i++)
        {
            positionBuffer[i] = transform.position;
        }

        // �ւ̐߂𐶐�
        segments = new Transform[segmentCount];
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = Instantiate(segmentPrefab, transform.position, Quaternion.identity);
            segments[i] = segment.transform;
        }
    }

    void FixedUpdate()
    {
        // �擪�̈ړ�����
        float move = speed * Time.fixedDeltaTime;

        transform.Translate(Vector3.forward * move);

        // ���E�̉�]����
        float turn = Input.GetAxis("Horizontal") * rotationSpeed * Time.fixedDeltaTime;
        transform.Rotate(Vector3.up * turn);

        // �㉺�̉�]����
        float turn2 = Input.GetAxis("Vertical") * rotationSpeed * Time.fixedDeltaTime;
        transform.Rotate(Vector3.right * turn2);

        // ���݈ʒu�������O�o�b�t�@�ɋL�^
        bufferHead = (bufferHead + 1) % bufferSize; // �����O�o�b�t�@�̐擪��i�߂�
        positionBuffer[bufferHead] = transform.position;

        // �߂̒Ǐ]����
        for (int i = 0; i < segments.Length; i++)
        {
            // �����̂ǂ̈ʒu���Q�Ƃ��邩�v�Z
            int index = (bufferHead - Mathf.CeilToInt((i + 1) * segmentSpacing / move) + bufferSize) % bufferSize;

            // �߂��^�[�Q�b�g�ʒu�ֈړ��i��Ԃ�������j
            segments[i].position = Vector3.Lerp(segments[i].position, positionBuffer[index], Time.fixedDeltaTime * speed);
        }
    }
}
