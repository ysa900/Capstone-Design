using UnityEngine;

public class Arrow : Object, IPoolingObject, IDamageableSkill
{
    public Player player;

    // Arrow ����
    [SerializeField] float speed;
    [SerializeField] float damage;
    [SerializeField] float hp;
    [SerializeField] float maxhp;

    private float aliveTimer; // ��ų ���� �ð��� üũ�� ����
    private float aliveTime = 5f;
    int returnIndex; // Ǯ�� �� ���� �ε���

    Rigidbody2D rigid; // ���� �Է��� �ޱ����� ����

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

    // ȭ���� ���ư� �÷��̾� ����(Direction) ����
    private void SetPlayerPosition()
    {
        playerPosition = player.transform.position;
        myPosition = transform.position;
        direction = playerPosition - myPosition;
        direction = direction.normalized;
    }

    // ȭ�� ����(rotation) ���� (�÷��̾� �ٶ󺸰�)
    private void SetArrowDirection()
    {
        Vector2 direction = new Vector2(playerPosition.x - myPosition.x, playerPosition.y - myPosition.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion angleAxis = Quaternion.AngleAxis(angle + 180f, Vector3.forward);
        Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5f);
        transform.rotation = rotation;
    }

    // �÷��̾ ���󰡴� ��ų
    private void MoveToPlayer()
    {
        rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime); // Player �������� ��ġ ����

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
