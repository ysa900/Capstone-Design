using UnityEngine;

public class Arrow : Object, IPoolingObject, IDamageableSkill
{
    public Player player;

    // Arrow 정보
    [SerializeField] float speed;
    [SerializeField] float damage;
    [SerializeField] float hp;
    [SerializeField] float maxhp;

    private float aliveTimer; // 스킬 생존 시간을 체크할 변수
    private float aliveTime = 5f;
    int returnIndex; // 풀링 시 리턴 인덱스

    Rigidbody2D rigid; // 물리 입력을 받기위한 변수

    Vector2 playerPosition;
    Vector2 myPosition;
    Vector2 direction;

    public void Init()
    {
        hp = maxhp;
        aliveTimer = 0;

        SetPlayerPosition();
        SetArrowDirection();
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameManager.instance.player;

        Init();
    }

    private void FixedUpdate()
    {
        bool destroyArrow = aliveTimer > aliveTime;

        if (destroyArrow)
        {
            GameManager.instance.poolManager.ReturnArrow(this);
            return;
        }
        else
        {
            MoveToPlayer();
        }

        aliveTimer += Time.fixedDeltaTime;
    }

    // 화살이 날아갈 플레이어 방향(Direction) 설정
    private void SetPlayerPosition()
    {
        playerPosition = player.transform.position;
        myPosition = transform.position;
        direction = playerPosition - myPosition;
        direction = direction.normalized;
    }

    // 화살 방향(rotation) 보정 (플레이어 바라보게)
    private void SetArrowDirection()
    {
        Vector2 direction = new Vector2(playerPosition.x - myPosition.x, playerPosition.y - myPosition.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion angleAxis = Quaternion.AngleAxis(angle + 180f, Vector3.forward);
        Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5f);
        transform.rotation = rotation;
    }

    // 플레이어를 따라가는 스킬
    private void MoveToPlayer()
    {
        rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime); // Player 방향으로 위치 변경

        X = transform.position.x;
        Y = transform.position.y;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IPlayer iPlayer = collision.GetComponent<IPlayer>();

        if (iPlayer == null)
        {
            return;
        }

        iPlayer.TakeDamage(damage);
        GameManager.instance.poolManager.ReturnArrow(this);
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            GameManager.instance.poolManager.ReturnArrow(this);
        }
    }
}
