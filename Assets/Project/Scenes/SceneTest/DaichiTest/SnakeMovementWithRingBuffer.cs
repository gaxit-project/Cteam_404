using UnityEngine;

public class SnakeMovementWithRingBuffer : MonoBehaviour
{
    public GameObject segmentPrefab;  // 蛇の体の節のプレハブ
    public int segmentCount = 10;    // 節の数
    public float segmentSpacing = 0.5f;  // 節間の距離
    public float speed = 5f;         // 先頭の移動速度
    public float rotationSpeed = 200f;  // 回転速度

    private Transform[] segments;    // 蛇の節を管理する配列
    private Vector3[] positionBuffer; // リングバッファで位置履歴を管理
    private int bufferSize;          // リングバッファのサイズ
    private int bufferHead;          // リングバッファの先頭インデックス

    void Start()
    {
        // リングバッファの初期化（必要な履歴サイズを計算）
        bufferSize = Mathf.CeilToInt((segmentCount + 1) * segmentSpacing / (speed * Time.fixedDeltaTime));
        positionBuffer = new Vector3[bufferSize];
        bufferHead = 0;

        // 初期位置をリングバッファに登録
        for (int i = 0; i < bufferSize; i++)
        {
            positionBuffer[i] = transform.position;
        }

        // 蛇の節を生成
        segments = new Transform[segmentCount];
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = Instantiate(segmentPrefab, transform.position, Quaternion.identity);
            segments[i] = segment.transform;
        }
    }

    void FixedUpdate()
    {
        // 先頭の移動処理
        float move = speed * Time.fixedDeltaTime;

        transform.Translate(Vector3.forward * move);

        // 左右の回転処理
        float turn = Input.GetAxis("Horizontal") * rotationSpeed * Time.fixedDeltaTime;
        transform.Rotate(Vector3.up * turn);

        // 上下の回転処理
        float turn2 = Input.GetAxis("Vertical") * rotationSpeed * Time.fixedDeltaTime;
        transform.Rotate(Vector3.right * turn2);

        // 現在位置をリングバッファに記録
        bufferHead = (bufferHead + 1) % bufferSize; // リングバッファの先頭を進める
        positionBuffer[bufferHead] = transform.position;

        // 節の追従処理
        for (int i = 0; i < segments.Length; i++)
        {
            // 履歴のどの位置を参照するか計算
            int index = (bufferHead - Mathf.CeilToInt((i + 1) * segmentSpacing / move) + bufferSize) % bufferSize;

            // 節をターゲット位置へ移動（補間を加える）
            segments[i].position = Vector3.Lerp(segments[i].position, positionBuffer[index], Time.fixedDeltaTime * speed);
        }
    }
}
