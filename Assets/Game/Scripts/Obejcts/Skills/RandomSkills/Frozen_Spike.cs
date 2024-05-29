using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frozen_Spike : RandomSkill
{
    public float delay;
    private float delayTimer = 0f;
    public override void Init()
    {
        delayTimer = 0f;

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

        delayTimer += Time.fixedDeltaTime;

        base.FixedUpdate();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (delayTimer >= delay) // 이거 추가한 것
            {
                damageable.TakeDamage(gameObject, damage);

                return;
            }
            else
            {
                float waitTime = delay - delayTimer;
                StartCoroutine(EnemyAttack(waitTime, damageable));

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
            else
            {
                float waitTime = delay - delayTimer;
                StartCoroutine(SkillAttack(waitTime, damageableSkill));

                return;
            }
        }
    }

    IEnumerator EnemyAttack(float time, IDamageable damageable)
    {
        yield return new WaitForSeconds(time);

        damageable.TakeDamage(gameObject, damage);
    }

    IEnumerator SkillAttack(float time, IDamageableSkill damageableSkill)
    {
        yield return new WaitForSeconds(time);

        damageableSkill.TakeDamage(damage);
    }
}
