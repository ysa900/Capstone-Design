using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyOnSkill : Skill, IPoolingObject
{
    public bool isBossAppear;

    bool isCoroutineNow = false;

    public new void Init()
    {
        base.Init();

        isCoroutineNow = false;

        if (!isBossAppear)
        {
            SetEnemyPosition();
        }
        else
        {
            SetBossPosition();
        }
    }

    private void Start()
    {
        Init();
    }

    protected override void FixedUpdate()
    {
        bool destroySkill;

        if (!isBossAppear) { destroySkill = aliveTimer > aliveTime || enemy == null; }
        else { destroySkill = aliveTimer > aliveTime || boss == null; }

        if (destroySkill && !isCoroutineNow)
        {
            if (onSkillFinished != null)
                onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌

            GameManager.instance.poolManager.ReturnSkill(this, returnIndex);
            return;
        }

        base.FixedUpdate();
    }

    private void SetEnemyPosition()
    {
        Vector2 enemyPosition = enemy.transform.position;

        // 적 실제 위치로 보정
        switch (enemy.tag)
        {
            case "Pumpkin":
            case "WarLock":
            case "Ghoul":
            case "Spitter":
            case "Summoner":
                enemyPosition.y += enemy.capsuleCollider.size.y * 5;
                break;
        }

        X = enemyPosition.x;
        Y = enemyPosition.y;
    }

    private void SetBossPosition()
    {
        Vector2 bossPosition = boss.transform.position;

        // 스킬 위치를 보스 실제 위치로 변경
        X = bossPosition.x;
        Y = bossPosition.y - boss.capsuleCollider.size.y * 4;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
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
        isCoroutineNow = true;
        
        yield return new WaitForSeconds(delayTime); // 지정한 초 만큼 쉬기
        
        if (onSkillFinished != null)
            onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌

        GameManager.instance.poolManager.ReturnSkill(this, returnIndex);
    }
}

