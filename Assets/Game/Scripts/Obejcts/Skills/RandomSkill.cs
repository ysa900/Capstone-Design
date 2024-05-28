﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSkill : Skill, IPoolingObject
{
    public RandomSkill randomSkill;

    public float impactPointX;
    public float impactPointY;

    private float aliveTimer; // 스킬 생존 시간을 체크할 변수
    public float aliveTime; // 스킬 생존 시간
    private bool isCoroutineNow; // 현재 코루틴을 실행하고 있는지를 체크할 변수

    public bool isMeteor; // 메테오면 날아오는 도중엔 데미지 없어야 하므로 만든 변수
    public bool isIceSpike; // IceSpike 스킬의 자식 오브젝트 때문에 만든 변수
    public bool isStaySkill; // 몇초동안 지속되다가 사라지는 스킬이냐

    public float scale;
    private string sceneName;

    Rigidbody2D rigid; // 물리 입력을 받기위한 변수
    Vector2 direction; // 날아갈 방향

    Animator animator;
    Animator animator_ground;

    public new void Init()
    {
        aliveTimer = 0;

        DotDelayTimers.Clear();
        DotDamagedEnemies_Name.Clear();

        if (isIceSpike)
        {
            animator_ground = transform.Find("ground").GetComponent<Animator>();
        }
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sceneName = GameManager.instance.sceneName;

        if (isIceSpike)
        {
            animator_ground = transform.Find("ground").GetComponent<Animator>();
        }
    }

    private void FixedUpdate()
    {
        bool destroySkill = aliveTimer > aliveTime;

        if (destroySkill)
        {
            if (isMeteor)
            {
                RandomSkill explode;

                switch(sceneName)
                {
                    case "Stage1_ML":
                        explode = GameManager.instance.poolManager_ML.GetSkill(2) as RandomSkill;
                        break;
                    default:
                        explode = GameManager.instance.poolManager.GetSkill(2) as RandomSkill;
                        break;   
                }

                explode.X = X;
                explode.Y = Y;

                explode.aliveTime = 0.5f;
                explode.damage = damage;

                Transform parent = explode.transform.parent;

                explode.transform.parent = null;
                explode.transform.localScale = new Vector3(scale, scale, 0);
                explode.transform.parent = parent;
            }
            if (isStaySkill)
            {
                if(!isCoroutineNow)
                    StartCoroutine(Disappear());
            }
            else
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
            }
            
            return;
        }
        else
        {
            if(isMeteor) { MoveToimpactPoint(); }
        }

        aliveTimer += Time.fixedDeltaTime;

        if (isDotDamageSkill)
        {
            for (int i = 0; i < DotDelayTimers.Count; i++)
            {
                DotDelayTimers[i] += Time.fixedDeltaTime;
            }
        }
    }

    // 날아갈 방향을 정하는 함수
    public void setDirection()
    {
        Vector2 impactVector = new Vector2 (impactPointX, impactPointY);
        Vector2 nowVector = transform.position;

        direction = impactVector - nowVector;

        direction = direction.normalized;
    }

    // 폭발 지점으로 이동하는 함수
    private void MoveToimpactPoint()
    {
        setDirection();

        rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime);

        X = transform.position.x;
        Y = transform.position.y;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isMeteor)
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
                damageable.TakeDamage(gameObject, damage);

                return;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDotDamageSkill)
        {
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
                }
                
                return;
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
                    damageable.TakeDamage(gameObject, damage);

                    DotDelayTimers[enemyIndex] = 0;
                }

                return;
            }
        }
    }

    IEnumerator Disappear()
    {
        animator.SetTrigger("Finish");
        animator_ground.SetTrigger("Finish");

        isCoroutineNow = true;
        
        yield return new WaitForSeconds(0.2f); // 지정한 초 만큼 쉬기

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

        isCoroutineNow = false;
    }
}