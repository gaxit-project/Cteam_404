using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    
    public GameObject Canvas;

    private int BuildIndex;


    #region �V���O���g��

    public static PauseManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = FindObjectOfType<PauseManager>();
        }
        return Instance;
    }
    private void Awake()
    {
        BuildIndex = SceneManager.GetActiveScene().buildIndex;
        
        if (this != GetInstance())
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);

    }
    #endregion

    #region Scene���ړ������Ƃ��̏���
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BuildIndex = scene.buildIndex; // �r���h�ԍ����擾
        
        if (BuildIndex == 2)
        {
            if (Canvas == null) // Canvas��null�Ȃ�
            {
                Canvas = GameObject.Find("Canvas");  //Canvas���擾
                Canvas.SetActive(false);  //�擾���Ă����A�N�e�B�u�ɕύX
            }
        }
    }

    #endregion


    /// <summary>
    /// esc�L�[�������ꂽ��Canvas�����̉�
    /// </summary>
    private void Update()
    {
        if (BuildIndex == 2) //MainScene���Ȃ�
        {
            if (Canvas == null)
            {
                Canvas = GameObject.Find("Canvas");
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 0f;
                if (Canvas != null)
                {
                    Canvas.SetActive(true);
                    AudioManager.GetInstance().InitializeSliders();
                }
                else
                {
                    Debug.Log("Error!!");
                }
            }
        }
    } 

    /// <summary>
    /// Canvas�����������Ƃ��̃��\�b�h
    /// AudioManager�Ŏg�p
    /// </summary>
    /// <returns></returns>
    public GameObject GetCanvas()
    {
        return Canvas;
    }
}
