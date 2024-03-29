using UnityEngine.AI;

public class Skill : Object, IPullingObject
{
    // 스킬이 꺼질 때 SkillManager의 isSkillsActivated[]를 끄기 위한 delegate
    public delegate void OnSkillFinished(int index);
    public OnSkillFinished onSkillFinished;

    public void Init() { } // PoolManager때문에 이거 지우면 안됨

    public Enemy enemy;
    public Boss boss;
    public Player player;

    public float speed;
    public float damage;

    public int skillIndex;
    public int returnIndex;
}