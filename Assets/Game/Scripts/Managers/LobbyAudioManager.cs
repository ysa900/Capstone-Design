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
    public AudioClip bgmClip; // BGM ���� Ŭ��
    public float bgmVolume; // BGM ���� ����
    AudioSource bgmPlayer; // BGM ���� ������ҽ�
    AudioHighPassFilter bgmEffect;

    // SFX(ȿ����)
    [Header("#SFX")]
    public AudioClip[] sfxClips; // SFX ���� Ŭ�� �迭
    public float SfxVolume; // SFX ���� ����

    // ����� ä�� �ý���
    public int channels; // �ٷ��� ȿ������ ���� ���� ä�� ���� �迭 ����
    int channelIndex; // ä�� �ε���
    AudioSource[] sfxPlayers; // BGM ���� ������ҽ�

    public enum Sfx { Select }

    private void Awake()
    {
        Init();
    }


    private void Init()
    {
        // ����� �÷��̾� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform; // ������� ����ϴ� �ڽ� ������Ʈ ����
        bgmPlayer =bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = true; // Game ���۵��ڸ��� �ѱ�
        bgmPlayer.loop = true; // BGM ���ѹݺ�
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;

        // ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform; // ȿ������ ����ϴ� �ڽ� ������Ʈ ����
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