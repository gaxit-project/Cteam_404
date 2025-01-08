using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    
    public static class SettingScene  // �ݒ���
    {
        public static bool isHome = true;
    }
    
    

    float seVolume;
    float bgmVolume;

    #region �V���O���h��
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
        if (SESlider != null && BGMSlider != null)
        {
            SESlider.onValueChanged.AddListener(delegate { OnSEVolumeChange(); });
            BGMSlider.onValueChanged.AddListener(delegate { OnBGMVolumeChange(); });
        }
        PlayBGM(0);
    }
    private void Update()
    {

        if (!SettingScene.isHome)
        {

            if (SESlider == null && BGMSlider == null)
            {
                SESlider = GameObject.Find("Canvas/SESlider").GetComponent<Slider>();
                BGMSlider = GameObject.Find("Canvas/BGNSlider").GetComponent<Slider>();
                SESlider.onValueChanged.AddListener(delegate { OnSEVolumeChange(); });
                BGMSlider.onValueChanged.AddListener(delegate { OnBGMVolumeChange(); });

            }
            SESlider.value = seVolume;
            BGMSlider.value = bgmVolume;
        }
    }


    #region SE�EBGM����

    // SE�̒l���ύX���ꂽ�Ƃ��̏���
    public void OnSEVolumeChange()
    {
        seVolume = SESlider.value;
        _audioSourceSE.volume = SESlider.value;
        PlayerPrefs.SetFloat("SEVolume", seVolume);
    }

    // BGM�̒l���ύX���ꂽ�Ƃ��̏���
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

    public void PlaySound(int index)  // SE�Đ�
    {
        _audioSourceSE.PlayOneShot(_seLists[index]);
    }

    public void PlayBGM(int index)  // BGM�Đ�
    {
        _audioSourceBGM.clip = _bgmLists[index];
        _audioSourceBGM.Play();
    }

    public void StopBGM()  // BGM��~
    {
        _audioSourceBGM.Stop();
    }

    ///<summary>
    ///�I�[�o�[���C�h
    ///</summary>

    public void PlaySound(int index, AudioSource _audioSource)  // SE�Đ�
    {
        _audioSourceSE.PlayOneShot(_seLists[index]);
    }

    public void PlayBGM(int index, AudioSource _audioSource)  // BGM�Đ�
    {
        _audioSourceBGM.clip = _bgmLists[index];
        _audioSourceBGM.Play();
    }

    public void StopBGM(AudioSource _audioSource)  // BGM��~
    {
        _audioSourceBGM.Stop();
    }
    #endregion
}