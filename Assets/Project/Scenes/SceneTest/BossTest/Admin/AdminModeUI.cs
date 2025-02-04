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
    /// �Ǘ��҃��[�h��ON/OFF��؂�ւ���
    /// </summary>
    public void SetAdminModeActive(bool isActive)
    {
        if (adminModeText != null)
        {
            adminModeText.gameObject.SetActive(isActive);
        }
    }
}
