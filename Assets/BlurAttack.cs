using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;

public class BlurAttack : MonoBehaviour
{
    public PostProcessVolume volume;
    private DepthOfField dof;

    void Start()
    {
        if (volume == null)
        {
            Debug.LogError("PostProcessVolume が設定されていません");
            return;
        }

        if (!volume.profile.TryGetSettings(out dof))
        {
            Debug.LogError("DepthOfField 設定が見つかりません");
            return;
        }
    }

    public void ExecuteAttack()
    {
        if (dof == null)
        {
            Debug.LogWarning("DepthOfField が設定されていません");
            return;
        }

        Debug.Log("Boss は視界不良攻撃を発動した");
        StartCoroutine(ApplyBlurEffect());
    }

    private IEnumerator ApplyBlurEffect()
    {
        float duration = 3f; // ブラーの持続時間
        float blurAmount = 300f; // ブラーの強度
        float originalBlur = dof.focalLength.value;

        // 画面をぼかす
        dof.focalLength.value = blurAmount;

        yield return new WaitForSeconds(duration);

        // 元に戻す
        dof.focalLength.value = originalBlur;
    }
}
