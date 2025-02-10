using TMPro;
using UnityEngine;

public class AdminModeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI adminModeText;

    void Start()
    {
        if (adminModeText != null)
        {
            adminModeText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ŠÇ—Òƒ‚[ƒh‚ÌON/OFF‚ğØ‚è‘Ö‚¦‚é
    /// </summary>
    public void SetAdminModeActive(bool isActive)
    {
        if (adminModeText != null)
        {
            adminModeText.gameObject.SetActive(isActive);
        }
    }
}
