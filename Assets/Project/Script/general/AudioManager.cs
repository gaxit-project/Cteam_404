using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;

    // 音ファイル
    [SerializeField] AudioClip[] _seLists;
    [SerializeField] AudioClip[] _bgmLists;

    //音の鳴らし方の指定
    [Header("SEのAudioSource")]
    [SerializeField] AudioSource _audioSourceSE;
    [Header("BGMのAudioSource")]
    [SerializeField] AudioSource _audioSourceBGM;

    [Header("スライダー")]
    public Slider SESlider;
    public Slider BGMSlider;

    private float seVolume = 0.5f;
    private float bgmVolume = 0.5f; 

    

    #region シングルトン
    public static AudioManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = FindObjectOfType<AudioManager>();
        }
        return Instance;
    }
    private void Awake()
    {
        if (this != GetInstance())
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion
    void Start()
    {
        LoadVolumeSetting();

        if (SESlider != null && BGMSlider != null)
        {
            InitializeSliders();
        }
        if(_audioSourceSE == null)
        {
            _audioSourceSE = gameObject.AddComponent<AudioSource>();
        }
        if (_audioSourceBGM == null)
        {
            _audioSourceBGM = gameObject.AddComponent<AudioSource>();
        }
        PlayBGM(0);
        PlaySound(0);
    }
    private void Update()
    {
        int BuildIndex = PlayerPrefs.GetInt("CurrentSceneKey");
        if (BuildIndex == 1)
        {
            if (SESlider == null && BGMSlider == null)
            {

                SESlider.onValueChanged.AddListener(delegate { OnSEVolumeChange(); });
                BGMSlider.onValueChanged.AddListener(delegate { OnBGMVolumeChange(); });

            }
        }
    }

    /// <summary>
    /// スライダー初期化
    /// </summary>
    
    private void InitializeSliders()
    {
        SESlider = GameObject.Find("Canvas/seSlider").GetComponent<Slider>();
        BGMSlider = GameObject.Find("Canvas/bgmSlider").GetComponent<Slider>();

        // スライダー初期値を反映
        SESlider.value = seVolume;
        BGMSlider.value = bgmVolume;

        //　イベントリスナーを登録
        SESlider.onValueChanged.RemoveAllListeners();
        SESlider.onValueChanged.AddListener(delegate { OnSEVolumeChange(); });

        BGMSlider.onValueChanged.RemoveAllListeners();
        BGMSlider.onValueChanged.AddListener(delegate { OnBGMVolumeChange(); });
    }
    #region シーンを移動してSettingシーン戻ってきた際の処理
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Setting") // Settingシーンにいるときだけ処理を実行
        {
            InitializeSliders();
        }
    }
    #endregion

    #region SE・BGM操作

    /// <summary>
    /// SEの値が変更されたときの処理
    /// </summary>
    public void OnSEVolumeChange()
    {
        seVolume = SESlider.value;
        _audioSourceSE.volume = SESlider.value;
        PlayerPrefs.SetFloat("SEVolume", seVolume);
    }

    /// <summary>
    /// BGMの値が変更されたときの処理
    /// </summary>
    public void OnBGMVolumeChange()
    {
        bgmVolume = BGMSlider.value;
        _audioSourceBGM.volume = BGMSlider.value; // AudioSourceに適用
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume); // 保存
        PlayerPrefs.Save();
    }

    public float BGMVolume
    {
        get { return _audioSourceBGM.volume; }
        set { _audioSourceBGM.volume = value; }
    }

    public float SEVolume
    {
        get { return _audioSourceSE.volume; }
        set { _audioSourceSE.volume = value; }
    }
    /// <summary>
    ///SE再生
    /// </summary>
    /// <param name="index"></param>
    public void PlaySound(int index)
    {
        _audioSourceSE.clip = _seLists[index];
        _audioSourceSE.PlayOneShot(_seLists[index]);
    }
    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="index"></param>
    public void PlayBGM(int index)
    {
        _audioSourceBGM.clip = _bgmLists[index];
        _audioSourceBGM.Play();
    }
    /// <summary>
    /// BGM停止
    /// </summary>
    public void StopBGM()  
    {
        _audioSourceBGM.Stop();
    }

    ///<summary>
    ///オーバーライド
    ///SE再生
    ///</summary>
    public void PlaySound(int index, AudioSource _audioSource)  
    {
        _audioSourceSE.clip = _seLists[index];
        _audioSourceSE.PlayOneShot(_seLists[index]);
    }
    /// <summary>
    /// オーバーライド
    /// BGM再生
    /// </summary>
    /// <param name="index"></param>
    /// <param name="_audioSource"></param>
    public void PlayBGM(int index, AudioSource _audioSource)
    {
        _audioSourceBGM.clip = _bgmLists[index];
        _audioSourceBGM.Play();
    }

    /// <summary>
    /// オーバーライド
    /// BGM停止
    /// </summary>
    /// <param name="_audioSource"></param>

    public void StopBGM(AudioSource _audioSource)
    {
        _audioSourceBGM.Stop();
    }
    #endregion

    #region BGM・SEをゲーム終了時に保存・ロードする
    
    ///<sumaary>
    ///音量を保存する
    /// </sumaary>

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("SEVolume", seVolume);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.Save();
    }

    ///<summary>
    ///音量設定をロードする
    /// </summary>
    
    private void LoadVolumeSetting()
    {
        if(PlayerPrefs.HasKey("SEVolume") && PlayerPrefs.HasKey("BGMVolume"))
        {
            seVolume = PlayerPrefs.GetFloat("SEVolume");
            bgmVolume = PlayerPrefs.GetFloat("BGMVolume");
        }

        _audioSourceSE.volume  = seVolume;
        _audioSourceBGM.volume = bgmVolume;
    }

    ///<summary>
    ///ゲーム終了時に音量保存
    /// </summary>

    private void OnApplicationQuit()
    {
        SaveVolumeSettings();
    }
    #endregion

}