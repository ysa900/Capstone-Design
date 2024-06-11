using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static Unity.Barracuda.TextureAsTensorData;
using static UnityEngine.GraphicsBuffer;

public class Enemy : Object, IDamageable, IPoolingObject
{
    // Enemy 규칙: 모든 Enemy 프리팹은 오른쪽을 보는 게 기본

    // 플레이어 객체
    public Player player;

    // Enemy들 체력
    float[] enemy_HP = { 10, 30, 70, 100, 80, 150, 200, 300, 400, 500 };

    // enemy 정보
    public float hp;
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

    float agentToplayerDistance; // 적과 플레이어 사이의 거리

    bool isAgentShouldBeDisabled;


    // enemy가 죽었을 때 EnemyManager에게 알려주기 위한 delegate
    public delegate void OnEnemyWasKilled(Enemy killedEnemy, bool isKilledByPlayer);
    public OnEnemyWasKilled onEnemyWasKilled;

    // enemy 피격음을 EnemyManager가 총괄하도록 하는 delegate
    public delegate void OnEnemyHit();
    public OnEnemyHit onEnemyHit;

    public Rigidbody2D rigid; // 물리 입력을 받기위한 변수

    SpriteRenderer spriteRenderer; // 적 방향을 바꾸기 위해 flipX를 가져오기 위한 변수

    protected Animator animator; // 애니메이션 관리를 위한 변수

    public CapsuleCollider2D capsuleCollider; // Collider의 offset을 변경하기 위한 변수

    public NavMeshAgent agent; //Nav Mesh Agent 사용할 변수
    public bool isAgentDelay = false;


