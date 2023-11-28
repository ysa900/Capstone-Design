using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class Boss : Object, IDamageable
{
    const float Pi = Mathf.PI;

    [SerializeField]
    int hp;
    [SerializeField]
    float degreeSpeed;

    public Player player;

    // ��ų���� �غ� �ƴ���
    bool bulletSkillShouldBeAttack;
    bool laserSkillShouldBeAttack;
    bool genesisSkillShouldBeAttack;

    // �Ѿ�(�ذ�) ������
    float bulletAttackDelayTimer = 4f;
    float bulletAttackDelay = 6f;

    // ������ ������
    float laserAttackDelayTimer = 14f;
    float laserAttackDelay = 20f;

    // ���׽ý� ������
    float genesisAttackDelayTimer = 15f;
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

    // BossManager���� ��ų ����� �˷��ֱ� ���� Delegate��
    public delegate void OnBossTryBulletAttack();
    public OnBossTryBulletAttack onBossTryBulletAttack;

    public delegate void OnBossTryLaserAttack();
    public OnBossTryLaserAttack onBossTryLaserAttack;

    public delegate void OnBossTryGenesisAttack();
    public OnBossTryLaserAttack onBossTryGenesisAttack;

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
    }

    private void Update()
    {
        if(!isDead)
        {
            SetBossDirection(); // ���� ���� ����

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

            genesisSkillShouldBeAttack = genesisAttackDelay < genesisAttackDelayTimer; // ���� ��Ÿ���� �ƴ��� Ȯ��

            if (genesisSkillShouldBeAttack && isAttackReady)
            {
                genesisAttackDelayTimer = 0;

                TryAttack(2); // ��ų ��Ÿ���� �� ������ ������ �õ��Ѵ�
            }
            else
            {
                genesisAttackDelayTimer += Time.deltaTime;
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
                StartCoroutine(Shoot());
                break;
            case 1:
                StartCoroutine(CastLaser());
                break;
            case 2:
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

    IEnumerator Shoot()
    {
        animator.SetBool("Shoot", true);
        yield return new WaitForSeconds(0.3f); // ������ �� ��ŭ ����

        onBossTryBulletAttack();
        animator.SetBool("Shoot", false);

        StartCoroutine(TelePort());
    }

    IEnumerator CastLaser()
    {
        yield return new WaitForSeconds(1.0f); // ������ �� ��ŭ ����

        onBossTryLaserAttack();

        yield return new WaitForSeconds(2.6f); // ������ �� ��ŭ ����

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
            StopCoroutine(Dead());
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
    }
}
