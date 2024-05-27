using UnityEngine;

public class BeamLaser : PlayerAttachSkill
{
    public float delay = 1f;
    private float delayTimer = 0f;

    public override void Init()
    {
        delayTimer = 0;

        base.Init();
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
        else if(delayTimer < delay)
        {
            AttachPlayer();
        }

        base.FixedUpdate();

        delayTimer += Time.fixedDeltaTime;
    }

    private new void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (delayTimer >= delay) // 이거 추가한 것
            {
                damageable.TakeDamage(gameObject, damage);

                return;
            }
        }

        IDamageableSkill damageableSkill = collision.GetComponent<IDamageableSkill>();
        if (damageableSkill != null)
        {
            if (delayTimer >= delay) // 이거 추가한 것
            {
                damageableSkill.TakeDamage(damage);

                return;
            }
        }
    }

    private new void OnTriggerStay2D(Collider2D collision)
    {
        if (!isDotDamageSkill) return;

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            int enemyIndex = DotDamagedEnemies_Name.FindIndex(x => x == collision.gameObject.name);

            if (enemyIndex == -1)
            {
                DotDamagedEnemies_Name.Add(collision.gameObject.name);
                DotDelayTimers.Add(1f);

                enemyIndex = DotDamagedEnemies_Name.Count - 1;
            }

            if (DotDelayTimers[enemyIndex] < dotDelayTime) { return; }
            else
            {
                if (delayTimer >= delay) // 이거 추가한 것
                {
                    damageable.TakeDamage(gameObject, damage);

                    DotDelayTimers[enemyIndex] = 0;
                }

                return;
            }
        }

        IDamageableSkill damageableSkill = collision.GetComponent<IDamageableSkill>();

        if (damageableSkill != null && isDotDamageSkill)
        {
            int enemyIndex = DotDamagedEnemies_Name.FindIndex(x => x == collision.gameObject.name);

            if (enemyIndex == -1)
            {
                DotDamagedEnemies_Name.Add(collision.gameObject.name);
                DotDelayTimers.Add(1f);

                enemyIndex = DotDamagedEnemies_Name.Count - 1;
            }

            if (DotDelayTimers[enemyIndex] < dotDelayTime) { return; }
            else
            {
                if (delayTimer >= delay) // 이거 추가한 것
                {
                    damageableSkill.TakeDamage(damage);

                    DotDelayTimers[enemyIndex] = 0;
                }

                return;
            }
        }

    }
}

