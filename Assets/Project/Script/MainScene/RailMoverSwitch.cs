using UnityEngine;
using SplineMesh; // SplineMeshを使用

public class RailMoverSwitch : MonoBehaviour
{

    /// <summary>
    /// このスクリプトははコーディング規約に則っていません
    /// class名とスクリプト名は変わる予定です
    /// まだ変更点があるので，出来次第整理します．
    /// 何ならPlayerに組み込まれると思います．
    /// </summary>






    private bool OnRail = false;

    [Header("レール設定")]
    public Spline currentRail;             // 現在のレール
    public float speed = 5f;               // レール上の移動速度
    private float railPosition = 0f;       // レール上の進行度 (0〜1)

    [Header("Connected Rails")]
    public Spline[] connectedRails;        // 接続可能な他のレール
    private bool switching = false;        // レール切り替え中かどうか
    private Spline nextRail;               // 次に移動するレール




    void Update()
    {
        if (switching) return;

        // レールに沿った移動
        railPosition += speed * Time.deltaTime / currentRail.Length;

        // レール終点で停止
        if (railPosition >= 0.9f/*currentRail.Length*/)
        {
            railPosition = 0f;
        }

        // レール上の現在の位置と向きを計算
        MoveAlongRail();

        // ユーザー入力によるレール切り替え
        /*if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SwitchRail(-1); // 左レールへの切り替え
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwitchRail(1); // 右レールへの切り替え
        }*/


    }

    void MoveAlongRail()
    {
        var splineSample = currentRail.GetSampleAtDistance(railPosition * currentRail.Length);
        transform.position = splineSample.location;   // レール上の位置
        transform.forward = splineSample.tangent;     // レールに沿った向き
    }

    void SwitchRail(int direction)
    {
        if (connectedRails.Length == 0) return;

        // 接続レールのインデックス計算
        int targetIndex = Mathf.Clamp(direction + connectedRails.Length / 2, 0, connectedRails.Length - 1);
        nextRail = connectedRails[targetIndex];

        // 切り替えアニメーションを開始
        StartCoroutine(SwitchingAnimation(nextRail));
    }

    private System.Collections.IEnumerator SwitchingAnimation(Spline targetRail)
    {
        switching = true;

        // 切り替えアニメーションの設定
        float t = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = targetRail.GetSampleAtDistance(0).location;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        // 次のレールへの切り替え完了
        currentRail = targetRail;
        railPosition = 0f;
        switching = false;
    }
}
