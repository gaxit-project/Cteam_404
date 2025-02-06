using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;

    // ���t�@�C��
    [SerializeField] AudioClip[] _seLists;
    [SerializeField] AudioClip[] _bgmLists;

    //���̖炵���̎w��
    [Header("SE��AudioSource")]
    [SerializeField] AudioSource _audioSourceSE;
    [Header("BGM��AudioSource")]
    [SerializeField] AudioSource _audioSourceBGM;

    [Header("�X���C�_�[")]
    public Slider SESlider;
    public Slider BGMSlider;

    private float seVolume = 0.5f;
    private float bgmVolume = 0.5f; 

    

    #region �V���O���g��
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
    /// �X���C�_�[������
    /// </summary>
    
    private void InitializeSliders()
    {
        SESlider = GameObject.Find("Canvas/seSlider").GetComponent<Slider>();
        BGMSlider = GameObject.Find("Canvas/bgmSlider").GetComponent<Slider>();

        // �X���C�_�[�����l�𔽉f
        SESlider.value = seVolume;
        BGMSlider.value = bgmVolume;

        //�@�C�x���g���X�i�[��o�^
        SESlider.onValueChanged.RemoveAllListeners();
        SESlider.onValueChanged.AddListener(delegate { OnSEVolumeChange(); });

        BGMSlider.onValueChanged.RemoveAllListeners();
        BGMSlider.onValueChanged.AddListener(delegate { OnBGMVolumeChange(); });
    }
    #region �V�[�����ړ�����Setting�V�[���߂��Ă����ۂ̏���
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
        if(scene.name == "Setting") // Setting�V�[���ɂ���Ƃ��������������s
        {
            InitializeSliders();
        }
    }
    #endregion

    #region SE�EBGM����

    /// <summary>
    /// SE�̒l���ύX���ꂽ�Ƃ��̏���
    /// </summary>
    public void OnSEVolumeChange()
    {
        seVolume = SESlider.value;
        _audioSourceSE.volume = SESlider.value;
        PlayerPrefs.SetFloat("SEVolume", seVolume);
    }

    /// <summary>
    /// BGM�̒l���ύX���ꂽ�Ƃ��̏���
    /// </summary>
    public void OnBGMVolumeChange()
    {
        bgmVolume = BGMSlider.value;
        _audioSourceBGM.volume = BGMSlider.value; // AudioSource�ɓK�p
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume); // �ۑ�
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
    ///SE�Đ�
    /// </summary>
    /// <param name="index"></param>
    public void PlaySound(int index)
    {
        _audioSourceSE.clip = _seLists[index];
        _audioSourceSE.PlayOneShot(_seLists[index]);
    }
    /// <summary>
    /// BGM�Đ�
    /// </summary>
    /// <param name="index"></param>
    public void PlayBGM(int index)
    {
        _audioSourceBGM.clip = _bgmLists[index];
        _audioSourceBGM.Play();
    }
    /// <summary>
    /// BGM��~
    /// </summary>
    public void StopBGM()  
    {
        _audioSourceBGM.Stop();
    }

    ///<summary>
    ///�I�[�o�[���C�h
    ///SE�Đ�
    ///</summary>
    public void PlaySound(int index, AudioSource _audioSource)  
    {
        _audioSourceSE.clip = _seLists[index];
        _audioSourceSE.PlayOneShot(_seLists[index]);
    }
    /// <summary>
    /// �I�[�o�[���C�h
    /// BGM�Đ�
    /// </summary>
    /// <param name="index"></param>
    /// <param name="_audioSource"></param>
    public void PlayBGM(int index, AudioSource _audioSource)
    {
        _audioSourceBGM.clip = _bgmLists[index];
        _audioSourceBGM.Play();
    }

    /// <summary>
    /// �I�[�o�[���C�h
    /// BGM��~
    /// </summary>
    /// <param name="_audioSource"></param>

    public void StopBGM(AudioSource _audioSource)
    {
        _audioSourceBGM.Stop();
    }
    #endregion

    #region BGM�ESE���Q�[���I�����ɕۑ��E���[�h����
    
    ///<sumaary>
    ///���ʂ�ۑ�����
    /// </sumaary>

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("SEVolume", seVolume);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.Save();
    }

    ///<summary>
    ///���ʐݒ�����[�h����
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
    ///�Q�[���I�����ɉ��ʕۑ�
    /// </summary>

    private void OnApplicationQuit()
    {
        SaveVolumeSettings();
    }
    #endregion

}