using System;
using UnityEngine;

public class ArrangeEnemy_ML : Enemy_ML
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

        Vector2 enemyPos = transform.position;
        Vector2 playerPos_ML = player_ML.transform.position;
        float distance = Vector2.Distance(enemyPos, playerPos_ML);

        bool isInAttackRange = distance <= attackRange; // 플레이어가 사거리 내에 있을때만 공격이 나간다
        bool isAttackOK = attackCoolTime <= attackCoolTimer; // 플레이어가 사거리 내에 있을때만 공격이 나간다
        
        if (!isInAttackRange)
        {
            LookAtPlayer();
            MoveToPlayer();
        }
        else if (isAttackOK)
        {
            Arrange_Attack();
            attackCoolTimer = 0;
        }
        else
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            LookAtPlayer();
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
                GameManager.instance.poolManager_ML.GetArrow(compensateX, compensateY, this);
                break;
        }
        
    }
}