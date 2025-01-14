using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    public void ButtonOnClicked()
    {
        SceneChangeManager.Instance.SceneChange(_sceneName);

    }
}