    public virtual void Init()
    {
        hp = enemy_HP[index];
        isDead = false;

        damageDelayTimer = 0;

        agent.enabled = true;

        float playerX = player.transform.position.x;
        float playerY = player.transform.position.y;

        sceneName = GameManager.instance.sceneName;

        // 몬스터가 스테이지에 맞게 소환되게 함
        switch (sceneName)
        {
            case "Stage1": // Stage1은 원형으로 소환 
                float radius = UnityEngine.Random.Range(20, 30);
                degree = UnityEngine.Random.Range(0f, 360f);

                float tmpX = (float)Math.Cos(degree * Mathf.Deg2Rad) * radius;
                float tmpY = (float)Math.Sin(degree * Mathf.Deg2Rad) * radius;

                X = tmpX + playerX;
                Y = tmpY + playerY;


                if (degree <= -360)
                {
                    degree %= -360;
                }

                break;

            case "Stage2": // Stage2는 좌우로만 소환
                tmpX = UnityEngine.Random.Range(20, 30);
                tmpY = UnityEngine.Random.Range(-26, 5.5f); // 맵 y축 범위로 제한

                if (UnityEngine.Random.Range(0, 2) == 1) tmpX = -tmpX; // 플레이어 기준 왼쪽, 오른쪽 랜덤

                X = tmpX + playerX;
                Y = tmpY;

                break;
            case "Stage3": // Stage3는 정해진 범위 안에 소환 (플레이어와 겹치지 않게)
                bool isPositionSameWithPlayer;
                int breakNum = 0; // while문 탈출을 위한 num

                do
                {
                    radius = UnityEngine.Random.Range(20, 25);
                    degree = UnityEngine.Random.Range(0f, 360f);

                    tmpX = (float)Math.Cos(degree * Mathf.Deg2Rad) * radius;
                    tmpY = (float)Math.Sin(degree * Mathf.Deg2Rad) * radius;

                    Vector2 playerPos = player.transform.position;
                    Vector2 myPos = new Vector2(tmpX, tmpY);
                    isPositionSameWithPlayer = Vector2.Distance(playerPos, myPos) < 5; 
                    
                    breakNum++;
                    if (breakNum >= 1000)// 1000회 반복 내에 마땅한 위치를 찾지 못했다면 그냥 break;
                    {
                        Debug.Log("1000번 내에 찾지 못함");
                        break;
                    }
                }
                while (isPositionSameWithPlayer);

                X = tmpX;
                Y = tmpY;

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
        agent = GetComponent<NavMeshAgent>();

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

        MoveIfToFar(); // 플레이어와의 거리가 너무 멀면 위치 이동
        damageDelayTimer += Time.fixedDeltaTime;
        /*   agent.enabled = true;*/
    }

    // 플레이어 방향으로 이동하는 함수
    protected void LookAtPlayer()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 myPosition = transform.position;

        Vector2 direction = playerPosition - myPosition;

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

        bool isFlipWeirdEnemy = tag == "Spitter" || tag == "Summoner";

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
        agentToplayerDistance = Vector3.Distance(player.transform.position, transform.position);

        if (agentToplayerDistance > 20f && sceneName == "Stage3")
        {
            agent.enabled = false;
            Vector2 playerPosition = player.transform.position;
            Vector2 myPosition = transform.position;

            Vector2 direction = playerPosition - myPosition;

            direction = direction.normalized;
            rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime);
        }
        else
        {
            if (!agent.enabled)
            {
                Vector2 playerPosition = player.transform.position;
                Vector2 myPosition = transform.position;

                Vector2 direction = playerPosition - myPosition;

                direction = direction.normalized;
                rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime);
                
                if(!isAgentShouldBeDisabled)
                    agent.enabled = true;

            }
            else
            {
                agent.enabled = true;
                agent.SetDestination(player.transform.position);
            }
        }

    }

    // 플레이어와의 거리가 너무 멀면 이동시키는 함수
    private void MoveIfToFar()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 myPosition = transform.position;

        float distance = Vector2.Distance(myPosition, playerPosition);

        bool isToFar = distance > 40f;

        if (isToFar)
        {
            agent.enabled = false;
            switch (sceneName)
            {
                case "Stage1": // 플레이어가 너무 멀리 가면 enemy를 플레이어를 중심으로 점 대칭 이동
                    float xDiff = myPosition.x - playerPosition.x;
                    float yDiff = myPosition.y - playerPosition.y;

                    // toFar 범위에 계속 걸치는 문제를 방지하기 위해 안쪽으로 넣어줌
                    xDiff = xDiff > 0 ? xDiff - 10 : xDiff + 10;
                    yDiff = yDiff > 0 ? yDiff - 10 : yDiff + 10;

                    Vector2 vector2 = new Vector2(playerPosition.x - xDiff, playerPosition.y - yDiff);
                    transform.position = vector2;

                    break;

                case "Stage2": // 플레이어가 너무 멀리 가면 enemy를 플레이어를 중심으로 y축 대칭 이동
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

        onEnemyHit(); // delegate 호출
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool shield_Flame = collision.GetComponent<PointEffector2D>() != null;
        if (shield_Flame)
        {
            float time = collision.gameObject.GetComponentInParent<Shield_Flame>().aliveTime;
            float timer = collision.gameObject.GetComponentInParent<Shield_Flame>().aliveTimer;

            isAgentShouldBeDisabled = true;
            agent.enabled = false;

            StartCoroutine(EnableAgent(time - timer + 0.2f));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (agent.enabled)
        {
            agent.velocity = Vector3.zero;
            rigid.velocity = Vector3.zero;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (agent.enabled)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                rigid.velocity = Vector3.zero;
            }
        }

        if (collision.gameObject.tag == "Obstacle" && agent.enabled)
        {
            agent.SetDestination(transform.position + new Vector3(-1, 3, 0));
            rigid.mass = 3.5f;
            isAgentDelay = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (agent.enabled)
        {
            agent.isStopped = false;
            agent.velocity = Vector3.zero;
            rigid.velocity = Vector3.zero;
        }


        if (collision.gameObject.tag == "Obstacle" && agent.enabled)
        {
            rigid.mass = 1.0f;
            isAgentDelay = false;
        }
    }
    
    IEnumerator Dead()
    {
        isDead = true;

        agent.enabled = false;

        animator.SetTrigger("Dead");

        StopCoroutine(StartCoroutine(EnableAgent(2f)));

        if (!isTimeOver)
        {
            onEnemyWasKilled(this, true); // 대리자 호출
        }

        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<CapsuleCollider2D>().enabled = false;

        yield return new WaitForSeconds(0.5f); // 지정한 초 만큼 쉬기

        GameManager.instance.poolManager.ReturnEnemy(this, index);
    }

    public IEnumerator makeEnemyHardPattern()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        hp *= 2f;

        yield return new WaitForSeconds(30f); // 지정한 초 뒤에 패턴 끄기

        transform.GetChild(0).gameObject.SetActive(false);

        // Enemy Hp 보정
        if (tag == "EvilTree")
        {
            if (hp >= enemy_HP[0])
            {
                hp = enemy_HP[0];
            }
        }
        else if (tag == "Pumpkin")
        {
            if (hp >= enemy_HP[1])
            {
                hp = enemy_HP[1];
            }
        }
        else if (tag == "Warlock")
        {
            if (hp >= enemy_HP[2])
            {
                hp = enemy_HP[2];
            }
        }
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

    IEnumerator EnableAgent(float time)
    {
        yield return new WaitForSeconds(time);

        isAgentShouldBeDisabled = false;
        agent.enabled = true;
    }

}