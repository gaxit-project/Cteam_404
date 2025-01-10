using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class DamageVignetteEffect : MonoBehaviour
{
    public Material vignetteMaterial;
    [Range(-5f, 5f)]
    public float vignettePower = 0f;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (vignetteMaterial != null)
        {
            vignetteMaterial.SetFloat("_Power", vignettePower);
            Graphics.Blit(src, dest, vignetteMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
