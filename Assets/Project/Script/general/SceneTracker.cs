using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTracker : MonoBehaviour
{
    private const string CurrentSceneKey = "CurrentSceneBuildIndex";
    public void  SceneTrack()
    {
        int SceneIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("CurrentSceneKey", SceneIndex);
        PlayerPrefs.Save();
        
    }


}
