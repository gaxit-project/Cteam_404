using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdminModeManager : MonoBehaviour
{
    [Header("�Ǘ��҃��[�h�̃A�N�V����")]
    [SerializeField] private List<MonoBehaviour> adminActions = new List<MonoBehaviour>();

    [Header("UI �Ǘ�")]
    [SerializeField] private AdminModeUI adminModeUI;

    private KeyCode[] adminCode = {
        KeyCode.UpArrow, KeyCode.UpArrow,
        KeyCode.DownArrow, KeyCode.DownArrow,
        KeyCode.LeftArrow, KeyCode.RightArrow,
        KeyCode.LeftArrow, KeyCode.RightArrow,
        KeyCode.B, KeyCode.A
    };

    private List<KeyCode> inputHistory = new List<KeyCode>();
    public static bool isAdminMode { get; private set; } = false;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            CheckInput();
        }

        if (isAdminMode && Input.GetKeyDown(KeyCode.Alpha1))
        {
            ExecuteAdminActions();
        }
    }

    /// <summary>
    /// �L�[���͂��`�F�b�N���ĊǗ��҃��[�h��؂�ւ�
    /// </summary>
    private void CheckInput()
    {
        foreach (KeyCode key in adminCode)
        {
            if (Input.GetKeyDown(key))
            {
                inputHistory.Add(key);
                if (inputHistory.Count > adminCode.Length)
                {
                    inputHistory.RemoveAt(0);
                }
                break;
            }
        }

        if (inputHistory.Count == adminCode.Length)
        {
            bool isCorrect = true;
            for (int i = 0; i < adminCode.Length; i++)
            {
                if (inputHistory[i] != adminCode[i])
                {
                    isCorrect = false;
                    break;
                }
            }

            if (isCorrect)
            {
                ToggleAdminMode();
                inputHistory.Clear();
            }
        }
    }

    /// <summary>
    /// �Ǘ��҃��[�h�̐؂�ւ�
    /// </summary>
    private void ToggleAdminMode()
    {
        isAdminMode = !isAdminMode;
        Debug.Log("�Ǘ��҃��[�h: " + (isAdminMode ? "ON" : "OFF"));

        BossHealthManager bossHealth = FindObjectOfType<BossHealthManager>();
        if (bossHealth != null && !adminActions.Contains(bossHealth)) 
        {
            adminActions.Add(bossHealth);
        }

        if (bossHealth != null)
        {
            bossHealth.UpdateHealthUI();
        }

        if (adminModeUI != null)
        {
            adminModeUI.SetAdminModeActive(isAdminMode);
        }
    }

    /// <summary>
    /// �Ǘ��҃��[�h�Ŏ��s����A�N�V���������s
    /// </summary>
    private void ExecuteAdminActions()
    {
        foreach (MonoBehaviour action in adminActions)
        {
            if (action is IAdminAction adminAction)
            {
                adminAction.ExecuteAction();
            }
        }
    }
}
