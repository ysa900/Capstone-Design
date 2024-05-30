using Unity.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.AI;
using System.Collections.Generic;

public class Player : Agent, IPlayer
{
    public NavMeshAgent navAgent; // Navmesh

    public Rigidbody2D rigid;
    public bool isPlayerDead; // 플레이어가 죽었는지 판별하는 변수
    public bool isPlayerLookLeft; // 플레이어가 보고 있는 방향을 알려주는 변수
    public bool isPlayerShielded; // 플레이어가 보호막의 보호를 받고있냐

    // 플레이어 피격음 딜레이
    float hitDelayTime = 0.1f;
    float hitDelayTimer = 0.1f;

    SpriteRenderer spriteRenderer; // 플레이어 방향을 바꾸기 위해 flipX를 가져오기 위한 변수
    Animator animator; // 애니메이션 관리를 위한 변수
    CircleCollider2D absorberCollider; // Absorber의 Collider - 자석 효과 범위를 바꾸기 위한 변수

    // 플레이어가 죽었을 시 GameManager에게 알려주기 위한 delegate
    public delegate void OnPlayerWasKilled(Player player);
    public OnPlayerWasKilled onPlayerWasKilled;

    // 플레이어가 레벨업 했을 때 GameManager에게 알려주기 위한 delegate
    public delegate void OnPlayerLevelUP();
    public OnPlayerLevelUP onPlayerLevelUP;

    private GameAudioManager gameAudioManager;

    public PlayerData playerData; // 플레이어 데이터
    public MeleeEnemy enemy; 

    private void Awake()
    {
        gameAudioManager = FindAnyObjectByType<GameAudioManager>();
        
    }

    void Start()
    {
        // 변수 초기화
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        absorberCollider = gameObject.GetComponentInChildren<CircleCollider2D>();
        navAgent = GetComponent<NavMeshAgent>();
        enemy = FindAnyObjectByType<MeleeEnemy>();
        rigid = GetComponent<Rigidbody2D>();
        
    }

    void Update()
    {
    }

    // 물리 연산 프레임마다 호출되는 생명주기 함수
    private void FixedUpdate()
    {
        
        hitDelayTimer += Time.fixedDeltaTime;
    }
    
    // 프레임이 끝나기 직전에 실행되는 함수
    private void LateUpdate()
    {
        animator.SetFloat("Speed", speed); // animator의 float타입인 변수 Speed를 inpuVec의 크기만큼으로 설정한다

        isPlayerLookLeft = nextMove.x < 0; // 플레이어가 왼쪽을 보고 있으면

        if (nextMove.x != 0) // 키를 안눌렀을 때는 실행 안되도록 하기 위해 inputVec.x가 0이 아닌 경우만 실행하게 한다
        {
            spriteRenderer.flipX = isPlayerLookLeft; // 플레이어를 x축으로 뒤집는다
        }
        else
        {
            isPlayerLookLeft = spriteRenderer.flipX;
        }
    }
    public override void OnEpisodeBegin()
    {
        transform.position = Vector2.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(enemy.transform.position);
        
    }

    [SerializeField] float speed = 1;
    public Vector2 nextMove;
    public override void OnActionReceived(ActionBuffers actions)
    {
        nextMove.x = actions.ContinuousActions[0];
        nextMove.y = actions.ContinuousActions[1];

        nextMove.x -= enemy.transform.position.x;
        nextMove.y -= enemy.transform.position.y;

        navAgent.SetDestination(nextMove * Time.deltaTime * speed);
    }
    private void OnTriggerEnter(Collider other) 
    {
        if  ( other.transform == enemy )    
        {
            SetReward(-1);
            EndEpisode();
        }
        else if ( GameManager.instance.gameTime == 10f)
        {
            SetReward(+1);
            EndEpisode();
        }
    }

    // 자석 범위를 변경하는 함수
    public void ChangeMagnetRange()
    {
        absorberCollider.radius = playerData.magnetRange;
    }

    //player 경험치 획득 함수
    public void GetExp(int expAmount)
    {
        playerData.Exp += expAmount;

        if (playerData.Exp >= playerData.nextExp[playerData.level])
        {
            onPlayerLevelUP(); // delegate 호출

            playerData.level++;
            playerData.Exp = 0;
        }
    }

    // 플레이어가 몬스터와 충돌하면 데미지를 입는다
    private void OnCollisionStay2D(Collision2D collision)
    {
        bool isNotDamageObject = collision.gameObject.tag == "Obstacle" ||
            collision.gameObject.tag == "ClearWall";

        if (isNotDamageObject) // 장애물과 충돌한거면 데미지 안입음 
            return;

        if (!isPlayerDead)
        {
            if (!isPlayerShielded)
            {
                playerData.hp -= Time.deltaTime * 5 * playerData.damageReductionValue;

                bool isHitDelayOK = hitDelayTimer >= hitDelayTime;
                if (isHitDelayOK)
                {
                    gameAudioManager.PlaySfx(GameAudioManager.Sfx.Melee); // 피격  효과음
                    hitDelayTimer = 0;
                }
            }

            if (playerData.hp <= 0)
            {
                isPlayerDead = true;

                animator.SetBool("Dead", true);

                onPlayerWasKilled(this);

                rigid.constraints = RigidbodyConstraints2D.FreezeAll;

                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); // 죽었을 때 나오는 묘비 크기 때문에 크기 조정 해준 것
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isPlayerDead)
        {
            if (!isPlayerShielded)
            {
                playerData.hp -= damage * playerData.damageReductionValue;
                gameAudioManager.PlaySfx(GameAudioManager.Sfx.Melee); // 피격  효과음
            }

            if (playerData.hp <= 0)
            {
                isPlayerDead = true;

                animator.SetBool("Dead", true);

                onPlayerWasKilled(this);

                rigid.constraints = RigidbodyConstraints2D.FreezeAll;

                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); // 죽었을 때 나오는 묘비 크기 때문에 크기 조정 해준 것
            }
        }
    }

    public void OnPlayerBlinded()
    {

    }
}