using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Player_ML : MonoBehaviour
{

    public bool isPlayerDead; // 플레이어가 죽었는지 판별하는 변수

    public bool isPlayerLookLeft; // 플레이어가 보고 있는 방향을 알려주는 변수

    public bool isPlayerShielded; // 플레이어가 보호막의 보호를 받고있냐

    Rigidbody2D rigid; // 물리 입력을 받기위한 변수
    SpriteRenderer spriteRenderer; // 플레이어 방향을 바꾸기 위해 flipX를 가져오기 위한 변수
    Animator animator; // 애니메이션 관리를 위한 변수

    // 플레이어가 죽었을 시 GameManager에게 알려주기 위한 delegate
    public delegate void OnPlayerWasKilled(Player_ML player_ML);
    public OnPlayerWasKilled onPlayerWasKilled;

    // 플레이어가 레벨업 했을 때 GameManager에게 알려주기 위한 delegate
    public delegate void OnPlayerLevelUP();
    public OnPlayerLevelUP onPlayerLevelUP;

    private GameAudioManager gameAudioManager;

    public PlayerData playerData; // 플레이어 데이터
    public NavMeshAgent agent;
    private Enemy_ML enemy_ML;

    void Awake()
    {
        // 변수 초기화
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        gameAudioManager = FindAnyObjectByType<GameAudioManager>();

        rigid.constraints = RigidbodyConstraints2D.None;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.velocity = Vector3.zero;
        rigid.velocity = Vector3.zero;
    }



    void FixedUpdate()
    {
        // ReceiveDirectionInput(); // 키보드 방향키 입력을 가져오는 함수
        MovePlayer();

    }

    // 프레임이 끝나기 직전에 실행되는 함수
    private void LateUpdate()
    {
        animator.SetFloat("Speed", agent.speed); // animator의 float타입인 변수 Speed를 inpuVec의 크기만큼으로 설정한다

        // isPlayerLookLeft = inputVec.x < 0; // 플레이어가 왼쪽을 보고 있으면

        // if (inputVec.x != 0) // 키를 안눌렀을 때는 실행 안되도록 하기 위해 inputVec.x가 0이 아닌 경우만 실행하게 한다
        // {
        //     spriteRenderer.flipX = isPlayerLookLeft; // 플레이어를 x축으로 뒤집는다
        // }
        // else
        // {
        //     isPlayerLookLeft = spriteRenderer.flipX;
        // }
    }

    // 플레이어를 움직이는 함수
    private void MovePlayer()
    {
        // 플레이어의 방향벡터를 가져와서 속도를 설정
        // fixedDeltaTime은 물리 프레임 시간
        Vector3 nextVec = (transform.position - enemy_ML.transform.position).normalized * playerData.speed * Time.fixedDeltaTime;

        agent.SetDestination(agent.transform.position + nextVec);

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
}