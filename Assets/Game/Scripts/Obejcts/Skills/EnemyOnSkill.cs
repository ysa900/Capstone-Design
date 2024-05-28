using System.Collections;
using UnityEngine;

public class EnemyOnSkill : Skill, IPoolingObject
{
    public bool isBossAppear;

    private float aliveTime = 0f; // 스킬 생존 시간을 체크할 변수

    private string sceneName;
    public new void Init()
    {
        aliveTime = 0f;
    }

    private void Start()
    {
        sceneName = GameManager.instance.sceneName;
    }

    private void FixedUpdate()
    {
        bool destroySkill;

        if (!isBossAppear) { destroySkill = aliveTime > 1.5f || enemy == null; }
        else { destroySkill = aliveTime > 1.5f || boss == null; }

        if (destroySkill)
        {
            if (onSkillFinished != null)
                onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌

            switch(sceneName)
            {
                case "Stage1_ML":
                    GameManager.instance.poolManager_ML.ReturnSkill(this, returnIndex);
                    break;
                default:
                    GameManager.instance.poolManager.ReturnSkill(this, returnIndex);
                    break;
            }

            return;
        }

        aliveTime += Time.fixedDeltaTime;
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

        if (onSkillFinished != null)
            onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌

             switch(sceneName)
            {
                case "Stage1_ML":
                    GameManager.instance.poolManager_ML.ReturnSkill(this, returnIndex);
                    break;
                default:
                    GameManager.instance.poolManager.ReturnSkill(this, returnIndex);
                    break;
            }
    }
}

