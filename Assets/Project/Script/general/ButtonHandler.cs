using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    public GameObject Canvas;

    int BuildIndex;
    void Update()
    {
        BuildIndex = SceneManager.GetActiveScene().buildIndex;       
    }
    public void ButtonOnClicked()
    {
        SceneChangeManager.Instance.SceneChange(_sceneName);
    }


    /// <summary>
    /// MainScene��Canvas��ReturnButton�������ꂽ��Canvas���\���ɂ���
    /// </summary>

    public void ReturnOnClicked()
    {
        
        if (BuildIndex == 2)
        {
            Time.timeScale = 1.0f;
            if (Canvas != null && Canvas.activeSelf)
            {
                Canvas.SetActive(false);
                AudioManager.GetInstance().Update();
            }
        }
    }

    public void ApplicationEnd() // quit�{�^���������ƃQ�[�����I��
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // �Q�[���I��
        #else
            Application.Quit(); // �Q�[���I��
        #endif
    }
}
