using System.Collections;
using UnityEngine;

public class EnemyOnSkill : Skill
{
    public bool isBossAppear;

    private float aliveTime = 0f; // 스킬 생존 시간을 체크할 변수

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        /*
        bool destroySkill = aliveTime > 1f || enemy == null;

        if (destroySkill)
        {
            Destroy(gameObject);
            return;
        }

        aliveTime += Time.fixedDeltaTime;*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(gameObject, damage);

            StartCoroutine(Delay(0.5f));

            return;
        }

        IDamageableSkill damageableSkill = collision.GetComponent<IDamageableSkill>();

        if (damageableSkill != null)
        {
            damageableSkill.TakeDamage(damage);

            StartCoroutine(Delay(0.5f));

            return;
        }
    }

    IEnumerator Delay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime); // 지정한 초 만큼 쉬기
        
        Destroy(gameObject);
    }
}

