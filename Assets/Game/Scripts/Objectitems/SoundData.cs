using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Scriptable Object/SoundData")]

public class SoundData : ScriptableObject
{
    // 사운드 시스템
    [Header("# Sound 정보")]
    public float masterSound;
    public float bgmSound;
    public float sfxSound;
    public bool isMute;

    public bool isFirstLobby;
}