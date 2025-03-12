using System.Collections.Generic;
using UnityEngine;

public class Ending : MonoBehaviour
{
    public static Ending Instance { get; private set; }
    public List<Renderer> targetRenderers = new List<Renderer>(); // 対象オブジェクトのRendererをセット
    public Texture2D dissolveTexture; // ディゾルブ用のノイズテクスチャ
    public Color edgeColor = Color.white;
    public float edgeWidth = 0.05f;

    [Range(0, 1)] public float dissolveAmount = 0.0f;
    public float dissolveSpeed = 1.0f;
    private bool isDissolving = false;

    private Dictionary<Renderer, MaterialPropertyBlock> propBlocks = new Dictionary<Renderer, MaterialPropertyBlock>();

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
        foreach (Renderer rend in targetRenderers)
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            rend.GetPropertyBlock(propBlock);
            propBlocks[rend] = propBlock;
        }
        isDissolving = false;
    }

    void Update()
    {
        if (isDissolving)
        {
            dissolveAmount += Time.deltaTime * dissolveSpeed;
            dissolveAmount = Mathf.Clamp01(dissolveAmount);
            ApplyDissolveEffect();

            if (dissolveAmount >= 1.0f)
            {
                foreach (Renderer rend in targetRenderers)
                {
                    rend.enabled = false; // 透明になったら非表示
                }
            }
        }
    }

    public void StartDissolve()
    {
        isDissolving = true;
    }

    private void ApplyDissolveEffect()
    {
        foreach (var kvp in propBlocks)
        {
            Renderer rend = kvp.Key;
            MaterialPropertyBlock propBlock = kvp.Value;

            propBlock.SetFloat("_DissolveAmount", dissolveAmount);
            propBlock.SetColor("_EdgeColor", edgeColor);
            propBlock.SetFloat("_EdgeWidth", edgeWidth);
            if (dissolveTexture != null)
                propBlock.SetTexture("_DissolveTex", dissolveTexture);

            rend.SetPropertyBlock(propBlock);
        }
    }
}
