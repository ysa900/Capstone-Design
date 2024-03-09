using Unity.VisualScripting;
using UnityEngine;

public class EnemyTrackingSkill : Skill, IPullingObject
{
    public bool isBossAppear;

    private float aliveTime; // 스킬 생존 시간을 체크할 변수

    Rigidbody2D rigid; // 물리 입력을 받기위한 변수

    Vector2 enemyPosition;
    Vector2 bossPosition;
    Vector2 myPosition;
    Vector2 direction;

    public new void Init()
    {
        aliveTime = 0;

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

    private void FixedUpdate()
    {
        bool destroySkill = aliveTime > 3f;

        if (destroySkill)
        {
            GameManager.instance.poolManager.ReturnSkill(this, index);
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
        enemyPosition = enemy.transform.position;

        myPosition = player.transform.position;

        // 적 실제 위치로 보정
        if (enemy.isEnemyLookLeft)
            enemyPosition.x -= enemy.capsuleCollider.size.x * 5;
        else
            enemyPosition.x += enemy.capsuleCollider.size.x * 5;
        enemyPosition.y += enemy.capsuleCollider.size.y * 5;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(gameObject, damage);

            GameManager.instance.poolManager.ReturnSkill(this, index);

            return;
        }

        IDamageableSkill damageableSkill = collision.GetComponent<IDamageableSkill>();

        if (damageableSkill != null)
        {
            damageableSkill.TakeDamage(damage);

            GameManager.instance.poolManager.ReturnSkill(this, index);

            return;
        }
    }
}

