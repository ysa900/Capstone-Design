using Unity.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    // 키보드 방향키 입력을 위한 벡터
    public Vector2 inputVec;

    [SerializeField]
    // 플레이어 정보
    public float speed;
    public float hp;
    public float maxHp = 100;
    public int Exp;
    public int level;
    public int[] nextExp;

    // 플레이어 패시브 효과 관련 변수
    public float damageReductionValue = 1f; // 뎀감
    public float magnetRange; // 자석 범위

    //킬 수
    public int kill;

    public bool isPlayerDead; // 플레이어가 죽었는지 판별하는 변수

    public bool isPlayerLookLeft; // 플레이어가 보고 있는 방향을 알려주는 변수

    public bool isPlayerShielded; // 플레이어가 보호막의 보호를 받고있냐

    // 플레이어 피격음 딜레이
    float hitDelayTime = 0.1f;
    float hitDelayTimer = 0.1f;

    Rigidbody2D rigid; // 물리 입력을 받기위한 변수
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

    private void Awake()
    {
        gameAudioManager = FindAnyObjectByType<GameAudioManager>();
    }

    void Start()
    {
        // 변수 초기화
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        absorberCollider = gameObject.GetComponentInChildren<CircleCollider2D>();

        nextExp = new int[100];
        int num = 0;
        for (int i = 0; i < nextExp.Length; i++)
        {
            if (level >= 30)
            {
                num += 100;
                nextExp[i] = num;


            }
            else
            {
                num += 5;
                nextExp[i] = num;

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ReceiveDirectionInput(); // 키보드 방향키 입력을 가져오는 함수
    }

    // 물리 연산 프레임마다 호출되는 생명주기 함수
    private void FixedUpdate()
    {
        MovePlayer();

        hitDelayTimer += Time.fixedDeltaTime;
    }

    // 프레임이 끝나기 직전에 실행되는 함수
    private void LateUpdate()
    {
        animator.SetFloat("Speed", inputVec.magnitude); // animator의 float타입인 변수 Speed를 inpuVec의 크기만큼으로 설정한다

        isPlayerLookLeft = inputVec.x < 0; // 플레이어가 왼쪽을 보고 있으면

        if (inputVec.x != 0) // 키를 안눌렀을 때는 실행 안되도록 하기 위해 inputVec.x가 0이 아닌 경우만 실행하게 한다
        {
            spriteRenderer.flipX = isPlayerLookLeft; // 플레이어를 x축으로 뒤집는다
        }
        else
        {
            isPlayerLookLeft = spriteRenderer.flipX;
        }
    }

    // 키보드 방향키 입력을 가져오는 함수
    private void ReceiveDirectionInput()
    {
        // 수평, 수직 방향 입력을 받는다
        // inputmanager에 기본 설정돼있다
        // GetAxisRaw를 해야 더욱 명확한 컨트롤 가능
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
    }

    // 플레이어를 움직이는 함수
    private void MovePlayer()
    {
        // 플레이어의 방향벡터를 가져와서 속도를 설정
        // fixedDeltaTime은 물리 프레임 시간
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;

        // 입력받은 방향으로 플레이어 위치 설정
        rigid.MovePosition(rigid.position + nextVec);
    }

    //player 경험치 획득 함수
    public void GetExp(int expAmount)
    {
        Exp += expAmount;

        if (Exp >= nextExp[level])
        {
            onPlayerLevelUP(); // delegate 호출

            level++;
            Exp = 0;
        }

    }

    // 자석 범위를 변경하는 함수
    public void ChangeMagnetRange()
    {
        absorberCollider.radius = magnetRange;
    }

    // 플레이어가 무언가와 충돌하면 데미지를 입는다
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Obstacle") // 장애물과 충돌한거면 데미지 안입음 
            return;

        if (!isPlayerDead)
        {
            if (!isPlayerShielded)
            {
                hp -= Time.deltaTime * 5 * damageReductionValue;

                bool isHitDelayOK = hitDelayTimer >= hitDelayTime;
                if (isHitDelayOK)
                {
                    gameAudioManager.PlaySfx(GameAudioManager.Sfx.Melee); // 피격  효과음
                    hitDelayTimer = 0;
                }
            }

            if (hp <= 0)
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
                hp -= damage * damageReductionValue;
                gameAudioManager.PlaySfx(GameAudioManager.Sfx.Melee); // 피격  효과음
            }

            if (hp <= 0)
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