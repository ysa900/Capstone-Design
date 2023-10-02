using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 키보드 방향키 입력을 위한 벡터
    public Vector2 inputVec;

    [SerializeField]
    // 플레이어 정보
    public float speed;
    public float hp;

    private bool isPlayerDead;

    Rigidbody2D rigid; // 물리 입력을 받기위한 변수
    SpriteRenderer spriteRenderer; // 플레이어 방향을 바꾸기 위해 flipX를 가져오기 위한 변수
    Animator animator; // 애니메이션 관리를 위한 변수

    // 플레이어가 죽었을 시 GameManager에게 알려주기 위한 delegate
    public delegate void OnPlayerWasKilled(Player player);
    public OnPlayerWasKilled onPlayerWasKilled;

    void Start()
    {
        // 변수 초기화
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ReceiveDirectionInput();
    }

    // 물리 연산 프레임마다 호출되는 생명주기 함수
    private void FixedUpdate()
    {
        MovePlayer();
    }

    // 프레임이 끝나기 직전에 실행되는 함수
    private void LateUpdate()
    {
        animator.SetFloat("Speed", inputVec.magnitude); // animator의 float타입인 변수 Speed를 inpuVec의 크기만큼으로 설정한다

        bool isPlayerLookLeft = inputVec.x < 0; // 플레이어가 왼쪽을 보고 있으면

        if (inputVec.x != 0) // 키를 안눌렀을 때는 실행 안되도록 하기 위해 inputVec.x가 0이 아닌 경우만 실행하게 한다
        {
            spriteRenderer.flipX = isPlayerLookLeft; // 플레이어를 x축으로 뒤집는다
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

    // 플레이어가 무언가와 충돌하면 데미지를 입는다
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isPlayerDead)
        {
            hp -= Time.deltaTime * 10;

            if (hp < 0)
            {
                isPlayerDead = true;

                animator.SetBool("Dead", true);

                onPlayerWasKilled(this);

                rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
    }
}
