using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    public void ButtonOnClicked()
    {
        SceneChangeManager.Instance.SceneChange(_sceneName);
    }

    public void ApplicationEnd() // quitボタンを押すとゲームを終了
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // ゲーム終了
        #else
            Application.Quit(); // ゲーム終了
        #endif
    }
}
