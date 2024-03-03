using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class LobbyAudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer m_AudioMixer;
    [SerializeField] private Slider m_MusicMasterSlider;
    [SerializeField] private Slider m_MusicBGMSlider;
    [SerializeField] private Slider m_MusicSFXSlider;

    //// BGM
    //[Header("#BGM")]
    //public AudioClip bgmClip; // BGM ���� Ŭ��
    //public float bgmVolume; // BGM ���� ����
    //AudioSource bgmPlayer; // BGM ���� ������ҽ�
    //AudioHighPassFilter bgmEffect;

    //// SFX(ȿ����)
    //[Header("#SFX")]
    //public AudioClip[] sfxClips; // SFX ���� Ŭ�� �迭
    //public float SfxVolume; // SFX ���� ����

    //// ����� ä�� �ý���
    //public int channels; // �ٷ��� ȿ������ ���� ���� ä�� ���� �迭 ����
    //int channelIndex; // ä�� �ε���
    //AudioSource[] sfxPlayers; // BGM ���� ������ҽ�

    //public enum Sfx { Select }

    private void Awake()
    {
        m_MusicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        m_MusicBGMSlider.onValueChanged.AddListener(SetMusicVolume);
        m_MusicSFXSlider.onValueChanged.AddListener(SetSFXVolume);

        //Init();
    }

    //private void Init()
    //{
    //    // ����� �÷��̾� �ʱ�ȭ
    //    GameObject bgmObject = new GameObject("BgmPlayer");
    //    bgmObject.transform.parent = transform; // ������� ����ϴ� �ڽ� ������Ʈ ����
    //    bgmPlayer = bgmObject.AddComponent<AudioSource>();
    //    bgmPlayer.playOnAwake = true; // Game ���۵��ڸ��� �ѱ�
    //    bgmPlayer.loop = true; // BGM ���ѹݺ�
    //    bgmPlayer.volume = bgmVolume;
    //    bgmPlayer.clip = bgmClip;

    //    // ȿ���� �÷��̾� �ʱ�ȭ
    //    GameObject sfxObject = new GameObject("SfxPlayer");
    //    sfxObject.transform.parent = transform; // ȿ������ ����ϴ� �ڽ� ������Ʈ ����
    //    sfxPlayers = new AudioSource[channels];

    //    for (int index = 0; index < sfxPlayers.Length; index++)
    //    {
    //        sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
    //        sfxPlayers[index].playOnAwake = false;
    //        sfxPlayers[index].bypassListenerEffects = true;
    //        sfxPlayers[index].volume = SfxVolume;
    //    }
    //}

    //public void PlayBGM(bool isPlay)
    //{
    //    if (isPlay)
    //    {
    //        bgmPlayer.Play();
    //    }
    //    else
    //    {
    //        bgmPlayer.Stop();
    //    }
    //}

    //public void PlaySfx(Sfx sfx)
    //{
    //    for (int index = 0; index < sfxPlayers.Length; index++)
    //    {
    //        int loopIndex = (index + channelIndex) % sfxPlayers.Length;

    //        if (sfxPlayers[loopIndex].isPlaying)
    //        {
    //            continue;
    //        }

    //        channelIndex = loopIndex;
    //        sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
    //        sfxPlayers[loopIndex].Play();
    //        break;
    //    }
    //}


    public void SetMasterVolume(float volume)
    {
        m_AudioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        m_AudioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        m_AudioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }

    public void ToggleAudioVolume()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }
}