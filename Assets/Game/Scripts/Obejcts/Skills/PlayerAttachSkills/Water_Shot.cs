
public class Water_Shot : PlayerAttachSkill
{
    protected override void FixedUpdate()
    {
        bool destroySkill = aliveTimer > aliveTime;

        if (destroySkill)
        {
            if (onSkillFinished != null)
                onSkillFinished(skillIndex); // skillManager���� delegate�� �˷���

            GameManager.instance.poolManager.ReturnSkill(this, returnIndex);
            return;
        }
        else
            AttachPlayer();

        base.FixedUpdate();
    }

}
