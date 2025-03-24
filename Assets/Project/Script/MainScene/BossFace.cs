using UnityEngine;
using UnityEngine.UI;

public class BossFace : MonoBehaviour
{
    public static BossFace Instance { get; private set; }

    [Header("ボスの顔画像")]
    [SerializeField]
    private Texture2D[] _faceTextures; // 顔画像の配列

    [SerializeField]
    private Material _ledMaterial; // LEDシェーダーを適用するマテリアル

    #region インスタンス初期化
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 重複するインスタンスを破棄
            return;
        }
        Instance = this;
    }
    #endregion

    void Start()
    {
        ChangeFace(0);
    }
    public void ChangeFace(int index)
    {
        _ledMaterial.SetTexture("_MainTex", _faceTextures[index]);
    }
}
