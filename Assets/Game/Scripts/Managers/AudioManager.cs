using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Analytics;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // AudioManager �ν��Ͻ�ȭ

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

    // ����� ��ħ ���� �ذ� ���� �ڵ�(����)
    private Dictionary<AudioClip, List<float>> soundOneShot = new Dictionary<AudioClip, List<float>>();
    private int MaxDuplicateOneShotAudioClips = 30; // oneshot�� �ִ� ��ó�� ����ɼ� �մ� ��


    public enum Sfx { Dead, Hit, LevelUp=3, Lose, Melee, Range=7, Select, Win }

    private void Awake()
    {
        instance = this;
        Init();
    }
    void Init()
    {
        // ����� �÷��̾� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform; // ������� ����ϴ� �ڽ� ������Ʈ ����
        bgmPlayer =bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false; // Game ���۵��ڸ��� ������ �ʰ�..
        bgmPlayer.loop = true; // BGM ���ѹݺ�
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

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

    public void EffectBGM(bool isPlay)
    {
        bgmEffect.enabled = isPlay;
    }

    public void PlaySfx(Sfx sfx)
    {
        for(int index=0; index<sfxPlayers.Length; index++)
        {
            int loopIndex=(index+channelIndex) %sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }

            int ranIndex = 0;
            if(sfx == Sfx.Hit || sfx == Sfx.Melee)
            {
                ranIndex = Random.Range(0, 2);
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            // ??
            //PlayOneShotSound(sfxPlayers[loopIndex], sfxClips[loopIndex], 0.2f);
            break;
        }
    }
    // ����� ��ħ ���� �ذ� ���� �ڵ�(�޼ҵ�)
    void PlayOneShotSound(AudioSource source, AudioClip clip, float volumeScale)
    {

        //�ش� Ŭ���� ����ǰ� �մ� ���� ���� ����ϱ����� �Ʒ��Ͱ��� ó���Ѵ�
        // ������� max ��ŭ�̸� ������Ѵ�
        if (!soundOneShot.ContainsKey(clip))
        {
            soundOneShot[clip] = new List<float>() { volumeScale };
        }
        else
        {
            int count = soundOneShot[clip].Count;
            //��Ŭ���� ���� ������� ������ ������ ������ �����Ѵ�
            if (count == MaxDuplicateOneShotAudioClips) return;
            soundOneShot[clip].Add(volumeScale);
        }
        int count1 = soundOneShot[clip].Count;
        //Debug.Log(clip.name + " ������� : " + count1);


        source.PlayOneShot(clip, volumeScale);
        StartCoroutine(RemoveVolumeFromClip(clip, volumeScale));

    }

    private IEnumerator RemoveVolumeFromClip(AudioClip clip, float volume)
    {
        // ��� �ð����ȱ�ٸ��� ���Ŀ� ����� ���� �����
        yield return new WaitForSeconds(clip.length);

        List<float> volumes;
        if (soundOneShot.TryGetValue(clip, out volumes))
        {
            volumes.Remove(volume);
        }
    }
}