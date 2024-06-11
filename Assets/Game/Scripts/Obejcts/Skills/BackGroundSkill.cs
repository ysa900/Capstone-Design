
public class BackGroundSkill : Skill, IPoolingObject
{
    public bool isStaySkill; // 몇초동안 지속되다가 사라지는 스킬이냐

    public float scale;

    private void Start()
    {
    }

    protected override void FixedUpdate()
    {
        bool destroySkill = aliveTimer > aliveTime;

        if (destroySkill)
        {
            if (onSkillFinished != null)
                onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌

            GameManager.instance.poolManager.ReturnSkill(this, returnIndex);
            
            return;
        }

        base.FixedUpdate();
    }
}