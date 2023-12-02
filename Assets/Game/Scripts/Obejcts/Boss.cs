using JetBrains.Annotations;
using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class Boss : Object, IDamageable
{
    const float Pi = Mathf.PI;

    [SerializeField]
    public float hp;
    public float maxHp = 20000;
    
    [SerializeField]
    float degreeSpeed;

    public Player player;

    // ��ų���� �غ� �ƴ���
    bool bulletSkillShouldBeAttack;
    bool laserSkillShouldBeAttack;
    bool gridLaserSkillShouldBeAttack;
    bool genesisSkillShouldBeAttack;

    // �Ѿ�(�ذ�) ������
    float bulletAttackDelayTimer = 4f;
    float bulletAttackDelay = 10f;

    // ������ ������
    float laserAttackDelayTimer = 8f;
    float laserAttackDelay = 15f;

    // ���� ������ ������
    float gridLaserAttackDelayTimer = 18f;
    float gridLaserAttackDelay = 20f;

    // ���׽ý� ������
    float genesisAttackDelayTimer = 25f;
    float genesisAttackDelay = 30f;

    bool isAttackNow; // ���� ���̸� �������� �ʵ��� �ϱ� ���� ����
    bool isAttackReady = true; // �����߿� �ٸ� ������ ������ �ʵ��� �ϱ� ���� ����

    private float colliderOffsetX; // collider�� offset x��ǥ
    private float colliderOffsetY; // collider�� offset y��ǥ

    private bool isDead;

    private float damageDelay = 4f;
    private float damageDelayTimer = 0;

    public bool isBossLookLeft; // ������ ���� �ִ� ������ �˷��ִ� ����

    private bool isRanNumDecided; // ������ �÷��̾� ������ �󸶳� ������ ���ϴ� ranNum�� ��������
    private bool isGoingClock; // ���� �ð�������� �����ֳ�
    private float ranNum_End; // ranNum �� ��ġ
    private float degree = Pi / 4; // ������ ���� �� ����
    private float radius = 8.5f; // ���� ������
    private float tmpX; // Cirle�� ����� �� 0,0�� �������� ������ X
    private float tmpY; // Cirle�� ����� �� 0,0�� �������� ������ X
    
    Animator animator;

    // BossManager���� ������ �׾��ٰ� �˷��ֱ� ���� Delegate
    public delegate void OnBossDead();
    public OnBossDead onbossDead;

    // BossManager���� ��ų ����� �˷��ֱ� ���� Delegate��
    public delegate void OnBossTryBulletAttack();
    public OnBossTryBulletAttack onBossTryBulletAttack;

    public delegate void OnBossTryLaserAttack(float num);
    public OnBossTryLaserAttack onBossTryLaserAttack;

    public delegate void OnBossTryGridLaserAttack(float x, float y, bool isRightTop);
    public OnBossTryGridLaserAttack onBossTryGridLaserAttack;

    public delegate void OnBossTryGenesisAttack();
    public OnBossTryGenesisAttack onBossTryGenesisAttack;

    Rigidbody2D rigid; // ���� �Է��� �ޱ����� ����
    SpriteRenderer spriteRenderer;
    public CapsuleCollider2D capsuleCollider; // Collider�� offset�� �����ϱ� ���� ����

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        colliderOffsetX = capsuleCollider.offset.x; // offset �ʱⰪ�� ����
        colliderOffsetY = capsuleCollider.offset.y;

        hp = maxHp;
    }

    private void Update()
    {
        if(!isDead)
        {
            SetBossDirection(); // ���� ���� ����

            // Bullet ����
            bulletSkillShouldBeAttack = bulletAttackDelay < bulletAttackDelayTimer; // ���� ��Ÿ���� �ƴ��� Ȯ��
            if (bulletSkillShouldBeAttack && isAttackReady)
            {
                bulletAttackDelayTimer = 0;

                TryAttack(0); // ��ų ��Ÿ���� �� ������ ������ �õ��Ѵ�
            }
            else
            {
                bulletAttackDelayTimer += Time.deltaTime;
            }

            if (hp < maxHp / 100 * 80) // hp�� 80���� �̸��̸�
            {
                // gridLaser ����
                gridLaserSkillShouldBeAttack = gridLaserAttackDelay < gridLaserAttackDelayTimer; // ���� ��Ÿ���� �ƴ��� Ȯ��

                if (gridLaserSkillShouldBeAttack && isAttackReady)
                {
                    gridLaserAttackDelayTimer = 0;

                    TryAttack(3); // ��ų ��Ÿ���� �� ������ ������ �õ��Ѵ�
                }
                else
                {
                    gridLaserAttackDelayTimer += Time.deltaTime;
                }
            }

            if (hp >= maxHp / 100 * 50) // hp�� 50% �̻��̸�
            {
                // Laser ����
                laserSkillShouldBeAttack = laserAttackDelay < laserAttackDelayTimer; // ���� ��Ÿ���� �ƴ��� Ȯ��

                if (laserSkillShouldBeAttack && isAttackReady)
                {
                    laserAttackDelayTimer = 0;

                    TryAttack(1); // ��ų ��Ÿ���� �� ������ ������ �õ��Ѵ�
                }
                else
                {
                    laserAttackDelayTimer += Time.deltaTime;
                }
            }
            else // hp�� 50% �̸��̸�
            {
                // LaserThree ����
                laserSkillShouldBeAttack = laserAttackDelay < laserAttackDelayTimer; // ���� ��Ÿ���� �ƴ��� Ȯ��

                if (laserSkillShouldBeAttack && isAttackReady)
                {
                    laserAttackDelayTimer = 0;

                    TryAttack(2); // ��ų ��Ÿ���� �� ������ ������ �õ��Ѵ�
                }
                else
                {
                    laserAttackDelayTimer += Time.deltaTime;
                }
            }

            if (hp < maxHp / 100 * 30) // hp�� 30���� �̸��̸�
            {
                // Genesis ����
                genesisSkillShouldBeAttack = genesisAttackDelay < genesisAttackDelayTimer; // ���� ��Ÿ���� �ƴ��� Ȯ��

                if (genesisSkillShouldBeAttack && isAttackReady)
                {
                    genesisAttackDelayTimer = 0;

                    TryAttack(4); // ��ų ��Ÿ���� �� ������ ������ �õ��Ѵ�
                }
                else
                {
                    genesisAttackDelayTimer += Time.deltaTime;
                }
            }

            damageDelayTimer += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if(!isDead && !isAttackNow)
            MoveAroundPlayer();
    }

    // �÷��̾� ������ ���� �Լ�
    private void MoveAroundPlayer()
    {
        if (!isRanNumDecided)
        {
            isGoingClock = UnityEngine.Random.Range(0, 2) == 0;
            
            if(isGoingClock)
                ranNum_End = degree - UnityEngine.Random.Range(Pi / 180 * 80, Pi / 180 * 271);
            else
                ranNum_End = degree + UnityEngine.Random.Range(Pi / 180 * 80, Pi / 180 * 271);

            isRanNumDecided = true;

            MoveAroundPlayer();
        }
        else
        {
            if (isGoingClock)
            {
                degree -= degreeSpeed;

                if (degree <= ranNum_End)
                {
                    isRanNumDecided = false;
                }
            }
            else
            {
                degree += degreeSpeed;

                if (degree >= ranNum_End)
                {
                    isRanNumDecided = false;
                }
            }
            
            tmpX = (float)Math.Cos(degree) * radius;
            tmpY = (float)Math.Sin(degree) * radius;

            X = tmpX + player.transform.position.x;
            Y = tmpY + player.transform.position.y;
        }

        if (degree >= Pi * 4)
            degree %= Pi*2;

        if (degree <= -Pi * 4)
            degree %= -Pi * 2;
    }

    private void TryAttack(int num)
    {
        isAttackNow = true;
        isAttackReady = false;

        rigid.constraints = RigidbodyConstraints2D.FreezeAll; // �÷��̾ �� �� �и��� �ʰ� ��ǥ�� ����
        switch (num)
        {
            case 0:
                StartCoroutine(CastBullet());
                break;
            case 1:
                StartCoroutine(CastLaser());
                break;
            case 2:
                StartCoroutine(CastLaserThree());
                break;
            case 3:
                StartCoroutine(CastGridLaser());
                break;
            case 4:
                StartCoroutine(CastGenesis());
                break;
        }
    }

    private void SetBossDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 myPosition = transform.position;

        Vector2 direction = playerPosition - myPosition;
        direction = direction.normalized;

        if (Math.Abs(direction.x) >= 0.3f)
        {
            isBossLookLeft = direction.x < 0;
        }

        spriteRenderer.flipX = isBossLookLeft;

        Vector2 colliderOffset; // CapsuleCollider�� offset�� ���� Vector2

        if (isBossLookLeft) // Enemy�� ������ ���� collider�� x�� ��Ī�� ���ش�
        {
            colliderOffset = new Vector2(-colliderOffsetX, colliderOffsetY);
        }
        else
        {
            colliderOffset = new Vector2(colliderOffsetX, colliderOffsetY);

        }
        capsuleCollider.offset = colliderOffset; // capsuleCollider�� ����
    }

    IEnumerator CastBullet()
    {
        animator.SetBool("Shoot", true);
        yield return new WaitForSeconds(0.3f); // ������ �� ��ŭ ����

        onBossTryBulletAttack();
        animator.SetBool("Shoot", false);

        StartCoroutine(TelePort());
    }

    IEnumerator CastLaser()
    {
        yield return new WaitForSeconds(0.75f); // ������ �� ��ŭ ����

        onBossTryLaserAttack(0f);

        yield return new WaitForSeconds(3.2f); // ������ �� ��ŭ ����

        StartCoroutine(TelePort());
    }

    IEnumerator CastLaserThree()
    {
        yield return new WaitForSeconds(1.5f); // ������ �� ��ŭ ����

        onBossTryLaserAttack(30f);
        onBossTryLaserAttack(0f);
        onBossTryLaserAttack(-30f);

        yield return new WaitForSeconds(3.2f); // ������ �� ��ŭ ����

        StartCoroutine(TelePort());
    }

    IEnumerator CastGridLaser()
    {
        animator.SetTrigger("Genesis");
        animator.SetBool("Genesis_Stay", true);

        float mapTop = player.transform.position.y + 20;
        float mapRight = player.transform.position.x + 20;

        float root2 = 1.414f;
        float tmpX = mapRight - 40f + root2;
        float tmpY = mapTop - root2;

        for (int i = 0; i < 7; i++)
        {
            onBossTryGridLaserAttack(tmpX, tmpY, true);
            tmpX += root2 * 4;
            tmpY -= root2 * 4;
        }

        tmpX = mapRight - 40f + root2;
        tmpY = mapTop - 40f + root2;
        for (int i = 0; i < 7; i++)
        {
            onBossTryGridLaserAttack(tmpX, tmpY, false);
            tmpX += root2 * 4;
            tmpY += root2 * 4;
        }

        yield return new WaitForSeconds(1.6f); // ������ �� ��ŭ ����

        animator.SetBool("Genesis_Stay", false);

        StartCoroutine(TelePort());
    }

    IEnumerator CastGenesis()
    {
        animator.SetTrigger("Genesis");
        animator.SetBool("Genesis_Stay", true);
        yield return new WaitForSeconds(1.5f); // ������ �� ��ŭ ����
        
        onBossTryGenesisAttack();

        yield return new WaitForSeconds(2.5f); // ������ �� ��ŭ ����
        animator.SetBool("Genesis_Stay", false);

        StartCoroutine(TelePort());
    }

    IEnumerator TelePort()
    {
        animator.SetTrigger("Teleport");
        yield return new WaitForSeconds(0.34f); // ������ �� ��ŭ ����

        rigid.constraints = RigidbodyConstraints2D.FreezeRotation; // ��ǥ ������ Ǯ��, ȸ���� ���ϰ�
        isAttackNow = false;

        yield return new WaitForSeconds(0.02f); // ������ �� ��ŭ ����

        isAttackNow = true;

        animator.SetTrigger("Teleport_arrive");

        yield return new WaitForSeconds(0.34f); // ������ �� ��ŭ ����

        isAttackNow = false;

        isAttackReady = true;
    }

    // IDamageable�� �Լ� TakeDamage
    public void TakeDamage(GameObject causer, float damage)
    {
        hp = hp - (int)damage;

        if (hp <= 0)
        {
            StartCoroutine(Dead());
        }
        else
        {
            if (damageDelay <= damageDelayTimer)
            {
                if(!isAttackNow)
                {
                    animator.SetTrigger("Hit");
                }

                damageDelayTimer = 0;
            }
        }
    }

    IEnumerator Dead()
    {
        animator.SetTrigger("Dead");
        //onBossWasKilled(this); // �븮�� ȣ��

        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<CapsuleCollider2D>().enabled = false;

        isDead = true;

        yield return new WaitForSeconds(0.5f); // ������ �� ��ŭ ����

        Destroy(gameObject);
        onbossDead();
    }
}
