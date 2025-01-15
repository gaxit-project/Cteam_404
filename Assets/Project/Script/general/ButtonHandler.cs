using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    public void ButtonOnClicked()
    {
        SceneChangeManager.Instance.SceneChange(_sceneName);
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
