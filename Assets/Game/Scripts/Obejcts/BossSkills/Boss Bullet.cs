using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Boss_Bullet : Object
{
    public Player player;
    public Transform target;

    public float damage = 10f;

    bool isDead;
    bool isOnLeftSide;

    private float speed = 3;

    private float aliveTime = 10f;
    private float aliveTimer; // 스킬 생존 시간을 체크할 변수

    Animator animator;
    Rigidbody2D rigid; // 물리 입력을 받기위한 변수
    SpriteRenderer spriteRenderer;
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (!(GameManager.instance.boss == null))
        {
            bool isBossLookLeft = GameManager.instance.boss.isBossLookLeft;

            float bulletX = GameManager.instance.boss.transform.position.x;
            float bulletY = GameManager.instance.boss.transform.position.y;

            if (isBossLookLeft)
            {
                bulletX -= 2.5f;
            }
            else
            {
                bulletX += 2.5f;
            }

            bulletY -= 3f;

            transform.position = new Vector2(bulletX, bulletY);

            player = GameManager.instance.player;
            target = player.GetComponent<Transform>();

            animator = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            target = player.GetComponent<Transform>();
            Vector2 direction = new Vector2(transform.position.x - target.position.x, transform.position.y - target.position.y);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5f);
            transform.rotation = rotation;

            isOnLeftSide = Mathf.Cos(angle * Mathf.Deg2Rad) < 0; // cos값이 -면 플레이어를 기준으로 왼쪽에 있는 것

            spriteRenderer.flipY = isOnLeftSide;

            rigid.MovePosition(rigid.position - direction.normalized * speed * Time.fixedDeltaTime); // 플레이어 방향으로 위치 변경

            X = transform.position.x;
            Y = transform.position.y;

            if (aliveTimer > aliveTime)
            {
                StartCoroutine(Dead());
            }

            aliveTimer += Time.fixedDeltaTime;
        }

    }

    IEnumerator Dead()
    {
        animator.SetTrigger("Hit");

        isDead = true;

        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<CapsuleCollider2D>().enabled = false;

        yield return new WaitForSeconds(0.35f); // 지정한 초 만큼 쉬기

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IPlayer iPlayer = collision.GetComponent<IPlayer>();

        if (iPlayer == null)
        {
            return;
        }

        iPlayer.TakeDamage(damage);

        StartCoroutine(Dead());
    }
}

