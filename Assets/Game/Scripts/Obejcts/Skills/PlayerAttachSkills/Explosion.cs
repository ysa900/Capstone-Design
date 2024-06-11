
public class Explosion : PlayerAttachSkill
{
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
        else
        {
            X = player.transform.position.x;
            Y = player.transform.position.y;
        }

        base.FixedUpdate();
    }
}
