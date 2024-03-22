using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class LobbyAudioManager : MonoBehaviour
{
    private LobbyManager lobbyManager;

    [SerializeField] private AudioMixer m_AudioMixer;
    [SerializeField] private Slider m_MusicMasterSlider;
    [SerializeField] private Slider m_MusicBGMSlider;
    [SerializeField] private Slider m_MusicSFXSlider;

    [SerializeField] private TextMeshProUGUI masterSoundLabel;
    [SerializeField] private TextMeshProUGUI BGMSoundLabel;
    [SerializeField] private TextMeshProUGUI SFXSoundLabel;


    private float masterVolume;
    private float bgmVolume;
    private float sfxVolume;

    private float prevMasterVolume; // 이전 Master 볼륨 값
    private float prevBgmVolume; // 이전 BGM 볼륨 값
    private float prevSfxVolume; // 이전 SFX 볼륨 값

    private void Awake()
    {
        lobbyManager = FindAnyObjectByType<LobbyManager>();

        m_MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        m_MusicBGMSlider.onValueChanged.AddListener(SetMusicVolume);
        m_MusicSFXSlider.onValueChanged.AddListener(SetSFXVolume);

        lobbyManager.soundSaveButtonObject.onClick.AddListener(SaveVolume); // Save 버튼에 클릭 리스너 추가
        lobbyManager.SettingPageBackButtonObject.onClick.AddListener(RestoreVolume); // Back 버튼에 클릭 리스너 추가

        // 이전에 저장한 볼륨 값 불러오기
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);

        prevMasterVolume = masterVolume; // 초기화
        prevBgmVolume = bgmVolume; // 초기화
        prevSfxVolume = sfxVolume; // 초기화

        // 슬라이더에 이전 볼륨 값 설정
        m_MusicMasterSlider.value = masterVolume;
        m_MusicBGMSlider.value = bgmVolume;
        m_MusicSFXSlider.value = sfxVolume;

        // 레이블에 이전 볼륨 값 표시
        masterSoundLabel.text = (masterVolume * 100).ToString("F0");
        BGMSoundLabel.text = (bgmVolume * 100).ToString("F0");
        SFXSoundLabel.text = (sfxVolume * 100).ToString("F0");
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        masterSoundLabel.text = (volume * 100).ToString("F0");
    }

    public void SetMusicVolume(float volume)
    {
        bgmVolume = volume;
        BGMSoundLabel.text = (volume * 100).ToString("F0");
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        SFXSoundLabel.text = (volume * 100).ToString("F0");
    }

    public void SaveVolume()
    {
        // 수정한 볼륨 값 저장
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();

        // 이전 값 업데이트
        prevMasterVolume = masterVolume;
        prevBgmVolume = bgmVolume;
        prevSfxVolume = sfxVolume;
    }

    public void RestoreVolume()
    {
        // 이전 값으로 되돌리기
        m_MusicMasterSlider.value = prevMasterVolume;
        m_MusicBGMSlider.value = prevBgmVolume;
        m_MusicSFXSlider.value = prevSfxVolume;

        // 이전 값으로 볼륨 조정
        SetMasterVolume(prevMasterVolume);
        SetMusicVolume(prevBgmVolume);
        SetSFXVolume(prevSfxVolume);
    }
}
