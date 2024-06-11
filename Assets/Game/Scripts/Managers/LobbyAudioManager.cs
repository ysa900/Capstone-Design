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

    public int count;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (!soundData.isFirstLobby)
            SoundInitialInit();

        SetMasterVolume(soundData.masterSound);
        SetBGMVolume(soundData.bgmSound);
        SetSFXVolume(soundData.sfxSound);
        SetMuteSetting(soundData.isMute);

        m_MusicMasterSlider.onValueChanged.AddListener(delegate { SetMasterVolume(m_MusicMasterSlider.value); });
        m_MusicBGMSlider.onValueChanged.AddListener(delegate { SetBGMVolume(m_MusicBGMSlider.value); });
        m_MusicSFXSlider.onValueChanged.AddListener(delegate { SetSFXVolume(m_MusicSFXSlider.value); });
        soundMuteToggle.onValueChanged.AddListener(delegate { ToggleAudioVolume(soundMuteToggle.isOn); });
    }

    public void SetMasterVolume(float volume)
    {
        soundData.masterSound = volume;
        m_MusicMasterSlider.value = soundData.masterSound;

        Debug.Log(soundData.masterSound);
        if (soundData.masterSound == 0.001f)
            m_AudioMixer.SetFloat("Master", -80);
        else
            m_AudioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);

        masterFillImage.fillAmount = m_MusicMasterSlider.value;
        masterSoundLabel.text = (volume * 100).ToString("F0");
    }

    public void SetBGMVolume(float volume)
    {
        soundData.bgmSound = volume;
        m_MusicBGMSlider.value = soundData.bgmSound;

        Debug.Log(soundData.bgmSound);
        if (soundData.bgmSound == 0.001f)
            m_AudioMixer.SetFloat("BGM", -80);
        else
            m_AudioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        bgmFillImage.fillAmount = m_MusicBGMSlider.value;
        BGMSoundLabel.text = (volume * 100).ToString("F0");
    }

    public void SetSFXVolume(float volume)
    {
        soundData.sfxSound = volume;
        m_MusicSFXSlider.value = soundData.sfxSound;

        Debug.Log(soundData.sfxSound);
        if (soundData.sfxSound == 0.001f)
            m_AudioMixer.SetFloat("SFX", -80);
        else
            m_AudioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20 - 15);
        sfxFillImage.fillAmount = m_MusicSFXSlider.value;
        SFXSoundLabel.text = (volume * 100).ToString("F0");
    }

    public void SetMuteSetting(bool isOn)
    {
        if (soundData.isMute)
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
        soundData.isMute = isOn;

        if (!soundData.isMute)
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

    void SoundInitialInit()
    {
        soundData.masterSound = 1f;
        soundData.bgmSound = 1f;
        soundData.sfxSound = 1f;
        soundData.isMute = false;
    }
}