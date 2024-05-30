using UnityEngine;

public class EnemyTrackingSkill : Skill, IPoolingObject
{
    public bool isBossAppear;

    Rigidbody2D rigid; // 물리 입력을 받기위한 변수

    Vector2 enemyPosition;
    Vector2 bossPosition;
    Vector2 myPosition;
    Vector2 direction;

    public override void Init()
    {
        base.Init();

        X = player.transform.position.x;
        Y = player.transform.position.y;

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
        rigid = GetComponent<Rigidbody2D>();
        
        if(!isBossAppear)
        {
            SetEnemyPosition();
        }
        else
        {
            SetBossPosition();
        }
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
        else if(!isBossAppear)
        {
            MoveToEnemy();
        }
        else
        {
            MoveToBoss();
        }

        base.FixedUpdate();
    }

    private void SetEnemyPosition()
    {
        enemyPosition = enemy.transform.position;

        myPosition = player.transform.position;

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

        direction = enemyPosition - myPosition;

        direction = direction.normalized;
    }

    private void SetBossPosition()
    {
        bossPosition = boss.transform.position;
        myPosition = transform.position;

        // 적 실제 위치로 보정
        if (boss.isBossLookLeft)
            bossPosition.x -= boss.capsuleCollider.size.x * 4;
        else
            bossPosition.x += boss.capsuleCollider.size.x * 4;
        bossPosition.y -= boss.capsuleCollider.size.y * 4;

        direction = bossPosition - myPosition;

        direction = direction.normalized;
    }

    // 적을 따라가는 스킬
    private void MoveToEnemy()
    {
        rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime); // Enemy 방향으로 위치 변경
        
        X = transform.position.x;
        Y = transform.position.y;
    }

    // 보스를 따라가는 스킬
    private void MoveToBoss()
    {
        rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime); // Boss 방향으로 위치 변경

        X = transform.position.x;
        Y = transform.position.y;
    }
}