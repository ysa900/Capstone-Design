using UnityEngine;
using UnityEngine.UIElements;

public class RandomSkill : Skill
{
    public RandomSkill randomSkill;

    public float impactPonitX;
    public float impactPonitY;

    private float aliveTimer; // 스킬 생존 시간을 체크할 변수
    public float aliveTime; // 스킬 생존 시간

    public bool isMeteor; // 메테오면 날아오는 도중엔 데미지 없어야 하므로 만든 변수

    Rigidbody2D rigid; // 물리 입력을 받기위한 변수
    Vector2 direction; // 날아갈 방향

    public RandomSkill fireNormal2ExplodePrefab;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        bool destroySkill = aliveTimer > aliveTime;

        if (destroySkill)
        {
            if (isMeteor)
            {
                RandomSkill explode;
                explode = Instantiate(fireNormal2ExplodePrefab);

                explode.X = X;
                explode.Y = Y;

                explode.aliveTime = 0.5f;
                explode.damage = damage;
            }

            Destroy(gameObject);
            return;
        }
        else
        {
            MoveToimpactPonit();
        }

        aliveTimer += Time.fixedDeltaTime;
    }

    // 날아갈 방향을 정하는 함수
    public void setDirection()
    {
        Vector2 impactVector = new Vector2 (impactPonitX, impactPonitY);
        Vector2 nowVector = transform.position;

        direction = impactVector - nowVector;

        direction = direction.normalized;
    }

    // 폭발 지점으로 이동하는 함수
    private void MoveToimpactPonit()
    {
        setDirection();

        rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime);

        X = transform.position.x;
        Y = transform.position.y;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isMeteor)
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();

            if (damageable == null)
            {
                return;
            }

            damageable.TakeDamage(gameObject, damage);
        }
    }
}