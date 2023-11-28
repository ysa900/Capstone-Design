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

    // 스킬들이 준비 됐는지
    bool bulletSkillShouldBeAttack;
    bool laserSkillShouldBeAttack;
    bool genesisSkillShouldBeAttack;

    // 총알(해골) 딜레이
    float bulletAttackDelayTimer = 4f;
    float bulletAttackDelay = 6f;

    // 레이저 딜레이
    float laserAttackDelayTimer = 14f;
    float laserAttackDelay = 20f;

    // 제네시스 딜레이
    float genesisAttackDelayTimer = 15f;
    float genesisAttackDelay = 30f;

    bool isAttackNow; // 공격 중이면 움직이지 않도록 하기 위한 변수
    bool isAttackReady = true; // 공격중엔 다른 공격이 나가지 않도록 하기 위한 변수

    private float colliderOffsetX; // collider의 offset x좌표
    private float colliderOffsetY; // collider의 offset y좌표

    private bool isDead;

    private float damageDelay = 4f;
    private float damageDelayTimer = 0;

    public bool isBossLookLeft; // 보스가 보고 있는 방향을 알려주는 변수

    private bool isRanNumDecided; // 보스가 플레이어 주위를 얼마나 돌지를 정하는 ranNum이 정해졌냐
    private bool isGoingClock; // 지금 시계방향으로 돌고있냐
    private float ranNum_End; // ranNum 끝 위치
    private float degree = Pi / 4; // 보스의 현재 원 각도
    private float radius = 8.5f; // 원의 반지름
    private float tmpX; // Cirle을 계산할 때 0,0을 기준으로 생각한 X
    private float tmpY; // Cirle을 계산할 때 0,0을 기준으로 생각한 X
    
    Animator animator;

    // BossManager에게 스킬 사용을 알려주기 위한 Delegate들
    public delegate void OnBossTryBulletAttack();
    public OnBossTryBulletAttack onBossTryBulletAttack;

    public delegate void OnBossTryLaserAttack();
    public OnBossTryLaserAttack onBossTryLaserAttack;

    public delegate void OnBossTryGenesisAttack();
    public OnBossTryLaserAttack onBossTryGenesisAttack;

    Rigidbody2D rigid; // 물리 입력을 받기위한 변수
    SpriteRenderer spriteRenderer;
    public CapsuleCollider2D capsuleCollider; // Collider의 offset을 변경하기 위한 변수

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        colliderOffsetX = capsuleCollider.offset.x; // offset 초기값을 저장
        colliderOffsetY = capsuleCollider.offset.y;
    }

    private void Update()
    {
        if(!isDead)
        {
            SetBossDirection(); // 보스 방향 설정

            bulletSkillShouldBeAttack = bulletAttackDelay < bulletAttackDelayTimer; // 공격 쿨타임이 됐는지 확인
            if (bulletSkillShouldBeAttack && isAttackReady)
            {
                bulletAttackDelayTimer = 0;

                TryAttack(0); // 스킬 쿨타임이 다 됐으면 공격을 시도한다
            }
            else
            {
                bulletAttackDelayTimer += Time.deltaTime;
            }

            laserSkillShouldBeAttack = laserAttackDelay < laserAttackDelayTimer; // 공격 쿨타임이 됐는지 확인

            if (laserSkillShouldBeAttack && isAttackReady)
            {
                laserAttackDelayTimer = 0;

                TryAttack(1); // 스킬 쿨타임이 다 됐으면 공격을 시도한다
            }
            else
            {
                laserAttackDelayTimer += Time.deltaTime;
            }

            genesisSkillShouldBeAttack = genesisAttackDelay < genesisAttackDelayTimer; // 공격 쿨타임이 됐는지 확인

            if (genesisSkillShouldBeAttack && isAttackReady)
            {
                genesisAttackDelayTimer = 0;

                TryAttack(2); // 스킬 쿨타임이 다 됐으면 공격을 시도한다
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

    // 플레이어 주위를 도는 함수
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

        rigid.constraints = RigidbodyConstraints2D.FreezeAll; // 플레이어가 밀 때 밀리지 않게 좌표를 고정
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

        Vector2 colliderOffset; // CapsuleCollider의 offset에 넣을 Vector2

        if (isBossLookLeft) // Enemy가 왼쪽을 보면 collider도 x축 대칭을 해준다
        {
            colliderOffset = new Vector2(-colliderOffsetX, colliderOffsetY);
        }
        else
        {
            colliderOffset = new Vector2(colliderOffsetX, colliderOffsetY);

        }
        capsuleCollider.offset = colliderOffset; // capsuleCollider에 적용
    }

    IEnumerator Shoot()
    {
        animator.SetBool("Shoot", true);
        yield return new WaitForSeconds(0.3f); // 지정한 초 만큼 쉬기

        onBossTryBulletAttack();
        animator.SetBool("Shoot", false);

        StartCoroutine(TelePort());
    }

    IEnumerator CastLaser()
    {
        yield return new WaitForSeconds(1.0f); // 지정한 초 만큼 쉬기

        onBossTryLaserAttack();

        yield return new WaitForSeconds(2.6f); // 지정한 초 만큼 쉬기

        StartCoroutine(TelePort());
    }

    IEnumerator CastGenesis()
    {
        animator.SetTrigger("Genesis");
        animator.SetBool("Genesis_Stay", true);
        yield return new WaitForSeconds(1.5f); // 지정한 초 만큼 쉬기
        
        onBossTryGenesisAttack();

        yield return new WaitForSeconds(2.5f); // 지정한 초 만큼 쉬기
        animator.SetBool("Genesis_Stay", false);

        StartCoroutine(TelePort());
    }

    IEnumerator TelePort()
    {
        animator.SetTrigger("Teleport");
        yield return new WaitForSeconds(0.34f); // 지정한 초 만큼 쉬기

        rigid.constraints = RigidbodyConstraints2D.FreezeRotation; // 좌표 고정은 풀되, 회전은 못하게
        isAttackNow = false;

        yield return new WaitForSeconds(0.02f); // 지정한 초 만큼 쉬기

        isAttackNow = true;

        animator.SetTrigger("Teleport_arrive");

        yield return new WaitForSeconds(0.34f); // 지정한 초 만큼 쉬기

        isAttackNow = false;

        isAttackReady = true;
    }

    // IDamageable의 함수 TakeDamage
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
        //onBossWasKilled(this); // 대리자 호출

        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<CapsuleCollider2D>().enabled = false;

        isDead = true;

        yield return new WaitForSeconds(0.5f); // 지정한 초 만큼 쉬기

        Destroy(gameObject);
    }
}
