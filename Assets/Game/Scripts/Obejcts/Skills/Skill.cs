using System.Collections.Generic;
using UnityEngine;

public class Skill : Object, IPoolingObject
{
    public float aliveTimer; // 스킬 생존 시간을 체크할 변수
    public float aliveTime; // 스킬 생존 시간

    // 도트 데미지 관련 변수
    public bool isDotDamageSkill;
    protected float dotDelayTime = 0.2f;
    protected List<float> DotDelayTimers = new List<float>();
    protected List<string> DotDamagedEnemies_Name = new List<string>();

    // 스킬이 꺼질 때 SkillManager의 isSkillsActivated[]를 끄기 위한 delegate
    public delegate void OnSkillFinished(int index);
    public OnSkillFinished onSkillFinished;

    public Enemy enemy;
    public Boss boss;
    public Player player;

    public float speed;
    public float damage;

    public int skillIndex;
    public int returnIndex;

    public virtual void Init()
    {
        aliveTimer = 0;

        DotDelayTimers.Clear();
        DotDamagedEnemies_Name.Clear();
    }

    protected virtual void FixedUpdate()
    {
        aliveTimer += Time.fixedDeltaTime;

        if (isDotDamageSkill)
        {
            for (int i = 0; i < DotDelayTimers.Count; i++)
            {
                DotDelayTimers[i] += Time.fixedDeltaTime;
            }
        }
    }


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(gameObject, damage);

            return;
        }

        IDamageableSkill damageableSkill = collision.GetComponent<IDamageableSkill>();
        if (damageableSkill != null)
        {
            damageableSkill.TakeDamage(damage);

            return;
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
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
                damageable.TakeDamage(gameObject, damage);

                DotDelayTimers[enemyIndex] = 0;

                return;
            }
        }

        IDamageableSkill damageableSkill = collision.GetComponent<IDamageableSkill>();
        if (damageableSkill != null)
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
                damageableSkill.TakeDamage(damage);

                DotDelayTimers[enemyIndex] = 0;

                return;
            }
        }
    }
}