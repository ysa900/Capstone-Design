using System;
using UnityEngine;

public class ArrangeEnemy : Enemy
{
    // Arrange Enemy attackRange
    int[] attackRanges = { 12 };

    [SerializeField] float attackRange;

    float attackCoolTime = 5f;
    float attackCoolTimer = 5f;

    public override void Init()
    {
        attackCoolTimer = 5f;

        switch (tag)
        {
            case "Skeleton_Archer":
                attackRange = attackRanges[0];
                break;
        }

        base.Init();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void FixedUpdate()
    {
        if (isDead)
            return;

        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

        Vector3 enemyPos = transform.position;
        Vector3 playerPos = player.transform.position;
        float distance = Vector3.Distance(enemyPos, playerPos);

        bool isInAttackRange = distance <= attackRange; // 플레이어가 사거리 내에 있을때만 공격이 나간다
        bool isAttackOK = attackCoolTime <= attackCoolTimer; // 플레이어가 사거리 내에 있을때만 공격이 나간다
        
        if (!isInAttackRange)
        {
            agent.enabled = true;
            LookAtPlayer();
            MoveToPlayer();
        }
        else if (isAttackOK)
        {
            Arrange_Attack();
            attackCoolTimer = 0;
            agent.enabled = false;
        }
        else
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            LookAtPlayer();
            agent.enabled = false;
        }

        attackCoolTimer += Time.fixedDeltaTime;

        base.FixedUpdate();
    }

    void Arrange_Attack()
    {
        switch (tag)
        {
            case "Skeleton_Archer":
                animator.SetTrigger("Attack");
                float compensateX = 0.7f;
                float compensateY = 0.01f;
                GameManager.instance.poolManager.GetArrow(compensateX, compensateY, this);
                break;
        }
        
    }
}