using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Analytics;

public class LobbyAudioManager : MonoBehaviour
{
    // BGM
    [Header("#BGM")]
    public AudioClip bgmClip; // BGM 관련 클립
    public float bgmVolume; // BGM 관련 볼륨
    AudioSource bgmPlayer; // BGM 관련 오디오소스
    AudioHighPassFilter bgmEffect;

    // SFX(효과음)
    [Header("#SFX")]
    public AudioClip[] sfxClips; // SFX 관련 클립 배열
    public float SfxVolume; // SFX 관련 볼륨

    // 오디오 채널 시스템
    public int channels; // 다량의 효과음을 내기 위해 채널 개수 배열 선언
    int channelIndex; // 채널 인덱스
    AudioSource[] sfxPlayers; // BGM 관련 오디오소스

    public enum Sfx { Select }

    private void Awake()
    {
        Init();
    }


    private void Init()
    {
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform; // 배경음을 담당하는 자식 오브젝트 생성
        bgmPlayer =bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = true; // Game 시작되자마자 켜기
        bgmPlayer.loop = true; // BGM 무한반복
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform; // 효과음을 담당하는 자식 오브젝트 생성
        sfxPlayers = new AudioSource[channels];

        for(int index=0; index<sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].bypassListenerEffects = true;
            sfxPlayers[index].volume = SfxVolume;
        }
    }

    public void PlayBGM(bool isPlay)
    {
        if (isPlay)
        {
            bgmPlayer.Play();
        }
        else {
            bgmPlayer.Stop();
        }
    }

    public void PlaySfx(Sfx sfx)
    {
        for(int index=0; index<sfxPlayers.Length; index++)
        {
            int loopIndex=(index+channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
}