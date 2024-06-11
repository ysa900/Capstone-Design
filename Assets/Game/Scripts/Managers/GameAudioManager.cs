using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager instance; // 정적 메모리에 담기 위한 instance 변수 선언

    /*// 인스턴스에 접근하기 위한 프로퍼티
    public static GameAudioManager instance
    {
        get
        {
            // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
            if (!_instance)
            {
                _instance = FindAnyObjectByType(typeof(GameAudioManager)) as GameAudioManager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }*/

    public SoundData soundData; // Sound Data 객체

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
    [SerializeField] static private Slider m_MusicMasterSlider;
    [SerializeField] static private Slider m_MusicBGMSlider;
    [SerializeField] static private Slider m_MusicSFXSlider;

    [SerializeField] static private TextMeshProUGUI masterSoundLabel;
    [SerializeField] static private TextMeshProUGUI BGMSoundLabel;
    [SerializeField] static private TextMeshProUGUI SFXSoundLabel;

    static Image masterFillImage;
    static Image sfxFillImage;
    static Image bgmFillImage;

    static Toggle soundMuteToggle;

    private bool isHitPlaying;

    public enum Sfx { Dead, Hit, LevelUp = 3, Lose, Melee, Range = 7, Select, Win }
    public enum Bgm { Stage1, Stage2, Stage3, Boss, Clear } // 필요 Bgm Clip들

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        /*if (_instance == null)
        {
            _instance = this;
        }

        // 인스턴스가 존재하는 경우 새로생기는 인스턴스를 삭제한다.
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        // 아래의 함수를 사용하여 씬이 전환되더라도 선언되었던 인스턴스가 파괴되지 않는다.
        DontDestroyOnLoad(gameObject);*/


        soundMuteToggle = GameObject.Find("Canvas").transform.Find("Setting Page/Sound Setting/Master Sound/Sound Mute Toggle").GetComponent<Toggle>();
        m_MusicMasterSlider = GameObject.Find("Canvas").transform.Find("Setting Page/Sound Setting/Master Sound/Master Sound Slider").GetComponent<Slider>();
        m_MusicBGMSlider = GameObject.Find("Canvas").transform.Find("Setting Page/Sound Setting/BGM Sound/BGM Slider").GetComponent<Slider>();
        m_MusicSFXSlider = GameObject.Find("Canvas").transform.Find("Setting Page/Sound Setting/SFX Sound/SFX Slider").GetComponent<Slider>();

        masterSoundLabel = GameObject.Find("Canvas").transform.Find("Setting Page/Sound Setting/Master Sound/Master Sound Slider/MasterSound Label").GetComponent<TextMeshProUGUI>();
        BGMSoundLabel = GameObject.Find("Canvas").transform.Find("Setting Page/Sound Setting/BGM Sound/BGM Slider/BGMSound Label").GetComponent<TextMeshProUGUI>();
        SFXSoundLabel = GameObject.Find("Canvas").transform.Find("Setting Page/Sound Setting/SFX Sound/SFX Slider/SFXSound Label").GetComponent<TextMeshProUGUI>();

        masterFillImage = GameObject.Find("Canvas").transform.Find("Setting Page/Sound Setting/Master Sound/Master Sound Slider/Fill Area/Fill").GetComponent<Image>();
        bgmFillImage = GameObject.Find("Canvas").transform.Find("Setting Page/Sound Setting/BGM Sound/BGM Slider/Fill Area/Fill").GetComponent<Image>();
        sfxFillImage = GameObject.Find("Canvas").transform.Find("Setting Page/Sound Setting/SFX Sound/SFX Slider/Fill Area/Fill").GetComponent<Image>();

        Debug.Log("넘어온 masterSound: " + soundData.masterSound);
        Debug.Log("넘어온 bgmSound: " + soundData.bgmSound);
        Debug.Log("넘어온 sfxSound: " + soundData.sfxSound);
        Debug.Log("받아온 isSoundMute: " + soundData.isMute);

        isHitPlaying = false;

        string OutputMixer = "Master";

        m_MusicMasterSlider.onValueChanged.AddListener(delegate { SetMasterVolume(m_MusicMasterSlider.value); });
        m_MusicBGMSlider.onValueChanged.AddListener(delegate { SetBGMVolume(m_MusicBGMSlider.value); });
        m_MusicSFXSlider.onValueChanged.AddListener(delegate { SetSFXVolume(m_MusicSFXSlider.value); });
        soundMuteToggle.onValueChanged.AddListener(delegate { ToggleAudioVolume(soundMuteToggle.isOn); });
        m_MusicMasterSlider.interactable = true;
        m_MusicBGMSlider.interactable = true;
        m_MusicSFXSlider.interactable = true;

        SetMasterVolume(soundData.masterSound);
        SetBGMVolume(soundData.bgmSound);
        SetSFXVolume(soundData.sfxSound);
        SetMuteSetting(soundData.isMute);

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

            // 이미 재생하고 있는 효과음이 있는가
            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            int randIndex = 0;
            if (sfx == Sfx.Hit || sfx == Sfx.Melee)
            {
                randIndex = Random.Range(0, 2);
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx + randIndex];
            sfxPlayers[loopIndex].Play();

            break;
        }
    }

    public void SetMasterVolume(float volume)
    {
        soundData.masterSound = volume;
        m_MusicMasterSlider.value = soundData.masterSound;

        Debug.Log(soundData.masterSound);
        if (soundData.masterSound == 0.001f)
            m_AudioMixer.SetFloat("Master", -80);
        else
            m_AudioMixer.SetFloat("Master", Mathf.Log10(volume) * 20 - 15);

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
            m_AudioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20 - 15);
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
            //soundData.isMute = false;
            AudioListener.volume = 1; // 켜기
            Debug.Log("soundData.isMute: " + soundData.isMute);
            Debug.Log("Toggle.isOn: " + soundMuteToggle.isOn);
        }
        else // 음소거 
        {
            //soundData.isMute = true;
            AudioListener.volume = 0; // 끄기
            Debug.Log("soundData.isMute: " + soundData.isMute);
            Debug.Log("Toggle.isOn: " + soundMuteToggle.isOn);
        }
    }
}