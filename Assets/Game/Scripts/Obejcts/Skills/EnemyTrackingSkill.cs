using UnityEngine;

public class EnemyTrackingSkill : Skill, IPoolingObject
{
    public bool isBossAppear;

    private float aliveTime; // 스킬 생존 시간을 체크할 변수

    Rigidbody2D rigid; // 물리 입력을 받기위한 변수

    Vector2 enemyPosition;
    Vector2 bossPosition;
    Vector2 myPosition;
    Vector2 direction;

    private string sceneName;

    public new void Init()
    {
        aliveTime = 0;

        switch(sceneName)
        {
            case "Stage1_ML":
                X = player_ML.transform.position.x;
                Y = player_ML.transform.position.y;          
                break;
            default:
                X = player.transform.position.x;
                Y = player.transform.position.y;
                break;
        }


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
        sceneName = GameManager.instance.sceneName;
        
        if(!isBossAppear)
        {
            SetEnemyPosition();
        }
        else
        {
            SetBossPosition();
        }
    }

    private void FixedUpdate()
    {
        bool destroySkill = aliveTime > 1f;

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
        else if(!isBossAppear)
        {
            MoveToEnemy();
        }
        else
        {
            MoveToBoss();
        }

        aliveTime += Time.fixedDeltaTime;
    }

    private void SetEnemyPosition()
    {
        switch(sceneName)
        {
            case "Stage1_ML":
                enemyPosition = enemy_ML.transform.position; // ML
                myPosition = player.transform.position;

                // 적 실제 위치로 보정

                switch (enemy_ML.tag)
                {
                    case "Pumpkin":
                    case "WarLock":
                    case "Ghoul":
                    case "Spitter":
                    case "Summoner":
                        enemyPosition.y += enemy_ML.capsuleCollider.size.y * 5;
                        break;
                }

                direction = enemyPosition - myPosition;
                direction = direction.normalized;

                break;
            default:
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
            break;
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(gameObject, damage);

            //GameManager.instance.poolManager.ReturnSkill(this, index);

            return;
        }

        IDamageableSkill damageableSkill = collision.GetComponent<IDamageableSkill>();

        if (damageableSkill != null)
        {
            damageableSkill.TakeDamage(damage);

            //GameManager.instance.poolManager.ReturnSkill(this, index);

            return;
        }
    }
}

