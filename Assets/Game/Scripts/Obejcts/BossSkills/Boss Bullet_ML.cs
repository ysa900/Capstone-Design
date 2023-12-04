using UnityEngine;
using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Boss_Bullet_ML : Agent, IPullingObject, IDamageableSkill
{
    Player player;

    [SerializeField]
    float hp = 500;

    public Transform target;
    float damage = 10f;

    bool isDead;
    bool isOnLeftSide;

    float aliveTime = 5f;
    float aliveTimer = 0f;

    Animator animator;
    SpriteRenderer spriteRenderer;

    public override void OnEpisodeBegin()
    {
        //Init();
    }

    private void Start()
    {
        Init();
    }
    public void Init()
    {
        if(!(GameManager.instance.boss == null))
        {
            isDead = false;
            aliveTimer = 0f;
            GetComponent<CapsuleCollider2D>().enabled = true;
            hp = 500f;

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
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
    }

    private void FixedUpdate()
    {
        if(!isDead)
        {
            target = player.GetComponent<Transform>();

            Vector2 direction = new Vector2(transform.position.x - target.position.x, transform.position.y - target.position.y);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5f);
            transform.rotation = rotation;

            isOnLeftSide = Mathf.Cos(angle * Mathf.Deg2Rad) < 0; // cos값이 -면 플레이어를 기준으로 왼쪽에 있는 것

            spriteRenderer.flipY = isOnLeftSide;

            if (aliveTimer > aliveTime)
            {
                StartCoroutine(Dead());
            }
            aliveTimer += Time.fixedDeltaTime;
        }
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
        
    }

    [SerializeField] float speed;
    Vector2 nextMove;
    public override void OnActionReceived(ActionBuffers actions)
    {
        nextMove.x = actions.ContinuousActions[0];
        nextMove.y = actions.ContinuousActions[1];

        transform.Translate(nextMove * Time.deltaTime * speed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IPlayer iPlayer = other.GetComponent<IPlayer>();

        if (iPlayer == null)
        {
            return;
        }

        animator.SetTrigger("Hit");
        iPlayer.TakeDamage(damage);

        StartCoroutine(Dead());
    }

    public void TakeDamage(float damage)
    {
        if (!isDead)
        {
            hp -= damage;
            
            if (hp <= 0)
            {
                StartCoroutine(Dead());
            }
        }
    }

    IEnumerator Dead()
    {
        animator.SetTrigger("Hit");

        isDead = true;

        GetComponent<CapsuleCollider2D>().enabled = false;

        yield return new WaitForSeconds(0.35f); // 지정한 초 만큼 쉬기

        gameObject.SetActive(false);
        //Destroy(gameObject);
    }
}

