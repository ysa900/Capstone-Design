using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Analytics;

public class GameAudioManager : MonoBehaviour
{

    // BGM
    [Header("#BGM")]
    public AudioClip[] bgmClips; // BGM 관련 클립 배열
    public float bgmVolume; // BGM 관련 볼륨
    AudioSource[] bgmPlayers; // BGM 관련 오디오소스
    AudioHighPassFilter bgmEffect;

    // SFX(효과음)
    [Header("#SFX")]
    public AudioClip[] sfxClips; // SFX 관련 클립 배열
    public float SfxVolume; // SFX 관련 볼륨
    // 오디오 채널 시스템
    public int channels; // 다량의 효과음을 내기 위해 채널 개수 배열 선언
    int channelIndex; // 채널 인덱스
    AudioSource[] sfxPlayers; // BGM 관련 오디오소스

    // 오디오 겹침 현상 해결 위한 코드(변수), 보류
    private Dictionary<AudioClip, List<float>> soundOneShot = new Dictionary<AudioClip, List<float>>();
    private int MaxDuplicateOneShotAudioClips = 30; // oneshot이 최대 겹처서 재생될수 잇는 수

    public enum Sfx { Dead, Hit, LevelUp = 3, Lose, Melee, Range = 7, Select, Win }

    private void Awake()
    {
        Init();
    }


    private void Init()
    {
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform; // 배경음을 담당하는 자식 오브젝트 생성
        bgmPlayers = new AudioSource[2];

        bgmPlayers[0] = bgmObject.AddComponent<AudioSource>();
        bgmPlayers[0].playOnAwake = false; // Game 시작되자마자 켜지지 않게..
        bgmPlayers[0].loop = true; // BGM 무한반복
        bgmPlayers[0].volume = bgmVolume;
        bgmPlayers[0].clip = bgmClips[0];

        bgmPlayers[1] = bgmObject.AddComponent<AudioSource>();
        bgmPlayers[1].playOnAwake = false; // Game 시작되자마자 켜지지 않게..
        bgmPlayers[1].loop = true; // BGM 무한반복
        bgmPlayers[1].volume = bgmVolume;
        bgmPlayers[1].clip = bgmClips[1];

        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform; // 효과음을 담당하는 자식 오브젝트 생성
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].bypassListenerEffects = true;
            sfxPlayers[index].volume = SfxVolume;
        }
    }

    public void PlayBGM(int num, bool isPlay)
    {
        if (isPlay)
        {
            bgmPlayers[num].Play();
        }
        else
        {
            bgmPlayers[num].Stop();
        }
    }

    public void EffectBGM(bool isPlay)
    {
        bgmEffect.enabled = isPlay;
    }

    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }

            int ranIndex = 0;
            if (sfx == Sfx.Hit || sfx == Sfx.Melee)
            {
                ranIndex = Random.Range(0, 2);
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            // 보류
            //PlayOneShotSound(sfxPlayers[loopIndex], sfxClips[loopIndex], 0.2f);
            break;
        }
    }


    // 오디오 겹침 현상 해결 위한 코드(메소드), 보류
    void PlayOneShotSound(AudioSource source, AudioClip clip, float volumeScale)
    {

        //해당 클립당 재생되고 잇는 사운드 수를 계산하기위해 아래와같이 처리한다
        // 재생수가 max 만큼이면 재생안한다
        if (!soundOneShot.ContainsKey(clip))
        {
            soundOneShot[clip] = new List<float>() { volumeScale };
        }
        else
        {
            int count = soundOneShot[clip].Count;
            //한클립당 현재 재생수가 지정한 갯수를 넘으면 리턴한다
            if (count == MaxDuplicateOneShotAudioClips) return;
            soundOneShot[clip].Add(volumeScale);
        }
        int count1 = soundOneShot[clip].Count;


        source.PlayOneShot(clip, volumeScale);
        StartCoroutine(RemoveVolumeFromClip(clip, volumeScale));

    }

    private IEnumerator RemoveVolumeFromClip(AudioClip clip, float volume)
    {
        // 재생 시간동안기다리고 그후에 저장된 값을 지운다
        yield return new WaitForSeconds(clip.length);

        List<float> volumes;
        if (soundOneShot.TryGetValue(clip, out volumes))
        {
            volumes.Remove(volume);
        }
    }
}