using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Object/PlayerData")]

public class PlayerData : ScriptableObject
{
    // 플레이어 정보
    [Header("# Player 정보")]
    public float speed;
    public float hp;
    public float maxHp;
    public float Exp;
    public int level;
    public int[] nextExp;

    public int kill; // 몬스터 킬 수

    // 플레이어 패시브 효과 관련 변수
    [Header("# Player 패시브 효과")]
    public float damageReductionValue; // 뎀감
    public float magnetRange; // 자석 범위
}
