using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class LobbyAudioManager : MonoBehaviour
{
    public static LobbyAudioManager instance;

    // Sound Data 오브젝트
    public SoundData soundData;

    [SerializeField] private AudioMixer m_AudioMixer;
    [SerializeField] private Slider m_MusicMasterSlider;
    [SerializeField] private Slider m_MusicBGMSlider;
    [SerializeField] private Slider m_MusicSFXSlider;

    [SerializeField] private TextMeshProUGUI masterSoundLabel;
    [SerializeField] private TextMeshProUGUI BGMSoundLabel;
    [SerializeField] private TextMeshProUGUI SFXSoundLabel;

    public Toggle soundMuteToggle;
    public Image masterFillImage;
    public Image sfxFillImage;
    public Image bgmFillImage;

    public float masterSound;
    public float bgmSound;
    public float sfxSound;
    public bool isSoundMute;

    private void Awake()
    {
        SoundInit();

        masterSound = soundData.masterSound;
        bgmSound = soundData.bgmSound;
        sfxSound = soundData.sfxSound;
        isSoundMute = soundData.isMute;

    
        m_MusicMasterSlider.onValueChanged.AddListener(delegate { SetMasterVolume(m_MusicMasterSlider.value); });
        m_MusicBGMSlider.onValueChanged.AddListener(delegate { SetBGMVolume(m_MusicBGMSlider.value); });
        m_MusicSFXSlider.onValueChanged.AddListener(delegate { SetSFXVolume(m_MusicSFXSlider.value); });
        soundMuteToggle.onValueChanged.AddListener(delegate { ToggleAudioVolume(soundMuteToggle.isOn); });
    }

    private void Update()
    {

        soundData.masterSound = masterSound;
        soundData.bgmSound = bgmSound;
        soundData.sfxSound = sfxSound;
        soundData.isMute = isSoundMute;
    }

    public void SetMasterVolume(float volume)
    {
        masterSound = volume;
        m_MusicMasterSlider.value = masterSound;

        Debug.Log(masterSound);
        if (masterSound == 0.001f)
            m_AudioMixer.SetFloat("Master", -80);
        else
            m_AudioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);

        masterFillImage.fillAmount = m_MusicMasterSlider.value;
        masterSoundLabel.text = (volume * 100).ToString("F0");
    }

    public void SetBGMVolume(float volume)
    {
        bgmSound = volume;
        m_MusicBGMSlider.value = bgmSound;

        Debug.Log(bgmSound);
        if (bgmSound == 0.001f)
            m_AudioMixer.SetFloat("BGM", -80);
        else
            m_AudioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        bgmFillImage.fillAmount = m_MusicBGMSlider.value;
        BGMSoundLabel.text = (volume * 100).ToString("F0");
    }

    public void SetSFXVolume(float volume)
    {
        sfxSound = volume;
        m_MusicSFXSlider.value = sfxSound;

        Debug.Log(sfxSound);
        if (sfxSound == 0.001f)
            m_AudioMixer.SetFloat("SFX", -80);
        else
            m_AudioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        sfxFillImage.fillAmount = m_MusicSFXSlider.value;
        SFXSoundLabel.text = (volume * 100).ToString("F0");
    }

    public void SetMuteSetting(bool isOn)
    {
        if (isSoundMute)
        {
            if (AudioListener.volume == 0)
            {
                soundMuteToggle.isOn = true;
            }
        }
        else
        {
            if (AudioListener.volume == 1)
            {
                soundMuteToggle.isOn = false;
            }
        }
    }

    public void ToggleAudioVolume(bool isOn)
    {
        // AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
        isSoundMute = isOn;

        if (!isSoundMute)
        {
            soundData.isMute = false;
            AudioListener.volume = 1; // 켜기
            Debug.Log("soundData.isMute: " + soundData.isMute);
            Debug.Log("Toggle.isOn: " + soundMuteToggle.isOn);
        }
        else // 음소거 
        {
            soundData.isMute = true;
            AudioListener.volume = 0; // 끄기
            Debug.Log("soundData.isMute: " + soundData.isMute);
            Debug.Log("Toggle.isOn: " + soundMuteToggle.isOn);
        }
    }

    void SoundInit()
    {
        soundData.masterSound = 1f;
        soundData.bgmSound = 1f;
        soundData.sfxSound = 1f;

        soundData.isMute = false;
    }
}