using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager instance; // 정적 메모리에 담기 위한 instance 변수 선언

    // BGM
    [Header("#BGM")]
    public AudioClip[] bgmClips; // BGM 관련 클립(파일) 배열
    public AudioSource bgmPlayer; // BGM 관련 오디오소스

    // SFX(효과음)
    [Header("#SFX")]
    public AudioClip[] sfxClips; // SFX 관련 클립 배열
    // 오디오 채널 시스템
    public int channels; // 다량의 효과음을 내기 위해 채널 개수 배열 선언
    int channelIndex; // 채널 인덱스
    AudioSource[] sfxPlayers = new AudioSource[10]; // SFX 관련 오디오소스

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

    public enum Sfx { Dead, Hit, LevelUp = 3, Lose, Melee, Range = 7, Select, Win }
    public enum Bgm { Stage1, Stage2, Boss1, Boss2, Clear } // 필요 Bgm Clip들

    private void Awake()
    {
        instance = this;

        string OutputMixer = "Master";

        m_MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        m_MusicBGMSlider.onValueChanged.AddListener(SetMusicVolume);
        m_MusicSFXSlider.onValueChanged.AddListener(SetSFXVolume);

        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform; // 배경음을 담당하는 자식 오브젝트 생성
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.outputAudioMixerGroup = m_AudioMixer.FindMatchingGroups(OutputMixer)[1];
        bgmPlayer.playOnAwake = false;

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform; // 효과음을 담당하는 자식 오브젝트 생성
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();

            sfxPlayers[index].outputAudioMixerGroup = m_AudioMixer.FindMatchingGroups(OutputMixer)[2];
            sfxPlayers[index].playOnAwake = false;
        }
    }

    public void PlaySfx(Sfx sfx)
    {

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;
            
            channelIndex = loopIndex;

            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();

            break;
        }
    }

    public void SetMasterVolume(float volume)
    {
        m_AudioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        masterVolume = volume;
        masterSoundLabel.text = (volume * 100).ToString("F0");
    }

    public void SetMusicVolume(float volume)
    {
        m_AudioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        bgmVolume = volume;
        BGMSoundLabel.text = (volume * 100).ToString("F0");
    }

    public void SetSFXVolume(float volume)
    {
        m_AudioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        sfxVolume = volume;
        SFXSoundLabel.text = (volume * 100).ToString("F0");
    }

    public void ToggleAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }
}