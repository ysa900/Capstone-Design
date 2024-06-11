using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    // 싱글톤 패턴을 사용하기 위한 인스턴스 변수
    private static Player _instance;

    // 키보드 방향키 입력을 위한 벡터
    public Vector2 inputVec;

    //[SerializeField]
    //// 플레이어 정보
    //public float speed;
    //public float hp;
    //public float maxHp;
    //public float Exp;
    //public int level;
    //public int[] nextExp;

    // 플레이어 패시브 효과 관련 변수
    //public float damageReductionValue = 1f; // 뎀감x
    //public float magnetRange; // 자석 범위

    //킬 수
    //public int kill;

    public bool isPlayerDead; // 플레이어가 죽었는지 판별하는 변수

    public bool isPlayerLookLeft; // 플레이어가 보고 있는 방향을 알려주는 변수

    public bool isPlayerShielded; // 플레이어가 보호막의 보호를 받고있냐

    // 플레이어 피격음 딜레이
    float hitDelayTime = 0.1f;
    float hitDelayTimer = 0.1f;

    // 플레이어 레벨업 함수 실행을 기다리게 하기 위한 변수
    public bool isSkillSelectComplete = true;

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

    public PlayerData playerData; // 플레이어 데이터


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        // 인스턴스가 존재하는 경우 새로생기는 인스턴스를 삭제한다.
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        // 아래의 함수를 사용하여 씬이 전환되더라도 선언되었던 인스턴스가 파괴되지 않는다.
        DontDestroyOnLoad(gameObject);

        // 변수 초기화
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        absorberCollider = gameObject.GetComponentInChildren<CircleCollider2D>();
    }

    public void Init()
    {
        isPlayerDead = false;
        isPlayerLookLeft = false;
        isPlayerShielded = false;
        hitDelayTimer = 0.1f;

        isSkillSelectComplete = true;

        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
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
        Vector2 nextVec = inputVec.normalized * playerData.speed * Time.fixedDeltaTime;

        // 입력받은 방향으로 플레이어 위치 설정
        rigid.MovePosition(rigid.position + nextVec);
    }

    //player 경험치 획득 함수
    public void GetExp(int expAmount)
    {
        playerData.Exp += expAmount;

        if (playerData.Exp >= playerData.nextExp[playerData.level])
            StartCoroutine(LevelUP()); // 레벨 업 함수 실행
    }

    // 자석 범위를 변경하는 함수
    public void ChangeMagnetRange()
    {
        absorberCollider.radius = playerData.magnetRange;
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
                    GameAudioManager.instance.PlaySfx(GameAudioManager.Sfx.Melee); // 피격  효과음
                    hitDelayTimer = 0;
                }
            }

            if (playerData.hp <= 0)
            {
                isPlayerDead = true;

                animator.SetBool("Dead", true);

                onPlayerWasKilled(this);

                rigid.constraints = RigidbodyConstraints2D.FreezeAll;
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
                GameAudioManager.instance.PlaySfx(GameAudioManager.Sfx.Melee); // 피격  효과음
            }

            if (playerData.hp <= 0)
            {
                isPlayerDead = true;

                animator.SetTrigger("Dead");

                onPlayerWasKilled(this);

                rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
    }

    IEnumerator LevelUP()
    {
        Debug.Log("레벨 업 기다리는 중");
        if (Time.timeScale != 0)
        {
            isSkillSelectComplete = true; // 오류 방지를 위한 코드
            Debug.Log("timeScale != 0, isSkillSelectComplete: " + isSkillSelectComplete);
        }
        yield return new WaitUntil(() => isSkillSelectComplete);

        onPlayerLevelUP(); // delegate 호출

        isSkillSelectComplete = false;
        Debug.Log("기다리기 끝, isSkillSelectComplete: "+ isSkillSelectComplete);

        playerData.Exp -= playerData.nextExp[playerData.level];
        playerData.level++;

        // 경험치를 경험치 통보다 많이 갖고있으면 재귀적으로 반복
        bool isAgain = playerData.Exp >= playerData.nextExp[playerData.level];
        if (isAgain) StartCoroutine(LevelUP());
    }

    public void RestoreHP(float restoreAmount)
    {
        playerData.hp += restoreAmount;
        playerData.hp = playerData.hp > 100 ? 100 : playerData.hp;
    }

    public void OnPlayerBlinded()
    {

    }
}