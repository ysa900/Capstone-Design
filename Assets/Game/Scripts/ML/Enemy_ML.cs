using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Unity.Barracuda.TextureAsTensorData;

public class Enemy_ML : Object, IDamageable, IPoolingObject
{
    // Enemy 규칙: 모든 Enemy 프리팹은 오른쪽을 보는 게 기본
    public Player_ML player_ML; // 플레이어 ML

    // Enemy들 체력
    int[] enemy_HP = { 15, 50, 80, 15, 50, 70, 15, 50, 80, 150 };

    // enemy 정보
    public int hp;
    public float speed;
    private float colliderOffsetX; // collider의 offset x좌표
    private float colliderOffsetY; // collider의 offset y좌표

    public bool isEnemyLookLeft; // 적이 보고 있는 방향을 알려주는 변수
    bool isFlipWeirdEnemyFlippedOnce; // flip 할 때 이상한 적이 한번이라도 flip 됐는 지

    protected bool isDead;
    private bool isTimeOver;

    private float damageDelay = 2f;
    private float damageDelayTimer = 0;

    private float degree = 0;

    public int index; // Enemy 종류

    string sceneName;

    // enemy가 죽었을 때 EnemyManager에게 알려주기 위한 delegate
    public delegate void OnEnemyWasKilled(Enemy_ML killedEnemy, bool isKilledByPlayer);
    public OnEnemyWasKilled onEnemyWasKilled;

    public Rigidbody2D rigid; // 물리 입력을 받기위한 변수

    SpriteRenderer spriteRenderer; // 적 방향을 바꾸기 위해 flipX를 가져오기 위한 변수

    protected Animator animator; // 애니메이션 관리를 위한 변수

    public CapsuleCollider2D capsuleCollider; // Collider의 offset을 변경하기 위한 변수

    public NavMeshAgent agent;

    public virtual void Init()
    {
        hp = enemy_HP[index];
        isDead = false;

        damageDelayTimer = 0;

        float playerMLX = player_ML.transform.position.x;
        float playerMLY = player_ML.transform.position.y;

        sceneName = GameManager.instance.sceneName;

        // 몬스터가 스테이지에 맞게 소환되게 함
        switch (sceneName)
        {
            // ML Stage1
             case "Stage1_ML":
                float radius = UnityEngine.Random.Range(20, 30);
                degree = UnityEngine.Random.Range(0f, 360f);

                float tmpX = (float)Math.Cos(degree) * radius;
                float tmpY = (float)Math.Sin(degree) * radius;

                X = tmpX + playerMLX;
                Y = tmpY + playerMLY;


                if (degree <= -360)
                {
                    degree %= -360;
                }
                break;
        }

        rigid.constraints = RigidbodyConstraints2D.None;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        GetComponent<CapsuleCollider2D>().enabled = true;
    }

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        agent = GetComponent<NavMeshAgent> ();


        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.velocity = Vector3.zero;
        rigid.velocity = Vector3.zero;

        colliderOffsetX = capsuleCollider.offset.x; // offset 초기값을 저장
        colliderOffsetY = capsuleCollider.offset.y;
    }

    protected virtual void FixedUpdate()
    {
        isTimeOver = GameManager.instance.gameTime >= GameManager.instance.maxGameTime;
    

        if (isTimeOver && !isDead)
        {
            StartCoroutine(Dead());
        }

        DestryIfTooFar(); // 플레이어와의 거리가 너무 멀면 죽음
        damageDelayTimer += Time.fixedDeltaTime;
        agent.enabled = true;
    }

    // 플레이어 방향으로 이동하는 함수
    protected void LookAtPlayer()
    {
        Vector2 playerMLPosition = player_ML.transform.position;
        Vector2 myPosition = transform.position;

        Vector2 direction = playerMLPosition - myPosition;

        // 이미 플레이어를 바라보고 있다면 return
        bool isLookAtCorrectly = direction.x < 0 == spriteRenderer.flipX;
        if (isLookAtCorrectly) return;

        if (Math.Abs(direction.x) >= 0.5f)
        {
            isEnemyLookLeft = direction.x < 0;
        }

        bool isNotFlip = spriteRenderer.flipX == isEnemyLookLeft; // Flip 안 해도 되냐
        spriteRenderer.flipX = isEnemyLookLeft;

        if (isNotFlip) return; // Flip 안 해도 되면 리턴

        Vector2 colliderOffset; // CapsuleCollider의 offset에 넣을 Vector2

        bool isFlipWeirdEnemy = tag == "Spitter" || tag == "Summoner"; // 움직임 어색한 프리팹들

        if (isEnemyLookLeft) // Enemy가 왼쪽을 보면 collider도 x축 대칭을 해준다
        {
            colliderOffset = new Vector2(-colliderOffsetX, colliderOffsetY);

            if (isFlipWeirdEnemy) // flip이 이상해서 보정해 줘야 되는 적이면 보정해 줌
            {
                X += 1.82f;
                isFlipWeirdEnemyFlippedOnce = true;
            }
        }
        else
        {
            colliderOffset = new Vector2(colliderOffsetX, colliderOffsetY);

            // flip이 이상해서 보정해 줘야 되는 적이면 보정해 줌
            if (isFlipWeirdEnemyFlippedOnce && isFlipWeirdEnemy)
            {
                X -= 1.82f;
            }
        }

        capsuleCollider.offset = colliderOffset; // capsuleCollider에 적용
    }

    // 플레이어 방향으로 이동하는 함수
    protected void MoveToPlayer()
    {

        agent.SetDestination(player_ML.transform.position);
    }

    // 플레이어와의 거리가 너무 멀면 죽는 함수
    private void DestryIfTooFar()
    {
        Vector2 playerMLPosition = player_ML.transform.position;
        Vector2 myPosition = transform.position;

        float distance = Vector3.Distance(myPosition, playerMLPosition);

        bool isTooFar = distance > 40f;

        if (isTooFar)
        {
            agent.enabled = false;
            switch (sceneName)
            {
                case "Stage1": // 플레이어가 너무 멀리 가면 enemy를 플레이어를 중심으로 점 대칭 이동
                    float xDiff = myPosition.x - playerMLPosition.x;
                    float yDiff = myPosition.y - playerMLPosition.y;

                    // toFar 범위에 계속 걸치는 문제를 방지하기 위해 안쪽으로 넣어줌
                    xDiff = xDiff > 0 ? xDiff - 10 : xDiff + 10;
                    yDiff = yDiff > 0 ? yDiff - 10 : yDiff + 10;

                    Vector2 vector2 = new Vector2(playerMLPosition.x - xDiff, playerMLPosition.y - yDiff);
                    transform.position = vector2;

                    break;

                case "Stage2": // 플레이어가 너무 멀리 가면 enemy를 플레이어를 중심으로 y축 대칭 이동
                    xDiff = myPosition.x - playerMLPosition.x;

                    // toFar 범위에 계속 걸치는 문제를 방지하기 위해 안쪽으로 넣어줌
                    xDiff = xDiff > 0 ? xDiff - 10 : xDiff + 10;

                    vector2 = new Vector2(playerMLPosition.x - xDiff, Y);
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        agent.velocity = Vector3.zero;
        rigid.velocity = Vector3.zero;
    }



    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            rigid.velocity = Vector3.zero;
        }  
     
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        agent.isStopped = false;
        agent.velocity = Vector3.zero;
        rigid.velocity = Vector3.zero;
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

        GameManager.instance.poolManager_ML.ReturnEnemy(this, index);
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