using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : Object, IDamageable, IPullingObject
{
    // 플레이어 객체
    public Player player;

    // Enemy들 체력
    int[] enemy_HP = { 15, 50, 150, 300, 15, 50, 150, 300 };

    // enemy 정보
    public int hp;
    public float speed;
    private float colliderOffsetX; // collider의 offset x좌표
    private float colliderOffsetY; // collider의 offset y좌표

    public bool isEnemyLookLeft; // 적이 보고 있는 방향을 알려주는 변수

    private bool isDead;
    private bool isTimeOver;

    private float damageDelay = 2f;
    private float damageDelayTimer = 0;

    private float degree = 0;

    public int index; // Enemy 종류

    int sceneNum;

    // enemy가 죽었을 때 EnemyManager에게 알려주기 위한 delegate
    public delegate void OnEnemyWasKilled(Enemy killedEnemy, bool isKilledByPlayer);
    public OnEnemyWasKilled onEnemyWasKilled;

    public Rigidbody2D rigid; // 물리 입력을 받기위한 변수

    SpriteRenderer spriteRenderer; // 적 방향을 바꾸기 위해 flipX를 가져오기 위한 변수

    Animator animator; // 애니메이션 관리를 위한 변수

    public CapsuleCollider2D capsuleCollider; // Collider의 offset을 변경하기 위한 변수

    public void Init()
    {
        hp = enemy_HP[index];
        isDead = false;

        damageDelayTimer = 0;

        float playerX = player.transform.position.x;
        float playerY = player.transform.position.y;

        sceneNum = GameManager.instance.sceneNum;

        // 몬스터가 스테이지에 맞게 소환되게 함
        switch (sceneNum)
        {
            case 2: // GameScene은 원형으로 소환
                float radius = UnityEngine.Random.Range(20, 30);
                degree = UnityEngine.Random.Range(0f, 360f);

                float tmpX = (float)Math.Cos(degree) * radius;
                float tmpY = (float)Math.Sin(degree) * radius;

                X = tmpX + playerX;
                Y = tmpY + playerY;

                if (degree <= -360)
                {
                    degree %= -360;
                }

                break;

            case 3:// Stage2는 좌우로만 소환
                tmpX = UnityEngine.Random.Range(20, 30);
                tmpY = UnityEngine.Random.Range(-26, 5.5f); // 맵 y축 범위로 제한

                if (UnityEngine.Random.Range(0, 2) == 1) tmpX = -tmpX; // 플레이어 기준 왼쪽, 오른쪽 랜덤

                X = tmpX + playerX;
                Y = tmpY;

                break;
        }
        
        rigid.constraints = RigidbodyConstraints2D.None;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        GetComponent<CapsuleCollider2D>().enabled = true;
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        colliderOffsetX = capsuleCollider.offset.x; // offset 초기값을 저장
        colliderOffsetY = capsuleCollider.offset.y;
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            MoveToPlayer();
        }

        isTimeOver = GameManager.instance.gameTime >= GameManager.instance.maxGameTime;
        if (isTimeOver && !isDead)
        {
            StartCoroutine(Dead());
        }

        DestryIfToFar(); // 플레이어와의 거리가 너무 멀면 죽음
        damageDelayTimer += Time.fixedDeltaTime;
    }

    // 플레이어 방향으로 이동하는 함수
    private void MoveToPlayer()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 myPosition = transform.position;

        Vector2 direction = playerPosition - myPosition;

        if (Math.Abs(direction.x) >= 0.5f)
        {
            isEnemyLookLeft = direction.x < 0;
        }

        spriteRenderer.flipX = isEnemyLookLeft;

        Vector2 colliderOffset; // CapsuleCollider의 offset에 넣을 Vector2

        if (isEnemyLookLeft) // Enemy가 왼쪽을 보면 collider도 x축 대칭을 해준다
        {
            colliderOffset = new Vector2(-colliderOffsetX, colliderOffsetY);
        }
        else
        {
            colliderOffset = new Vector2(colliderOffsetX, colliderOffsetY);

        }
        capsuleCollider.offset = colliderOffset; // capsuleCollider에 적용

        direction = direction.normalized;
        rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime); // 플레이어 방향으로 위치 변경

        X = transform.position.x;
        Y = transform.position.y;
    }

    // 플레이어와의 거리가 너무 멀면 죽는 함수
    private void DestryIfToFar()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 myPosition = transform.position;

        float distance = Vector2.Distance(myPosition, playerPosition);

        bool isToFar = distance > 40f;

        if (isToFar)
        {
            switch (sceneNum)
            {
                case 2: // 플레이어가 너무 멀리 가면 enemy를 플레이어를 중심으로 점 대칭 이동
                    float xDiff = myPosition.x - playerPosition.x;
                    float yDiff = myPosition.y - playerPosition.y;

                    // toFar 범위에 계속 걸치는 문제를 방지하기 위해 안쪽으로 넣어줌
                    xDiff = xDiff > 0 ? xDiff - 10 : xDiff + 10;
                    yDiff = yDiff > 0 ? yDiff - 10 : yDiff + 10;

                    Vector2 vector2 = new Vector2(playerPosition.x - xDiff, playerPosition.y - yDiff);
                    transform.position = vector2;

                    break;

                case 3: // 플레이어가 너무 멀리 가면 enemy를 플레이어를 중심으로 y축 대칭 이동
                    xDiff = myPosition.x - playerPosition.x;

                    // toFar 범위에 계속 걸치는 문제를 방지하기 위해 안쪽으로 넣어줌
                    xDiff = xDiff > 0 ? xDiff - 10 : xDiff + 10;

                    vector2 = new Vector2(playerPosition.x - xDiff, Y);
                    transform.position = vector2;

                    break;
            }
            
        }

    }

    // IDamageable의 함수 TakeDamage
    public void TakeDamage(GameObject causer, float damage)
    {
        hp = hp - (int)damage;

        ShowDamageText(damage, causer.tag); // damageText 출력

        if (hp <= 0 && !isDead)
        {
            StartCoroutine(Dead());
        }
        else
        {
            if (damageDelay <= damageDelayTimer)
            {
                animator.SetTrigger("Hit");
                damageDelayTimer = 0;
            }
        }
    }

    IEnumerator Dead()
    {
        isDead = true;

        animator.SetTrigger("Dead");

        if (!isTimeOver)
        {
            onEnemyWasKilled(this, true); // 대리자 호출
        }

        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<CapsuleCollider2D>().enabled = false;

        yield return new WaitForSeconds(0.5f); // 지정한 초 만큼 쉬기

        GameManager.instance.poolManager.ReturnEnemy(this, index);
    }

    // damageText 출력
    void ShowDamageText(float damage, string skillTag)
    {
        GameObject hudText = GameManager.instance.poolManager.GetText((int)damage, skillTag);

        float ranNumX = UnityEngine.Random.Range(-0.5f, 0.5f);
        float ranNumY = UnityEngine.Random.Range(1.0f, 2.0f);

        Vector3 vector3 = new Vector3(transform.position.x + ranNumX, transform.position.y + ranNumY, 0);
        hudText.transform.position = vector3;
    }

}