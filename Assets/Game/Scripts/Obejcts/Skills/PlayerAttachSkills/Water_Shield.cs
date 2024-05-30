using UnityEngine;

public class Water_Shield : PlayerAttachSkill
{
    public delegate void OnShieldSkillDestroyed();
    public OnShieldSkillDestroyed onShieldSkillDestroyed;

    protected override void FixedUpdate()
    {
        bool destroySkill = aliveTimer > aliveTime;

        if (destroySkill)
        {
            if (onSkillFinished != null)
                onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌

            onShieldSkillDestroyed();

            GameManager.instance.poolManager.ReturnSkill(this, returnIndex);
            return;
        }
        else
        {
            X = player.transform.position.x;
            Y = player.transform.position.y;
        }

        base.FixedUpdate();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // 아무것도 하지 않기 (지우면 안됨, 지우면 virtual로 선언 된 RandomSkill의 TriggerEnter가 작동 됨)
    }
}